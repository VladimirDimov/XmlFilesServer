using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Xml;
using Web.Models;
using Web.Services;
using Web.Utilities;

namespace UnitTests
{
    public class FileServiceTests
    {
        private readonly FileService _service;
        private Mock<ISerializationUtility> _serializationUtilityMock;
        private Mock<IFileUtility> _fileUtilityMock;
        private Mock<ILogger<FileService>> _loggerMock;

        public FileServiceTests()
        {
            _serializationUtilityMock = new Mock<ISerializationUtility>();
            _fileUtilityMock = new Mock<IFileUtility>();
            _loggerMock = new Mock<ILogger<FileService>>();

            _service = new FileService(_serializationUtilityMock.Object, _fileUtilityMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SaveFileAsyncShouldThrowIfModelIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveFilesAsync(null));
        }

        [Fact]
        public async Task SaveFileAsyncShouldCallDependingServicesCorrectlyWithSingleFile()
        {
            var fileName = "file1.xml";
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(x => x.FileName).Returns(fileName);

            bool overwriteExisting = true;

            await _service.SaveFilesAsync(new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = new List<IFormFile>
                {
                    fileMock.Object
                }
            });

            _serializationUtilityMock.Verify(
                x => x.XmlToJsonAsync(It.IsAny<Stream>()),
                Times.Once);

            _fileUtilityMock.Verify(
                x => x.SaveFileAsync(fileName, It.IsAny<string>(), overwriteExisting),
                Times.Once);
        }

        [Fact]
        public async Task SaveFileAsyncShouldCallDependingServicesCorrectlyWithMultipleFiles()
        {
            var numberOfFiles = 10;

            bool overwriteExisting = false;

            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = Enumerable.Range(1, numberOfFiles)
                            .Select(i =>
                            {
                                var fileName = $"file{i}.xml";
                                var fileMock = new Mock<IFormFile>();
                                fileMock.Setup(x => x.FileName).Returns(fileName);

                                return fileMock.Object;
                            })
            };

            await _service.SaveFilesAsync(model);

            _serializationUtilityMock.Verify(
                x => x.XmlToJsonAsync(It.IsAny<Stream>()),
                Times.Exactly(numberOfFiles));

            var expectedFileNames = Enumerable.Range(1, numberOfFiles).Select(i => $"file{i}.xml");

            _fileUtilityMock.Verify(
                x => x.SaveFileAsync(It.IsIn<string>(expectedFileNames), It.IsAny<string>(), overwriteExisting),
                Times.Exactly(numberOfFiles));
        }

        [Fact]
        public async Task SaveFileAsyncShouldCallXmlToJsonAsyncInParallel()
            => await AssertParallelCallAsync(() =>
            {
                var timeFlagsLock = new Object();
                var timeFlags = new List<DateTime>();

                _serializationUtilityMock
                    .Setup(x => x.XmlToJsonAsync(It.IsAny<Stream>()))
                    .Callback(async () =>
                    {
                        await Task.Run(() =>
                        {
                            lock (timeFlagsLock)
                            {
                                timeFlags.Add(DateTime.Now);
                            }

                            Thread.Sleep(1000);
                        });
                    })
                    .Returns(Task.FromResult("{}"));

                return Task.FromResult(timeFlags);
            });

        [Fact]
        public async Task SaveFileAsyncShouldCallSaveFileAsyncInParallel()
            => await AssertParallelCallAsync(() =>
            {
                var timeFlagsLock = new Object();
                var timeFlags = new List<DateTime>();

                _fileUtilityMock
                    .Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Callback(async () =>
                    {
                        await Task.Run(() =>
                        {
                            lock (timeFlagsLock)
                            {
                                timeFlags.Add(DateTime.Now);
                            }

                            Thread.Sleep(1000);
                        });
                    })
                    .Returns(Task.FromResult(new FileSaveResult()));

                return Task.FromResult(timeFlags);
            });

        [Fact]
        public async Task SaveFileAsyncShouldNotCallSaveFileAsyncIfInvalidXml()
        {
            var fileName = "file1.xml";
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(x => x.FileName).Returns(fileName);

            _serializationUtilityMock
                .Setup(x => x.XmlToJsonAsync(It.IsAny<Stream>()))
                .Throws<XmlException>();

            bool overwriteExisting = true;

            await _service.SaveFilesAsync(new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = new List<IFormFile>
                {
                    fileMock.Object
                }
            });

            _serializationUtilityMock.Verify(x => x.XmlToJsonAsync(It.IsAny<Stream>()), Times.Once);
            _fileUtilityMock.Verify(x => x.SaveFileAsync(fileName, It.IsAny<string>(), overwriteExisting), Times.Never);
        }

        private async Task AssertParallelCallAsync(Func<Task<List<DateTime>>> arrange)
        {
            var numberOfFiles = 3;

            bool overwriteExisting = false;

            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = Enumerable.Range(1, numberOfFiles)
                            .Select(i =>
                            {
                                var fileName = $"file{i}.xml";
                                var fileMock = new Mock<IFormFile>();
                                fileMock.Setup(x => x.FileName).Returns(fileName);

                                return fileMock.Object;
                            })
            };

            var timeFlags = await arrange.Invoke();

            await _service.SaveFilesAsync(model);

            while (timeFlags.Count < numberOfFiles)
            {
                /* 
                 * a workaround to await the mocked asynchronous callback
                 * this is needed, because the callback method in Moq accepts only Action, 
                 * which by default is not awaited
                 */
            }

            var maxTimeDifference = timeFlags.Max() - timeFlags.Min();

            Assert.True(maxTimeDifference < TimeSpan.FromMilliseconds(1000));
        }
    }
}