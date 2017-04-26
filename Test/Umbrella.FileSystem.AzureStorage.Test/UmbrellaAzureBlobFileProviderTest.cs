using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities.Mime;
using Xunit;

namespace Umbrella.FileSystem.AzureStorage.Test
{
    public class UmbrellaAzureBlobFileProviderTest
    {
        //TODO: When moving to GitHub this connection string needs to be dynamically set somehow before executing the tests
        private const string c_StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=umbrellablobtest;AccountKey=eaxPzjIwVy4WQTCUQnUIL6cIYbzFolVp72nfStCQMNXU8lG4I/zaa2ll1wdiZ2q2h4roIA+DCISXnwhD2nRU0A==;EndpointSuffix=core.windows.net";
        //private const string c_StorageConnectionString = "UseDevelopmentStorage=true";

        private string m_BaseDirectory;

        private string BaseDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(m_BaseDirectory))
                {
                    int indexToEndAt = AppContext.BaseDirectory.IndexOf(@"\bin\Debug\netcoreapp1.1");
                    m_BaseDirectory = AppContext.BaseDirectory.Remove(indexToEndAt, AppContext.BaseDirectory.Length - indexToEndAt);
                }

                return m_BaseDirectory;
            }
        }

        [Fact]
        public async Task CreateAsync_FromVirtualPath()
        {
            IUmbrellaFileProvider provider = CreateFileProvider();

            string fileName = "aspnet-mvc-logo.png";

            IUmbrellaFileInfo file = await provider.CreateAsync($"~/images/{fileName}");

            Assert.IsType<UmbrellaAzureBlobFileInfo>(file);
            Assert.Equal(-1, file.Length);
            Assert.Null(file.LastModified);
            Assert.Equal(fileName, file.Name);
            Assert.Equal("image/png", file.ContentType);
        }

        [Fact]
        public async Task CreateAsync_FromRelativePath()
        {
            IUmbrellaFileProvider provider = CreateFileProvider();

            string fileName = "aspnet-mvc-logo.png";

            IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{fileName}");

            Assert.IsType<UmbrellaAzureBlobFileInfo>(file);
            Assert.True(file.IsNew);
            Assert.Equal(-1, file.Length);
            Assert.Null(file.LastModified);
            Assert.Equal(fileName, file.Name);
            Assert.Equal("image/png", file.ContentType);
        }

        [Fact]
        public async Task CreateAsync_FromVirtualPath_Write_DeleteFile()
        {
            IUmbrellaFileProvider provider = CreateFileProvider();

            string fileName = "aspnet-mvc-logo.png";

            var physicalPath = $@"{BaseDirectory}\{fileName}";

            byte[] bytes = File.ReadAllBytes(physicalPath);

            IUmbrellaFileInfo file = await provider.CreateAsync($"~/images/{fileName}");

            Assert.True(file.IsNew);

            await file.WriteFromByteArrayAsync(bytes);

            CheckWrittenFileAssertions(file, bytes, fileName);

            //Cleanup
            await provider.DeleteAsync(file);
        }

        [Fact]
        public async Task CreateAsync_FromRelativePath_Write_DeleteFile()
        {
            IUmbrellaFileProvider provider = CreateFileProvider();

            string fileName = "aspnet-mvc-logo.png";

            var physicalPath = $@"{BaseDirectory}\{fileName}";

            byte[] bytes = File.ReadAllBytes(physicalPath);

            //Create the file
            IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{fileName}");

            Assert.True(file.IsNew);

            await file.WriteFromByteArrayAsync(bytes);

            CheckWrittenFileAssertions(file, bytes, fileName);

            //Cleanup
            await provider.DeleteAsync(file);
        }

        [Fact]
        public async Task CreateAsync_Write_Read_DeleteFile()
        {
            
        }

        [Fact]
        public async Task CreateAsync_Write_GetAsync_Read_DeleteFile()
        {
            IUmbrellaFileProvider provider = CreateFileProvider();

            string fileName = "aspnet-mvc-logo.png";

            var physicalPath = $@"{BaseDirectory}\{fileName}";

            byte[] bytes = File.ReadAllBytes(physicalPath);

            IUmbrellaFileInfo file = await provider.CreateAsync($"/images/{fileName}");

            Assert.True(file.IsNew);

            await file.WriteFromByteArrayAsync(bytes);

            CheckWrittenFileAssertions(file, bytes, fileName);

            //Get the file
            IUmbrellaFileInfo retrievedFile = await provider.GetAsync($"/images/{fileName}");

            Assert.NotNull(retrievedFile);

            CheckWrittenFileAssertions(retrievedFile, bytes, fileName);

            byte[] retrievedBytes = await file.ReadAsByteArrayAsync();
            Assert.Equal(bytes.Length, retrievedFile.Length);

            //Cleanup
            await provider.DeleteAsync(file);
        }

        [Fact]
        public async Task GetAsync_NotExists()
        {

        }

        [Fact]
        public async Task CreateAsync_GetAsync()
        {
            var provider = CreateFileProvider();

            //Should fail as not writing to the file won't push it to blob storage
        }

        [Fact]
        public async Task CreateAsync_ExistsAsync()
        {
            //Should be false as not calling write shouldn't create the blob
        }

        [Fact]
        public async Task CreateAsync_Write_ExistsAsync_DeletePath()
        {

        }

        [Fact]
        public async Task SaveAsync_GetAsync_DeletePath()
        {
            
        }

        [Fact]
        public async Task SaveAsync_ExistsAsync_DeletePath()
        {

        }

        [Fact]
        public async Task CopyAsync_FromPath_NotExists()
        {
            //Should be a file system exception with a file not found exception inside
        }

        [Fact]
        public async Task CopyAsync_FromFile_NotExists()
        {
            //Should be a file system exception with a file not found exception inside
        }

        [Fact]
        public async Task CopyAsync_InvalidSourceType()
        {

        }

        [Fact]
        public async Task CreateAsync_Write_CopyAsync_FromPath_ToPath()
        {
            var provider = CreateFileProvider();
            
        }

        [Fact]
        public async Task CreateAsync_Write_CopyAsync_FromFile_ToPath()
        {

        }

        [Fact]
        public async Task CreateAsync_Write_CopyAsync_FromFile_ToFile()
        {

        }

        [Fact]
        public async Task CreateAsync_CopyAsync()
        {
            //Should fail because you can't copy a new file
        }

        [Fact]
        public async Task DeleteAsync_NotExists()
        {
            //Should fail silently
        }

        private IUmbrellaFileProvider CreateFileProvider()
        {
            var logger = new Mock<ILogger<UmbrellaAzureBlobFileProvider>>();

            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger("UmbrellaAzureBlobFileProvider")).Returns(logger.Object);

            var mimeTypeUtility = new Mock<IMimeTypeUtility>();
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("png")))).Returns("image/png");
            mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith("jpg")))).Returns("image/jpg");

            var options = new UmbrellaAzureBlobFileProviderOptions
            {
                StorageConnectionString = c_StorageConnectionString
            };

            return new UmbrellaAzureBlobFileProvider(loggerFactory.Object, mimeTypeUtility.Object, options);
        }

        private void CheckWrittenFileAssertions(IUmbrellaFileInfo file, byte[] bytes, string fileName)
        {
            Assert.False(file.IsNew);
            Assert.IsType<UmbrellaAzureBlobFileInfo>(file);
            Assert.Equal(bytes.Length, file.Length);
            Assert.Equal(DateTimeOffset.UtcNow.Date, file.LastModified.Value.UtcDateTime.Date);
            Assert.Equal(fileName, file.Name);
            Assert.Equal("image/png", file.ContentType);
        }
    }
}