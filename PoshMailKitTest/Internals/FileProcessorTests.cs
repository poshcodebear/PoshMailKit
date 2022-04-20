using MimeKit;
using Moq;
using PoshMailKit.Internals;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace PoshMailKitTest.Internals;

public class FileProcessorTests
{
    private readonly MockRepository mockRepository;

    private readonly Mock<IFileSystem> mockFileSystem;

    public FileProcessorTests()
    {
        mockRepository = new MockRepository(MockBehavior.Strict);
        mockFileSystem = mockRepository.Create<IFileSystem>();
    }

    private IFileSystem GetMockFileSystem()
    {
        return mockFileSystem.Object;
    }

    private static FileProcessor CreateFileProcessor(string filePath, IFileSystem fileSystem)
    {
        return new FileProcessor(filePath, fileSystem);
    }

    [Fact]
    public void GetFullPathName_FileFullPath_ReturnsFullPath()
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
            $"Actual: {result}, expected: {expectedReturnPath}");
        mockRepository.VerifyAll();
    }

    [Fact]
    public void GetFullPathName_FileFullNetworkPath_ReturnsFullPath()
    {
        // Arrange
        var fileName = @"\\srv\test\tmp\test.txt";
        var filePath = @"\\srv\test\tmp";
        var expectedReturnPath = @"\\srv\test\tmp\test.txt";

        var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

        // Act
        var result = fileProcessor.GetFullPathName(fileName);

        // Assert
        Assert.True(result == expectedReturnPath,
            $"Actual: {result}, expected: {expectedReturnPath}");
        mockRepository.VerifyAll();
    }

    [Fact]
    public void GetFullPathName_FileFullNetworkPathMixedPathSeparators_ReturnsFullPath()
    {
        // Arrange
        var fileName = @"\\srv\test/tmp/test.txt";
        var filePath = @"\\srv\test\tmp";
        var expectedReturnPath = @"\\srv\test\tmp\test.txt";

        var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

        // Act
        var result = fileProcessor.GetFullPathName(fileName);

        // Assert
        Assert.True(result == expectedReturnPath,
            $"Actual: {result}, expected: {expectedReturnPath}");
        mockRepository.VerifyAll();
    }

    [Fact]
    public void GetFullPathName_FileRelativeNetworkPath_ReturnsFullPath()
    {
        // Arrange
        var fileName = @"..\test.txt";
        var filePath = @"\\srv\test\tmp";
        var expectedReturnPath = @"\\srv\test\test.txt";

        var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

        // Act
        var result = fileProcessor.GetFullPathName(fileName);

        // Assert
        Assert.True(result == expectedReturnPath,
            $"Actual: {result}, expected: {expectedReturnPath}");
        mockRepository.VerifyAll();
    }
    [Fact]
    public void GetFullPathName_FileRelativeNetworkPath_WithExtendedTypePath_ReturnsFullPath()
    {
        // Arrange
        var fileName = @".\test.txt";
        var filePath = @"Microsoft.PowerShell.Core\FileSystem::\\srv\test\tmp";
        var expectedReturnPath = @"\\srv\test\tmp\test.txt";

        var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

        // Act
        var result = fileProcessor.GetFullPathName(fileName);

        // Assert
        Assert.True(result == expectedReturnPath,
            $"Actual: {result}, expected: {expectedReturnPath}");
        mockRepository.VerifyAll();
    }

    [Fact]
    public void GetFullPathName_FileRelativeNetworkPathMixedPathSeparators_ReturnsFullPath()
    {
        // Arrange
        var fileName = @"../test.txt";
        var filePath = @"\\srv\test\tmp";
        var expectedReturnPath = @"\\srv\test\test.txt";

        var fileProcessor = CreateFileProcessor(filePath, GetMockFileSystem());

        // Act
        var result = fileProcessor.GetFullPathName(fileName);

        // Assert
        Assert.True(result == expectedReturnPath,
            $"Actual: {result}, expected: {expectedReturnPath}");
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
            $"Actual: {result}, expected: {expectedReturnPath}");
        mockRepository.VerifyAll();
    }

    [Fact]
    public void GetFullPathName_FileNameOnly_ReturnsFullPath()
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
            $"Actual: {result}, expected: {expectedReturnPath}");
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
            $"Filename: actual '{result.FileName}', expected '{fileName}',");
        Assert.True(result.ContentDisposition.Disposition == expectedContentDisposition.Disposition,
            $"ContentDispositionType: actual '{result.ContentDisposition.Disposition}', expected '{expectedContentDisposition.Disposition}'");
        Assert.True(actualContent == fileContents,
            $"File contents: actual '{actualContent}', expected '{fileContents}'");
        Assert.True(result.ContentType.MimeType == expectedContentType.MimeType,
            $"ContentType: actual '{result.ContentType.MimeType}', expected '{expectedContentType.MimeType}'");
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
            $"Filename: actual '{result.FileName}', expected '{fileName}'");
        Assert.True(result.ContentId == fileLabel,
            $"Filename: actual '{result.ContentId}', expected '{fileLabel}'");
        Assert.True(result.ContentDisposition.Disposition == expectedContentDisposition.Disposition,
            $"ContentDispositionType: actual '{result.ContentDisposition.Disposition}', expected '{expectedContentDisposition.Disposition}'");
        Assert.True(Enumerable.SequenceEqual(actualContent, fileContents),
            $"File contents: actual '{BitConverter.ToString(actualContent)}', expected '{BitConverter.ToString(fileContents)}'");
        Assert.True(result.ContentType.MimeType == expectedContentType.MimeType,
            $"ContentType: actual '{result.ContentType.MimeType}', expected '{expectedContentType.MimeType}'");
       
        mockRepository.VerifyAll();
    }
}
