// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.AzureStorage;
using Umbrella.FileSystem.Disk;
using Umbrella.Internal.Mocks;
using Umbrella.Utilities.Compilation;
using Umbrella.Utilities.Helpers;
using Umbrella.Utilities.Options.Abstractions;
using Xunit;
using Xunit.Extensions.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]

namespace Umbrella.FileSystem.Test;

public class UmbrellaFileProviderTest
{
#if AZUREDEVOPS
        private static readonly string _storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString")!;
#else
	private static readonly string _storageConnectionString = "UseDevelopmentStorage=true";
#endif

	private const string TestFileName = "aspnet-mvc-logo.png";
	private static string? _baseDirectory;

	private static string BaseDirectory
	{
		get
		{
			if (string.IsNullOrEmpty(_baseDirectory))
			{
				string baseDirectory = AppContext.BaseDirectory.ToLowerInvariant();
				int indexToEndAt = baseDirectory.IndexOf(PathHelper.PlatformNormalize($@"\bin\{DebugUtility.BuildConfiguration}\net6.0"), StringComparison.Ordinal);
				_baseDirectory = baseDirectory.Remove(indexToEndAt, baseDirectory.Length - indexToEndAt);
			}

			return _baseDirectory;
		}
	}

	public static List<Func<IUmbrellaFileStorageProvider>> Providers = new()
	{
		CreateAzureBlobFileProvider,
		CreateDiskFileProvider
	};

	public static List<string> PathsToTest = new()
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

	public static List<object[]> ProvidersAndPathsMemberData = new();

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
	public async Task CreateAsync_FromPathAsync(Func<IUmbrellaFileStorageProvider> providerFunc, string path)
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
	public async Task CreateAsync_FromVirtualPath_Write_DeleteFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		IUmbrellaFileInfo file = await provider.CreateAsync($"~/images/{TestFileName}");

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		//Cleanup
		_ = await provider.DeleteAsync(file);
	}

	[Theory]
	[MemberData(nameof(ProvidersAndPathsMemberData))]
	public async Task CreateAsync_Write_ReadBytes_DeleteFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc, string path)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		//Create the file
		IUmbrellaFileInfo file = await provider.CreateAsync(path);

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		bytes = await file.ReadAsByteArrayAsync();

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		//Cleanup
		_ = await provider.DeleteAsync(file);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_GetAsync_ReadBytes_DeleteFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{TestFileName}");

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		//Get the file
		IUmbrellaFileInfo? retrievedFile = await provider.GetAsync($"/images/{TestFileName}");

		Assert.NotNull(retrievedFile);

		CheckWrittenFileAssertions(provider, retrievedFile!, bytes.Length, TestFileName);

		_ = await file.ReadAsByteArrayAsync();
		Assert.Equal(bytes.Length, retrievedFile!.Length);

		//Cleanup
		_ = await provider.DeleteAsync(file);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_GetAsync_ReadBytes_DeleteFile_CasingMismatchAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{TestFileName}");

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		// Get the file but with a different casing
		IUmbrellaFileInfo? retrievedFile = await provider.GetAsync($"/images/{TestFileName.ToUpperInvariant()}");

		Assert.NotNull(retrievedFile);

		CheckWrittenFileAssertions(provider, retrievedFile!, bytes.Length, TestFileName);

		_ = await file.ReadAsByteArrayAsync();
		Assert.Equal(bytes.Length, retrievedFile!.Length);

		//Cleanup
		_ = await provider.DeleteAsync(file);
	}

	[Theory]
	[MemberData(nameof(ProvidersAndPathsMemberData))]
	public async Task CreateAsync_Write_ReadStream_DeleteFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc, string path)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await provider.DeleteAsync(file);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_GetAsync_ReadStream_DeleteFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{TestFileName}");

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		//Get the file
		IUmbrellaFileInfo? retrievedFile = await provider.GetAsync($"/images/{TestFileName}");

		Assert.NotNull(retrievedFile);

		CheckWrittenFileAssertions(provider, retrievedFile!, bytes.Length, TestFileName);

		byte[] retrievedBytes;

		using (var ms = new MemoryStream())
		{
			await file.WriteToStreamAsync(ms);
			retrievedBytes = ms.ToArray();
		}

		Assert.Equal(bytes.Length, retrievedBytes.Length);

		//Cleanup
		_ = await provider.DeleteAsync(file);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task GetAsync_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		IUmbrellaFileInfo? retrievedFile = await provider.GetAsync($"/images/doesnotexist.jpg");

		Assert.Null(retrievedFile);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_GetAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string path = "/images/createbutnowrite.jpg";
		var file = await provider.CreateAsync(path);

		Assert.True(file.IsNew);

		IUmbrellaFileInfo? reloadedFile = await provider.GetAsync(path);

		// Should fail as not writing to the file won't push it to blob storage
		Assert.Null(reloadedFile);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_ExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
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
	public async Task CreateAsync_Write_ExistsAsync_DeletePathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
	public async Task SaveAsyncBytes_GetAsync_DeletePathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		string subpath = $"/images/{TestFileName}";

		IUmbrellaFileInfo fileInfo = await provider.SaveAsync(subpath, bytes);

		CheckWrittenFileAssertions(provider, fileInfo, bytes.Length, TestFileName);

		IUmbrellaFileInfo? reloadedFileInfo = await provider.GetAsync(subpath);

		Assert.NotNull(reloadedFileInfo);

		CheckWrittenFileAssertions(provider, reloadedFileInfo!, bytes.Length, TestFileName);

		//Cleanup
		bool deleted = await provider.DeleteAsync(subpath);

		Assert.True(deleted);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task SaveAsyncStream_GetAsync_DeletePathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		Stream stream = File.OpenRead(physicalPath);
		stream.Position = 20;

		string subpath = $"/images/{TestFileName}";

		var fileInfo = await provider.SaveAsync(subpath, stream);

		CheckWrittenFileAssertions(provider, fileInfo, (int)stream.Length, TestFileName);

		IUmbrellaFileInfo? reloadedFileInfo = await provider.GetAsync(subpath);

		Assert.NotNull(reloadedFileInfo);

		CheckWrittenFileAssertions(provider, reloadedFileInfo!, (int)stream.Length, TestFileName);

		//Cleanup
		bool deleted = await provider.DeleteAsync(subpath);

		Assert.True(deleted);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task SaveAsyncBytes_ExistsAsync_DeletePathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
	public async Task SaveAsyncStream_ExistsAsync_DeletePathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		Stream stream = File.OpenRead(physicalPath);
		stream.Position = 20;

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
	public async Task CopyAsync_FromPath_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
		=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			_ = await provider.CopyAsync("~/images/notexists.jpg", "~/images/willfail.png");
		}));

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CopyAsync_FromFileBytes_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc) =>
		//Should be a file system exception with a file not found exception inside
		await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, bytes);

			_ = await provider.DeleteAsync(fileInfo);

			//At this point the file will not exist
			_ = await provider.CopyAsync(fileInfo, "~/images/willfail.jpg");
		}));

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CopyAsync_FromFileStream_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc) =>
		//Should be a file system exception with a file not found exception inside
		await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			Stream stream = File.OpenRead(physicalPath);
			stream.Position = 20;

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, stream);

			_ = await provider.DeleteAsync(fileInfo);

			//At this point the file will not exist
			_ = await provider.CopyAsync(fileInfo, "~/images/willfail.jpg");
		}));

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_CopyAsync_FromPath_ToPathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await copy.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(copy);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_CopyAsync_FromFile_ToPathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await copy.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(copy);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_CopyAsync_FromFile_ToFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		string subpath = $"/images/{TestFileName}";

		IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		//Create the copy file
		var copy = await provider.CreateAsync("/images/xx/copy.png");

		//Act
		_ = await provider.CopyAsync(file, copy);

		//Assert
		Assert.Equal(bytes.Length, copy.Length);

		//Read the file into memory and cache for our comparison
		_ = await copy.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(copy);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_CopyAsync_FromPath_ToPath_WithMetadataAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await copy.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(copy);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_CopyAsync_FromFile_ToPath_WithMetadataAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await copy.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(copy);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_CopyAsync_FromFile_ToFile_WithMetadataAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await provider.CopyAsync(file, copy);

		//Assert
		Assert.Equal(bytes.Length, copy.Length);
		Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
		Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

		//Read the file into memory and cache for our comparison
		_ = await copy.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, copy, bytes.Length, "copy.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(copy);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_CopyAsync_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
		=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			// Should fail because you can't copy a new file
			var fileInfo = await provider.CreateAsync("~/images/testimage.jpg");

			var copy = await provider.CopyAsync(fileInfo, "~/images/copy.jpg");
		}));
	#endregion

	#region Move
	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task MoveAsync_FromPath_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
		=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			_ = await provider.MoveAsync("~/images/notexists.jpg", "~/images/willfail.png");
		}));

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task MoveAsync_FromFileBytes_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc) =>
		//Should be a file system exception with a file not found exception inside
		await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			byte[] bytes = File.ReadAllBytes(physicalPath);

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, bytes);

			_ = await provider.DeleteAsync(fileInfo);

			//At this point the file will not exist
			_ = await provider.MoveAsync(fileInfo, "~/images/willfail.jpg");
		}));

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task MoveAsync_FromFileStream_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc) =>
		//Should be a file system exception with a file not found exception inside
		await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

			Stream stream = File.OpenRead(physicalPath);
			stream.Position = 20;

			string subpath = $"/images/{TestFileName}";

			var fileInfo = await provider.SaveAsync(subpath, stream);

			_ = await provider.DeleteAsync(fileInfo);

			//At this point the file will not exist
			_ = await provider.MoveAsync(fileInfo, "~/images/willfail.jpg");
		}));

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_MoveAsync_FromPath_ToPathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await move.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(move);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_MoveAsync_FromFile_ToPathAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await move.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(move);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_MoveAsync_FromFile_ToFileAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		string subpath = $"/images/{TestFileName}";

		IUmbrellaFileInfo file = await provider.CreateAsync(subpath);

		Assert.True(file.IsNew);

		await file.WriteFromByteArrayAsync(bytes);

		CheckWrittenFileAssertions(provider, file, bytes.Length, TestFileName);

		//Create the move file
		var move = await provider.CreateAsync("/images/xx/move.png");

		//Act
		_ = await provider.MoveAsync(file, move);

		//Assert
		Assert.Equal(bytes.Length, move.Length);

		//Read the file into memory and cache for our comparison
		_ = await move.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(move);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_MoveAsync_FromPath_ToPath_WithMetadataAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await move.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(move);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_MoveAsync_FromFile_ToPath_WithMetadataAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await move.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(move);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_Write_MoveAsync_FromFile_ToFile_WithMetadataAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		_ = await provider.MoveAsync(file, move);

		//Assert
		Assert.Equal(bytes.Length, move.Length);
		Assert.Equal("Magic", await file.GetMetadataValueAsync<string>("Name"));
		Assert.Equal("Man", await file.GetMetadataValueAsync<string>("Description"));

		//Read the file into memory and cache for our comparison
		_ = await move.ReadAsByteArrayAsync(cacheContents: true);

		CheckWrittenFileAssertions(provider, move, bytes.Length, "move.png");

		//Cleanup
		_ = await provider.DeleteAsync(file);
		_ = await provider.DeleteAsync(move);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task CreateAsync_MoveAsync_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
		=> await Assert.ThrowsAsync<UmbrellaFileNotFoundException>((Func<Task>)(async () =>
		{
			var provider = providerFunc();

			// Should fail because you can't move a new file
			var fileInfo = await provider.CreateAsync("~/images/testimage.jpg");

			var move = await provider.MoveAsync(fileInfo, "~/images/move.jpg");
		}));
	#endregion

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task DeleteAsync_NotExistsAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Should fail silently
		bool deleted = await provider.DeleteAsync("/images/notexists.jpg");

		Assert.True(deleted);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task Set_Get_MetadataValueAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		// Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		IUmbrellaFileInfo? savedFile = await provider.GetAsync(subpath);

		Assert.NotNull(savedFile);
		Assert.False(savedFile!.IsNew);
		Assert.Equal("Richard", await file.GetMetadataValueAsync<string>("FirstName"));
		Assert.Equal("Edwards", await file.GetMetadataValueAsync<string>("LastName"));

		// Cleanup
		_ = await provider.DeleteAsync(savedFile);
	}

	[Theory]
	[Order(100)]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task Create_DeleteDirectory_TopLevelAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		// Create a top level file at the root of the directory.
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		string subpath = $"/tempfolder/{TestFileName}";
		_ = await provider.SaveAsync(subpath, bytes, false);

		await provider.DeleteDirectoryAsync("/tempfolder");

		// Assert
		Assert.False(await provider.ExistsAsync(subpath));
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task Create_DeleteDirectory_DownLevelAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		// Create a top level file at the root of the directory.
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		string subpath = $"/images/{TestFileName}";
		_ = await provider.SaveAsync(subpath, bytes, false);

		// Now create another 2 file in a nested subdirectories
		string downLevelSubPath1 = $"/images/sub-images/{TestFileName}";
		_ = await provider.SaveAsync(downLevelSubPath1, bytes, false);

		string downLevelSubPath2 = $"/images/sub-images/nested/{TestFileName}";
		_ = await provider.SaveAsync(downLevelSubPath2, bytes, false);

		string downLevelSubPath3 = $"/images/sub-images/nested2/{TestFileName}";
		_ = await provider.SaveAsync(downLevelSubPath3, bytes, false);

		string downLevelSubPath4 = $"/images/sub-images/nested2/nestedmore/{TestFileName}";
		_ = await provider.SaveAsync(downLevelSubPath4, bytes, false);

		// Now delete only the down level file directory, i.e. /images/sub-images
		// which should also delete the nested directory
		await provider.DeleteDirectoryAsync("/images/sub-images");

		// Assert
		Assert.True(await provider.ExistsAsync(subpath));
		Assert.False(await provider.ExistsAsync(downLevelSubPath1));
		Assert.False(await provider.ExistsAsync(downLevelSubPath2));
		Assert.False(await provider.ExistsAsync(downLevelSubPath3));
		Assert.False(await provider.ExistsAsync(downLevelSubPath4));

		// Cleanup
		_ = await provider.DeleteAsync(subpath);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task Create_EnumerateDirectoryAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		// Create a top level file at the root of the directory.
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

		byte[] bytes = File.ReadAllBytes(physicalPath);

		string subpath = $"/images/{TestFileName}";
		_ = await provider.SaveAsync(subpath, bytes, false);

		// Now create another 2 files in a subdirectory
		string downLevelSubPath1 = $"/images/sub-images/{TestFileName}";
		_ = await provider.SaveAsync(downLevelSubPath1, bytes, false);

		string downLevelSubPath2 = $"/images/sub-images/the-other-file.png";
		_ = await provider.SaveAsync(downLevelSubPath2, bytes, false);

		// Now enumerate the files
		var topLevelResults = await provider.EnumerateDirectoryAsync("/images");
		var downLevelResults = await provider.EnumerateDirectoryAsync("/images/sub-images");

		// Assert
		_ = Assert.Single(topLevelResults);
		Assert.Equal(2, downLevelResults.Count);

		Assert.Equal(subpath, topLevelResults.ElementAt(0).SubPath);
		Assert.Equal(downLevelSubPath1, downLevelResults.ElementAt(0).SubPath);
		Assert.Equal(downLevelSubPath2, downLevelResults.ElementAt(1).SubPath);

		// Cleanup
		_ = await provider.DeleteAsync(subpath);
		_ = await provider.DeleteAsync(downLevelSubPath1);
		_ = await provider.DeleteAsync(downLevelSubPath2);
	}

	[Theory]
	[MemberData(nameof(ProvidersMemberData))]
	public async Task Create_WriteMetaDataValue_Reload_WriteMetaDataWithoutLoadingAsync(Func<IUmbrellaFileStorageProvider> providerFunc)
	{
		var provider = providerFunc();

		//Arrange
		string physicalPath = PathHelper.PlatformNormalize($@"{BaseDirectory}\{TestFileName}");

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
		IUmbrellaFileInfo? reloadedFile = await provider.GetAsync(subpath);

		Assert.NotNull(reloadedFile);

		// Write without loading first
		await reloadedFile!.WriteMetadataChangesAsync();

		string metaName = await reloadedFile.GetMetadataValueAsync<string>("Name");
		string metaDescription = await reloadedFile.GetMetadataValueAsync<string>("Description");

		Assert.Equal("Magic", metaName);
		Assert.Equal("Man", metaDescription);
	}

	private static IUmbrellaFileStorageProvider CreateAzureBlobFileProvider()
	{
		var options = new UmbrellaAzureBlobStorageFileProviderOptions
		{
			StorageConnectionString = _storageConnectionString,
			AllowUnhandledFileAuthorizationChecks = true
		};

		if (options is IServicesResolverUmbrellaOptions servicesOptions)
			servicesOptions.Initialize(new ServiceCollection(), new ServiceCollection().BuildServiceProvider());

		var provider = new UmbrellaAzureBlobStorageFileProvider(
			CoreUtilitiesMocks.CreateLoggerFactory<UmbrellaAzureBlobStorageFileProvider>(),
			CoreUtilitiesMocks.CreateMimeTypeUtility(("png", "image/png"), ("jpg,", "image/jpg")),
			CoreUtilitiesMocks.CreateGenericTypeConverter());

		provider.InitializeOptions(options);

		return provider;
	}

	private static IUmbrellaFileStorageProvider CreateDiskFileProvider()
	{
		var options = new UmbrellaDiskFileStorageProviderOptions
		{
			RootPhysicalPath = BaseDirectory,
			AllowUnhandledFileAuthorizationChecks = true
		};

		if (options is IServicesResolverUmbrellaOptions servicesOptions)
			servicesOptions.Initialize(new ServiceCollection(), new ServiceCollection().BuildServiceProvider());

		var provider = new UmbrellaDiskFileStorageProvider(
			CoreUtilitiesMocks.CreateLoggerFactory<UmbrellaDiskFileStorageProvider>(),
			CoreUtilitiesMocks.CreateMimeTypeUtility(("png", "image/png"), ("jpg", "image/jpg")),
			CoreUtilitiesMocks.CreateGenericTypeConverter());

		provider.InitializeOptions(options);

		return provider;
	}

	private static void CheckWrittenFileAssertions(IUmbrellaFileStorageProvider provider, IUmbrellaFileInfo file, int length, string fileName)
	{
		CheckPOCOFileType(provider, file);
		Assert.False(file.IsNew);
		Assert.Equal(length, file.Length);
		Assert.Equal(DateTimeOffset.UtcNow.Date, file.LastModified!.Value.UtcDateTime.Date);
		Assert.Equal(fileName, file.Name);
		Assert.Equal("image/png", file.ContentType);
	}

	private static void CheckPOCOFileType(IUmbrellaFileStorageProvider provider, IUmbrellaFileInfo file)
	{
		object _ = provider switch
		{
			UmbrellaAzureBlobStorageFileProvider => Assert.IsType<UmbrellaAzureBlobFileInfo>(file),
			UmbrellaDiskFileStorageProvider => Assert.IsType<UmbrellaDiskFileInfo>(file),
			_ => throw new InvalidOperationException("Unsupported provider."),
		};
	}
}