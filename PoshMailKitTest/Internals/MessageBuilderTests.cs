using MimeKit;
using MimeKit.Text;
using Moq;
using PoshMailKit.Internals;
using System;
using System.Collections.Generic;
using Xunit;

namespace PoshMailKitTest.Internals
{
    public class MessageBuilderTests
    {
        private MockRepository mockRepository;

        public MessageBuilderTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private MessageBuilder CreateMessageBuilder()
        {
            return new MessageBuilder();
        }

        [Fact]
        public void AddAttachments_EmptyList_DoesNothing()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            var filesToAttach = new List<MimePart>();

            // Act
            messageBuilder.AddAttachments(filesToAttach);

            // Assert
            var message = messageBuilder.Message;

            Assert.True(message.Body == null,
                $"MimeTypeFormat: actual '{message.Body}', expected '{null}'");

            mockRepository.VerifyAll();
        }

        [Fact] 
        public void AddAttachments_SingleAttachment_SetsMultipartAddsOneFile()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            List<MimePart> filesToAttach = null;

            // Act
            messageBuilder.AddAttachments(filesToAttach);

            // Assert
            Assert.True(false);

            mockRepository.VerifyAll();
        }

        [Fact]
        public void AddAttachments_DoubleAttachment_SetsMultipartAddsTwoFiles()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            List<MimePart> filesToAttach = null;

            // Act
            messageBuilder.AddAttachments(filesToAttach);

            // Assert
            Assert.True(false);

            mockRepository.VerifyAll();
        }

        [Fact]
        public void AddAttachments_RunTwice_SecondRunAddsFileAndDoesntRemoveFirstFile()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            List<MimePart> filesToAttach = null;

            // Act
            messageBuilder.AddAttachments(filesToAttach);

            // Assert
            Assert.True(false);

            mockRepository.VerifyAll();
        }

        [Fact]
        public void NewMailBody_TypicalUsage_AddsAppropriateMailBodyToBuilder()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();

            var format = TextFormat.Plain;
            var charsetEncoding = System.Text.Encoding.ASCII;
            var body = "This is a test";
            var contentTransferEncoding = ContentEncoding.QuotedPrintable;

            var expectedFormat = "text/plain";

            // Act
            messageBuilder.NewMailBody(
                format,
                charsetEncoding,
                body,
                contentTransferEncoding);

            // Assert
            var message = messageBuilder.Message;
            var bodyTextPart = (TextPart)message.Body;

            Assert.True(bodyTextPart.ContentType.MimeType == expectedFormat,
                $"MimeTypeFormat: actual '{bodyTextPart.ContentType.MimeType}', expected '{expectedFormat}'");
            Assert.True(bodyTextPart.ContentType.CharsetEncoding == charsetEncoding,
                $"CharsetEncoding: actual '{bodyTextPart.ContentType.CharsetEncoding}', expected '{charsetEncoding}'");
            Assert.True(message.TextBody == body,
                $"Message body: actual '{message.TextBody}', expected '{body}'");
            Assert.True(bodyTextPart.ContentTransferEncoding == contentTransferEncoding,
                $"ContentTransferEncoding: actual '{bodyTextPart.ContentTransferEncoding}', expected '{contentTransferEncoding}'");

            mockRepository.VerifyAll();
        }
    }
}
