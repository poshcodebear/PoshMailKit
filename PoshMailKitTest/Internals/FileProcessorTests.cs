using MimeKit;
using Moq;
using PoshMailKit.Internals;
using System;
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
        public void GetFullPathName_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var fileProcessor = CreateFileProcessor(@"", GetMockFileSystem());
            string fileName = null;

            // Act
            var result = fileProcessor.GetFullPathName(
                fileName);

            // Assert
            Assert.True(false);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFileMimePart_SimpleFileAttachment_ReturnsCorrectMimePart()
        {
            // Arrange
            var fileName = @"test.txt";
            var filePath = @"C:\test";
            var fileContents = "This is a test text file";
            var expectedContentType = new ContentType("text", "plain");
            var expectedContentDisposition = new ContentDisposition(ContentDisposition.Attachment);

            var mockFiles = new Dictionary<string, MockFileData>()
            {
                { $"{filePath}\\{fileName}", new MockFileData("This is a test text file") },
            };
            var fileProcessor = CreateFileProcessor(filePath, new MockFileSystem(mockFiles));

            // Act
            var result = fileProcessor.GetFileMimePart(fileName);

            // Assert
            var stream = new System.IO.MemoryStream();
            var encoding = System.Text.Encoding.UTF8;

            result.Content.DecodeTo(stream);
            string actualContent = encoding.GetString(stream.ToArray());

            Assert.True(actualContent == fileContents,
                $"Filename: expected '{fileName}', actual '{result.FileName}'");
            Assert.True(actualContent == fileContents,
                $"ContentDisposition: expected '{expectedContentDisposition.Disposition}', actual '{result.ContentDisposition.Disposition}'");
            Assert.True(actualContent == fileContents,
                $"File contents: expected '{fileContents}', actual '{actualContent}'");
            Assert.True(actualContent == fileContents,
                $"ContentType: expected '{expectedContentType.MimeType}', actual '{result.ContentType.MimeType}'");
            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetFileMimePart_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var fileProcessor = CreateFileProcessor(@"", GetMockFileSystem());
            string fileName = null;
            ContentDisposition contentDisposition = null;
            string lable = null;

            // Act
            var result = fileProcessor.GetFileMimePart(
                fileName,
                contentDisposition,
                lable);

            // Assert
            Assert.True(false);
            mockRepository.VerifyAll();
        }
    }
}
