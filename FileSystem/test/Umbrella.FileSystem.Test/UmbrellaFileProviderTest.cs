using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;
using Xunit;
using Xunit.Extensions.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]

namespace Umbrella.FileSystem.Test
{
	public class UmbrellaFileProviderTest
	{
#if AZUREDEVOPS
        private static readonly string StorageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
#else
		private const string StorageConnectionString = "UseDevelopmentStorage=true";
#endif

		private const string TestFileName = "aspnet-mvc-logo.png";
		private static string s_BaseDirectory;

		private static string BaseDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(s_BaseDirectory))
				{
					string baseDirectory = AppContext.BaseDirectory.ToLowerInvariant();
					int indexToEndAt = baseDirectory.IndexOf($@"\bin\{DebugUtility.BuildConfiguration}\netcoreapp3.1");
					s_BaseDirectory = baseDirectory.Remove(indexToEndAt, baseDirectory.Length - indexToEndAt);
				}

				return s_BaseDirectory;
			}
		}

		public static List<Func<IUmbrellaFileProvider>> Providers = new List<Func<IUmbrellaFileProvider>>
		{
			() => CreateAzureBlobFileProvider(),
			() => CreateDiskFileProvider()
		};

		public static List<string> PathsToTest = new List<string>
		{
			$"~/images/{TestFileName}",
			$"/images/{TestFileName}",
			$@"\images\{TestFileName}",
			$@"\images/{TestFileName}",
			$@"\images\\\\\\subbie\\\\{TestFileName}",
			$"/images/subfolder1/sub2/{TestFileName}",
			$"/images//////subfolder1/////sub2/{TestFileName}",
			$"/images/subfolder1/su345  __---!!^^%b2/{TestFileName}",
			$"/images/subfolder1/sub   2/{TestFileName}"
		};

		public static List<object[]> ProvidersMemberData = Providers.Select(x => new object[] { x }).ToList();
		public static List<object[]> PathsToTestMemberData = PathsToTest.Select(x => new object[] { x }).ToList();

		public static List<object[]> ProvidersAndPathsMemberData = new List<object[]>();

		static UmbrellaFileProviderTest()
		{
			foreach (var provider in Providers)
			{
				foreach (string path in PathsToTest)
				{
					ProvidersAndPathsMemberData.Add(new object[] { provider, path });
				}
			}
		}

		[Theory]
		[MemberData(nameof(ProvidersAndPathsMemberData))]
		public async Task CreateAsync_FromPath(Func<IUmbrellaFileProvider> providerFunc, string path)
		{
			var provider = providerFunc();

			IUmbrellaFileInfo file = await provider.CreateAsync(path);

			CheckPOCOFileType(provider, file);
			Assert.Equal(-1, file.Length);
			Assert.Null(file.LastModified);
			Assert.Equal(TestFileName, file.Name);
			Assert.Equal("image/png", file.ContentType);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_FromVirtualPath_Write_DeleteFile(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			IUmbrellaFileInfo file = await provider.CreateAsync($"~/images/{TestFileName}");

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Cleanup
			await provider.DeleteAsync(file);
		}

		[Theory]
		[MemberData(nameof(ProvidersAndPathsMemberData))]
		public async Task CreateAsync_Write_ReadBytes_DeleteFile(Func<IUmbrellaFileProvider> providerFunc, string path)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			//Create the file
			IUmbrellaFileInfo file = await provider.CreateAsync(path);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			bytes = await file.ReadAsByteArrayAsync();

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Cleanup
			await provider.DeleteAsync(file);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_GetAsync_ReadBytes_DeleteFile(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{TestFileName}");

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Get the file
			IUmbrellaFileInfo retrievedFile = await provider.GetAsync($"/images/{TestFileName}");

			Assert.NotNull(retrievedFile);

			CheckWrittenFileAssertions(provider, retrievedFile, bytes.Length, TestFileName);

			await file.ReadAsByteArrayAsync();
			Assert.Equal(bytes.Length, retrievedFile.Length);

			//Cleanup
			await provider.DeleteAsync(file);
		}

		[Theory]
		[MemberData(nameof(ProvidersAndPathsMemberData))]
		public async Task CreateAsync_Write_ReadStream_DeleteFile(Func<IUmbrellaFileProvider> providerFunc, string path)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			//Create the file
			IUmbrellaFileInfo file = await provider.CreateAsync(path);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			using (var ms = new MemoryStream())
			{
				await file.WriteToStreamAsync(ms);
				bytes = ms.ToArray();
			}

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Cleanup
			await provider.DeleteAsync(file);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_GetAsync_ReadStream_DeleteFile(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{TestFileName}");

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Get the file
			IUmbrellaFileInfo retrievedFile = await provider.GetAsync($"/images/{TestFileName}");

			Assert.NotNull(retrievedFile);

			CheckWrittenFileAssertions(provider, retrievedFile, bytes.Length, TestFileName);

			byte[] retrievedBytes;

			using (var ms = new MemoryStream())
			{
				await file.WriteToStreamAsync(ms);
				retrievedBytes = ms.ToArray();
			}

			Assert.Equal(bytes.Length, retrievedBytes.Length);

			//Cleanup
			await provider.DeleteAsync(file);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task GetAsync_NotExists(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			IUmbrellaFileInfo retrievedFile = await provider.GetAsync($"/images/doesnotexist.jpg");

			Assert.Null(retrievedFile);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_GetAsync(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string path = "/images/createbutnowrite.jpg";
			var file = await provider.CreateAsync(path);

			Assert.True(file.IsNew);

			file = await provider.GetAsync(path);

			//Should fail as not writing to the file won't push it to blob storage
			Assert.Null(file);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_ExistsAsync(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string path = "/images/createbutnowrite.jpg";
			var file = await provider.CreateAsync(path);

			Assert.True(file.IsNew);

			bool exists = await provider.ExistsAsync(path);

			// Should be false as not calling write shouldn't create anything
			Assert.False(exists);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_ExistsAsync_DeletePath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			bool exists = await provider.ExistsAsync(subpath);

			Assert.True(exists);

			// Cleanup
			bool deleted = await provider.DeleteAsync(subpath);

			Assert.True(deleted);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task SaveAsyncBytes_GetAsync_DeletePath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, bytes);

			CheckWrittenFileAssertions(provider, fileInfo, bytes.Length, TestFileName);

			fileInfo = await provider.GetAsync(subpath);

			CheckWrittenFileAssertions(provider, fileInfo, bytes.Length, TestFileName);

			//Cleanup
			bool deleted = await provider.DeleteAsync(subpath);

			Assert.True(deleted);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task SaveAsyncStream_GetAsync_DeletePath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			Stream stream = File.OpenRead(physicalPath);

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, stream);

			CheckWrittenFileAssertions(provider, fileInfo, (int)stream.Length, TestFileName);

			fileInfo = await provider.GetAsync(subpath);

			CheckWrittenFileAssertions(provider, fileInfo, (int)stream.Length, TestFileName);

			//Cleanup
			bool deleted = await provider.DeleteAsync(subpath);

			Assert.True(deleted);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task SaveAsyncBytes_ExistsAsync_DeletePath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, bytes);

			CheckWrittenFileAssertions(provider, fileInfo, bytes.Length, TestFileName);

			bool exists = await provider.ExistsAsync(subpath);

			Assert.True(exists);

			//Cleanup
			bool deleted = await provider.DeleteAsync(subpath);

			Assert.True(deleted);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task SaveAsyncStream_ExistsAsync_DeletePath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			Stream stream = File.OpenRead(physicalPath);

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, stream);

			CheckWrittenFileAssertions(provider, fileInfo, (int)stream.Length, TestFileName);

			bool exists = await provider.ExistsAsync(subpath);

			Assert.True(exists);

			//Cleanup
			bool deleted = await provider.DeleteAsync(subpath);

			Assert.True(deleted);
		}

		#region Copy
		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CopyAsync_FromPath_NotExists(Func<IUmbrellaFileProvider> providerFunc)
			=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				await provider.CopyAsync("~/images/notexists.jpg", "~/images/willfail.png");
			});

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CopyAsync_FromFileBytes_NotExists(Func<IUmbrellaFileProvider> providerFunc) =>
			//Should be a file system exception with a file not found exception inside
			await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				string physicalPath = $@"{BaseDirectory}\{TestFileName}";

				byte[] bytes = File.ReadAllBytes(physicalPath);

				string subpath = $"/images/{TestFileName}";

				var fileInfo = await provider.SaveAsync(subpath, bytes);

				await provider.DeleteAsync(fileInfo);

				//At this point the file will not exist
				await provider.CopyAsync(fileInfo, "~/images/willfail.jpg");
			});

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CopyAsync_FromFileStream_NotExists(Func<IUmbrellaFileProvider> providerFunc) =>
			//Should be a file system exception with a file not found exception inside
			await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				string physicalPath = $@"{BaseDirectory}\{TestFileName}";

				Stream stream = File.OpenRead(physicalPath);

				string subpath = $"/images/{TestFileName}";

				var fileInfo = await provider.SaveAsync(subpath, stream);

				await provider.DeleteAsync(fileInfo);

				//At this point the file will not exist
				await provider.CopyAsync(fileInfo, "~/images/willfail.jpg");
			});

		//[Theory]
		//[MemberData(nameof(ProvidersMemberData))]
		//public async Task CopyAsync_InvalidSourceType(Func<IUmbrellaFileProvider> providerFunc)
		//{
		//    //TODO: Test that files coming from one provider cannot be used with a different one.
		//    //Maybe look at building this in somehow?
		//}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_CopyAsync_FromPath_ToPath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var copy = await provider.CopyAsync(subpath, "/images/xx/copy.png");

			//Assert
			Assert.Equal(bytes.Length, copy.Length);

			//Read the file into memory and cache for our comparison
			await copy.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(copy);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_CopyAsync_FromFile_ToPath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var copy = await provider.CopyAsync(file, "/images/xx/copy.png");

			//Assert
			Assert.Equal(bytes.Length, copy.Length);

			//Read the file into memory and cache for our comparison
			await copy.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(copy);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_CopyAsync_FromFile_ToFile(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Create the copy file
			var copy = await provider.CreateAsync("/images/xx/copy.png");

			//Act
			await provider.CopyAsync(file, copy);

			//Assert
			Assert.Equal(bytes.Length, copy.Length);

			//Read the file into memory and cache for our comparison
			await copy.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(copy);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_CopyAsync_FromPath_ToPath_WithMetadata(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var copy = await provider.CopyAsync(subpath, "/images/xx/copy.png");

			//Assert
			Assert.Equal(bytes.Length, copy.Length);
			Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
			Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

			//Read the file into memory and cache for our comparison
			await copy.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(copy);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_CopyAsync_FromFile_ToPath_WithMetadata(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var copy = await provider.CopyAsync(file, "/images/xx/copy.png");

			//Assert
			Assert.Equal(bytes.Length, copy.Length);
			Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
			Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

			//Read the file into memory and cache for our comparison
			await copy.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(copy);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_CopyAsync_FromFile_ToFile_WithMetadata(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);
			
			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			//Create the copy file
			var copy = await provider.CreateAsync("/images/xx/copy.png");

			//Act
			await provider.CopyAsync(file, copy);

			//Assert
			Assert.Equal(bytes.Length, copy.Length);
			Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
			Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

			//Read the file into memory and cache for our comparison
			await copy.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(copy);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_CopyAsync_NotExists(Func<IUmbrellaFileProvider> providerFunc)
			=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				// Should fail because you can't copy a new file
				var fileInfo = await provider.CreateAsync("~/images/testimage.jpg");

				var copy = await provider.CopyAsync(fileInfo, "~/images/copy.jpg");
			});
		#endregion

		#region Move
		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task MoveAsync_FromPath_NotExists(Func<IUmbrellaFileProvider> providerFunc)
			=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				await provider.MoveAsync("~/images/notexists.jpg", "~/images/willfail.png");
			});

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task MoveAsync_FromFileBytes_NotExists(Func<IUmbrellaFileProvider> providerFunc) =>
			//Should be a file system exception with a file not found exception inside
			await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				string physicalPath = $@"{BaseDirectory}\{TestFileName}";

				byte[] bytes = File.ReadAllBytes(physicalPath);

				string subpath = $"/images/{TestFileName}";

				var fileInfo = await provider.SaveAsync(subpath, bytes);

				await provider.DeleteAsync(fileInfo);

				//At this point the file will not exist
				await provider.MoveAsync(fileInfo, "~/images/willfail.jpg");
			});

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task MoveAsync_FromFileStream_NotExists(Func<IUmbrellaFileProvider> providerFunc) =>
			//Should be a file system exception with a file not found exception inside
			await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				string physicalPath = $@"{BaseDirectory}\{TestFileName}";

				Stream stream = File.OpenRead(physicalPath);

				string subpath = $"/images/{TestFileName}";

				var fileInfo = await provider.SaveAsync(subpath, stream);

				await provider.DeleteAsync(fileInfo);

				//At this point the file will not exist
				await provider.MoveAsync(fileInfo, "~/images/willfail.jpg");
			});

		//[Theory]
		//[MemberData(nameof(ProvidersMemberData))]
		//public async Task MoveAsync_InvalidSourceType(Func<IUmbrellaFileProvider> providerFunc)
		//{
		//    //TODO: Test that files coming from one provider cannot be used with a different one.
		//    //Maybe look at building this in somehow?
		//}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_MoveAsync_FromPath_ToPath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var move = await provider.MoveAsync(subpath, "/images/xx/move.png");

			//Assert
			Assert.Equal(bytes.Length, move.Length);

			//Read the file into memory and cache for our comparison
			await move.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(move);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_MoveAsync_FromFile_ToPath(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var move = await provider.MoveAsync(file, "/images/xx/move.png");

			//Assert
			Assert.Equal(bytes.Length, move.Length);

			//Read the file into memory and cache for our comparison
			await move.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(move);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_MoveAsync_FromFile_ToFile(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Create the move file
			var move = await provider.CreateAsync("/images/xx/move.png");

			//Act
			await provider.MoveAsync(file, move);

			//Assert
			Assert.Equal(bytes.Length, move.Length);

			//Read the file into memory and cache for our comparison
			await move.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(move);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_MoveAsync_FromPath_ToPath_WithMetadata(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var move = await provider.MoveAsync(subpath, "/images/xx/move.png");

			//Assert
			Assert.Equal(bytes.Length, move.Length);
			Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
			Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

			//Read the file into memory and cache for our comparison
			await move.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(move);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_MoveAsync_FromFile_ToPath_WithMetadata(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			//Act
			var move = await provider.MoveAsync(file, "/images/xx/move.png");

			//Assert
			Assert.Equal(bytes.Length, move.Length);
			Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
			Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

			//Read the file into memory and cache for our comparison
			await move.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(move);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_Write_MoveAsync_FromFile_ToFile_WithMetadata(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			//Create the move file
			var move = await provider.CreateAsync("/images/xx/move.png");

			//Act
			await provider.MoveAsync(file, move);

			//Assert
			Assert.Equal(bytes.Length, move.Length);
			Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
			Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

			//Read the file into memory and cache for our comparison
			await move.ReadAsByteArrayAsync(cacheContents: true);

			CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

			//Cleanup
			await provider.DeleteAsync(file);
			await provider.DeleteAsync(move);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task CreateAsync_MoveAsync_NotExists(Func<IUmbrellaFileProvider> providerFunc)
			=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>(async () =>
			{
				var provider = providerFunc();

				// Should fail because you can't move a new file
				var fileInfo = await provider.CreateAsync("~/images/testimage.jpg");

				var move = await provider.MoveAsync(fileInfo, "~/images/move.jpg");
			});
		#endregion

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task DeleteAsync_NotExists(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Should fail silently
			bool deleted = await provider.DeleteAsync("/images/notexists.jpg");

			Assert.True(deleted);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task Set_Get_MetadataValueAsync(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			// Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

			// Act
			await file.SetMetadataValueAsync("FirstName", "Richard");
			await file.SetMetadataValueAsync("LastName", "Edwards");

			// Assert
			file = await provider.GetAsync(subpath);

			Assert.False(file.IsNew);
			Assert.Equal("Richard", await file.GetMetadataValueAsync<string>("FirstName"));
			Assert.Equal("Edwards", await file.GetMetadataValueAsync<string>("LastName"));

			// Cleanup
			await provider.DeleteAsync(file);
		}

		[Theory]
		[Order(100)]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task Create_DeleteDirectory_TopLevel(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			// Create a top level file at the root of the directory.
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/tempfolder/{TestFileName}";
			await provider.SaveAsync(subpath, bytes, false);

			await provider.DeleteDirectoryAsync("/tempfolder");

			// Assert
			Assert.False(await provider.ExistsAsync(subpath));
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task Create_DeleteDirectory_DownLevel(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			// Create a top level file at the root of the directory.
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";
			await provider.SaveAsync(subpath, bytes, false);

			// Now create another 2 file in a nested subdirectories
			string downLevelSubPath1 = $"/images/sub-images/{TestFileName}";
			await provider.SaveAsync(downLevelSubPath1, bytes, false);

			string downLevelSubPath2 = $"/images/sub-images/nested/{TestFileName}";
			await provider.SaveAsync(downLevelSubPath2, bytes, false);

			// Now delete only the down level file directory, i.e. /images/sub-images
			// which should also delete the nested directory
			await provider.DeleteDirectoryAsync("/images/sub-images");

			// Assert
			Assert.True(await provider.ExistsAsync(subpath));
			Assert.False(await provider.ExistsAsync(downLevelSubPath1));
			Assert.False(await provider.ExistsAsync(downLevelSubPath2));

			// Cleanup
			await provider.DeleteAsync(subpath);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task Create_EnumerateDirectory(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			// Create a top level file at the root of the directory.
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";
			await provider.SaveAsync(subpath, bytes, false);

			// Now create another 2 files in a subdirectory
			string downLevelSubPath1 = $"/images/sub-images/{TestFileName}";
			await provider.SaveAsync(downLevelSubPath1, bytes, false);

			string downLevelSubPath2 = $"/images/sub-images/the-other-file.png";
			await provider.SaveAsync(downLevelSubPath2, bytes, false);

			// Now enumerate the files
			var topLevelResults = await provider.EnumerateDirectoryAsync("/images");
			var downLevelResults = await provider.EnumerateDirectoryAsync("/images/sub-images");

			// Assert
			Assert.Equal(1, topLevelResults.Count);
			Assert.Equal(2, downLevelResults.Count);

			Assert.Equal(subpath, topLevelResults.ElementAt(0).SubPath);
			Assert.Equal(downLevelSubPath1, downLevelResults.ElementAt(0).SubPath);
			Assert.Equal(downLevelSubPath2, downLevelResults.ElementAt(1).SubPath);

			// Cleanup
			await provider.DeleteAsync(subpath);
			await provider.DeleteAsync(downLevelSubPath1);
			await provider.DeleteAsync(downLevelSubPath2);
		}

		[Theory]
		[MemberData(nameof(ProvidersMemberData))]
		public async Task Create_WriteMetaDataValue_Reload_WriteMetaDataWithoutLoading(Func<IUmbrellaFileProvider> providerFunc)
		{
			var provider = providerFunc();

			//Arrange
			string physicalPath = $@"{BaseDirectory}\{TestFileName}";

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

			Assert.True(file.IsNew);

			await file.WriteFromByteArrayAsync(bytes);

			// Write some metadata
			await file.SetMetadataValueAsync("Name", "Magic", writeChanges: false);
			await file.SetMetadataValueAsync("Description", "Man", writeChanges: false);
			await file.WriteMetadataChangesAsync();

			// Reload the file
			var reloadedFile = await provider.GetAsync(subpath);

			// Write without loading first
			await reloadedFile.WriteMetadataChangesAsync();

			string metaName = await reloadedFile.GetMetadataValueAsync<string>("Name");
			string metaDescription = await reloadedFile.GetMetadataValueAsync<string>("Description");

			Assert.Equal("Magic", metaName);
			Assert.Equal("Man", metaDescription);
		}

		private static IUmbrellaFileProvider CreateAzureBlobFileProvider()
		{
			var logger = new Mock<ILogger<UmbrellaAzureBlobStorageFileProvider>>();

			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(typeof(UmbrellaAzureBlobStorageFileProvider).FullName)).Returns(logger.Object);

			var mimeTypeUtility = new Mock<IMimeTypeUtility>();
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

			var genericTypeConverter = new Mock<IGenericTypeConverter>();
			genericTypeConverter.Setup(x => x.Convert(It.IsAny<string>(), (string)null, null)).Returns<string, string, Func<string, string>>((x, y, z) => x);

			var options = new UmbrellaAzureBlobStorageFileProviderOptions
			{
				StorageConnectionString = StorageConnectionString
			};

			var provider = new UmbrellaAzureBlobStorageFileProvider(loggerFactory.Object, mimeTypeUtility.Object, genericTypeConverter.Object);
			provider.InitializeOptions(options);

			return provider;
		}

		private static IUmbrellaFileProvider CreateDiskFileProvider()
		{
			var logger = new Mock<ILogger<UmbrellaDiskFileProvider>>();

			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(typeof(UmbrellaDiskFileProvider).FullName)).Returns(logger.Object);

			var mimeTypeUtility = new Mock<IMimeTypeUtility>();
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
			mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

			var genericTypeConverter = new Mock<IGenericTypeConverter>();
			genericTypeConverter.Setup(x => x.Convert(It.IsAny<string>(), (string)null, null)).Returns<string, string, Func<string, string>>((x, y, z) => x);

			var options = new UmbrellaDiskFileProviderOptions
			{
				RootPhysicalPath = BaseDirectory
			};

			var provider = new UmbrellaDiskFileProvider(loggerFactory.Object, mimeTypeUtility.Object, genericTypeConverter.Object);
			provider.InitializeOptions(options);

			return provider;
		}

		private void CheckWrittenFileAssertions(IUmbrellaFileProvider provider, IUmbrellaFileInfo file, int length, string fileName)
		{
			CheckPOCOFileType(provider, file);
			Assert.False(file.IsNew);
			Assert.Equal(length, file.Length);
			Assert.Equal(DateTimeOffset.UtcNow.Date, file.LastModified.Value.UtcDateTime.Date);
			Assert.Equal(fileName, file.Name);
			Assert.Equal("image/png", file.ContentType);
		}

		private void CheckPOCOFileType(IUmbrellaFileProvider provider, IUmbrellaFileInfo file)
		{
			switch (provider)
			{
				case UmbrellaAzureBlobStorageFileProvider _:
					Assert.IsType<UmbrellaAzureBlobStorageFileInfo>(file);
					break;
				case UmbrellaDiskFileProvider _:
					Assert.IsType<UmbrellaDiskFileInfo>(file);
					break;
				default:
					throw new Exception("Unsupported provider.");
			}
		}
	}
}