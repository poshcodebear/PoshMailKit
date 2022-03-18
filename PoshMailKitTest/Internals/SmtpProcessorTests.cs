using MimeKit;
using MailKit;
using Moq;
using PoshMailKit.Internals;
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

        private SmtpProcessor CreateSmtpProcessor(PMKSmtpClient mockClient)
        {
            return new SmtpProcessor(mockClient);
        }

        [Fact]
        public void SendMailMessage_MissingSmtpServer_DoesNotSend()
        {
            // Arrange
            var mockSmtpClient = new Mock<PMKSmtpClient>();
            var mockMimeMessage = new Mock<MimeMessage>();

            var message = new MimeMessage();
            var smtpProcessor = CreateSmtpProcessor(mockSmtpClient.Object);

            smtpProcessor.Message = message;

            // Act
            smtpProcessor.SendMailMessage();

            // Assert
            mockSmtpClient.Verify(mock =>
                mock.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never());

            mockRepository.VerifyAll();
        }

        [Fact]
        public void SendMailMessage_MissingMessage_DoesNotSend()
        {
            // Arrange
            var mockSmtpClient = new Mock<PMKSmtpClient>();

            var smtpServer = "test.smtp.local";
            var smtpProcessor = CreateSmtpProcessor(mockSmtpClient.Object);

            smtpProcessor.SmtpServer = smtpServer;

            // Act
            smtpProcessor.SendMailMessage();

            // Assert
            mockSmtpClient.Verify(mock =>
                mock.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never());

            mockRepository.VerifyAll();
        }

        [Fact]
        public void SendMailMessage_HasServerAndMessage_Sends()
        {
            // Arrange
            var mockSmtpClient = new Mock<PMKSmtpClient>();
            var mockMimeMessage = new Mock<MimeMessage>();

            var message = new MimeMessage();
            var smtpServer = "test.smtp.local";
            var smtpProcessor = CreateSmtpProcessor(mockSmtpClient.Object);

            smtpProcessor.Message = message;
            smtpProcessor.SmtpServer = smtpServer;

            // Act
            smtpProcessor.SendMailMessage();

            // Assert
            mockSmtpClient.Verify(mock => 
                mock.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()));
            
            mockRepository.VerifyAll();
        }
    }
}
