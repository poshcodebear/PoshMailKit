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
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private MessageBuilder CreateMessageBuilder()
        {
            return new MessageBuilder();
        }

        [Fact]
        public void AddAttachments_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var mailMessage = this.CreateMessageBuilder();
            List<MimePart> filesToAttach = null;

            // Act
            mailMessage.AddAttachments(
                filesToAttach);

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
            MimeMessage message = messageBuilder.Message;
            TextPart bodyTextPart = (TextPart)message.Body;

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
