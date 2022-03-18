using MimeKit;
using MailKit;
using Moq;
using PoshMailKit.Internals;
using System;
using Xunit;
using System.Threading;

namespace PoshMailKitTest.Internals
{
    public class SmtpProcessorTests
    {
        private MockRepository mockRepository;

        public SmtpProcessorTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private SmtpProcessor CreateSmtpProcessor()
        {
            return new SmtpProcessor();
        }

        [Fact]
        public void SendMailMessage_MissingSmtpServer_DoesNotSend()
        {
            // Arrange
            var sendMethodWasCalled = false;
            var mockSmptClient = new Mock<PMKSmtpClient>();
            mockSmptClient.Setup(c => c.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>())).
                Callback(() => sendMethodWasCalled = true);

            var mockMimeMessage = new Mock<MimeMessage>();
            var message = new MimeMessage();

            var smtpProcessor = CreateSmtpProcessor();

            smtpProcessor.Message = message;

            // Act
            smtpProcessor.SendMailMessage();

            // Assert
            Assert.False(sendMethodWasCalled);

            mockRepository.VerifyAll();
        }
    }
}
