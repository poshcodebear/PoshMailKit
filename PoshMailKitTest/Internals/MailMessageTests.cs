using MimeKit;
using MimeKit.Text;
using Moq;
using PoshMailKit.Internals;
using System;
using System.Collections.Generic;
using Xunit;

namespace PoshMailKitTest.Internals
{
    public class MailMessageTests
    {
        private MockRepository mockRepository;



        public MailMessageTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private MailMessage CreateMailMessage()
        {
            return new MailMessage();
        }

        [Fact]
        public void AddAttachments_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var mailMessage = this.CreateMailMessage();
            List<MimePart> filesToAttach = null;

            // Act
            mailMessage.AddAttachments(
                filesToAttach);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void NewMailBody_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var mailMessage = this.CreateMailMessage();
            TextFormat format = default(global::MimeKit.Text.TextFormat);
            System.Text.Encoding charsetEncoding = null;
            string body = null;
            ContentEncoding contentTransferEncoding = default(global::MimeKit.ContentEncoding);

            // Act
            mailMessage.NewMailBody(
                format,
                charsetEncoding,
                body,
                contentTransferEncoding);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }
    }
}
