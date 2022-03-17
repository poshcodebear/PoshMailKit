using MimeKit;
using Moq;
using PoshMailKit.Internals;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace PoshMailKitTest.Internals
{
    public class FileProcessorTests
    {
        private MockRepository mockRepository;

        private Mock<IFileSystem> mockFileSystem;

        public FileProcessorTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            mockFileSystem = mockRepository.Create<IFileSystem>();
        }

        private IFileSystem GetMockFileSystem()
        {
            return mockFileSystem.Object;
        }

        private FileProcessor CreateFileProcessor(string filePath, IFileSystem fileSystem)
        {
            return new FileProcessor(filePath, fileSystem);
        }

        [Fact]
        public void GetFullPathName_FileNameOnly_ReturnsFullPath()
        {
            // Arrange
            var fileName = @"C:\test\tmp\test.txt";
            var filePath = @"C:\test\tmp";
            var expectedReturnPath = @"C:\test\tmp\test.txt";

            var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

            // Act
            var result = fileProcessor.GetFullPathName(fileName);

            // Assert
            Assert.True(result == expectedReturnPath,
                $"Expected: {expectedReturnPath}, actual: {result}");
            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFullPathName_FileRelativePath_ReturnsFullPath()
        {
            // Arrange
            var fileName = @"..\test.txt";
            var filePath = @"C:\test\tmp";
            var expectedReturnPath = @"C:\test\test.txt";

            var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

            // Act
            var result = fileProcessor.GetFullPathName(fileName);

            // Assert
            Assert.True(result == expectedReturnPath,
                $"Expected: {expectedReturnPath}, actual: {result}");
            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFullPathName_FileFullPath_ReturnsFullPath()
        {
            // Arrange
            var fileName = "test.txt";
            var filePath = @"C:\test\tmp";
            var expectedReturnPath = @"C:\test\tmp\test.txt";

            var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

            // Act
            var result = fileProcessor.GetFullPathName(fileName);

            // Assert
            Assert.True(result == expectedReturnPath,
                $"Expected: {expectedReturnPath}, actual: {result}");
            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFileMimePart_WithoutLabel_ReturnsCorrectMimePart()
        {
            // Arrange
            var fileName = @"test.txt";
            var filePath = @"C:\test";
            var fileContents = "This is a test text file";
            var contentDispositionType = ContentDispositionType.Attachment;
            var expectedContentType = new ContentType("text", "plain");
            var expectedContentDisposition = new ContentDisposition(contentDispositionType.ToString());

            var mockFiles = new Dictionary<string, MockFileData>()
            {
                { $"{filePath}\\{fileName}", new MockFileData(fileContents) },
            };
            var fileProcessor = CreateFileProcessor(filePath, new MockFileSystem(mockFiles));

            // Act
            var result = fileProcessor.GetFileMimePart(fileName, contentDispositionType);

            // Assert
            var stream = new System.IO.MemoryStream();
            var encoding = System.Text.Encoding.UTF8;

            result.Content.DecodeTo(stream);
            string actualContent = encoding.GetString(stream.ToArray());

            Assert.True(result.FileName == fileName,
                $"Filename: expected '{fileName}', actual '{result.FileName}'");
            Assert.True(result.ContentDisposition.Disposition == expectedContentDisposition.Disposition,
                $"ContentDispositionType: expected '{expectedContentDisposition.Disposition}', actual '{result.ContentDisposition.Disposition}'");
            Assert.True(actualContent == fileContents,
                $"File contents: expected '{fileContents}', actual '{actualContent}'");
            Assert.True(result.ContentType.MimeType == expectedContentType.MimeType,
                $"ContentType: expected '{expectedContentType.MimeType}', actual '{result.ContentType.MimeType}'");
            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFileMimePart_WithLabel_ReturnsCorrectMimePart()
        {
            // Arrange
            var fileName = @"test.png";
            var filePath = @"C:\test";
            var fileLabel = "test-label";
            var fileContents = new byte[] { 0xf3, 0xab, 0x12, 0xd7};
            var contentDispositionType = ContentDispositionType.Inline;
            var expectedContentType = new ContentType("image", "png");
            var expectedContentDisposition = new ContentDisposition(contentDispositionType.ToString());

            var mockFiles = new Dictionary<string, MockFileData>()
            {
                { $"{filePath}\\{fileName}", new MockFileData(fileContents) },
            };
            var fileProcessor = CreateFileProcessor(filePath, new MockFileSystem(mockFiles));

            // Act
            var result = fileProcessor.GetFileMimePart(fileName, contentDispositionType, fileLabel);

            // Assert
            var stream = new System.IO.MemoryStream();

            result.Content.DecodeTo(stream);
            var actualContent = stream.ToArray();

            Assert.True(result.FileName == fileName,
                $"Filename: expected '{fileName}', actual '{result.FileName}'");
            Assert.True(result.ContentId == fileLabel,
                $"Filename: expected '{fileLabel}', actual '{result.ContentId}'");
            Assert.True(result.ContentDisposition.Disposition == expectedContentDisposition.Disposition,
                $"ContentDispositionType: expected '{expectedContentDisposition.Disposition}', actual '{result.ContentDisposition.Disposition}'");
            Assert.True(Enumerable.SequenceEqual(actualContent, fileContents),
                $"File contents: expected '{BitConverter.ToString(fileContents)}', actual '{BitConverter.ToString(actualContent)}'");
            Assert.True(result.ContentType.MimeType == expectedContentType.MimeType,
                $"ContentType: expected '{expectedContentType.MimeType}', actual '{result.ContentType.MimeType}'");
            mockRepository.VerifyAll();
        }
    }
}
