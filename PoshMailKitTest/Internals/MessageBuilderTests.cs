using MimeKit;
using MimeKit.Text;
using Moq;
using PoshMailKit.Internals;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using System;

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
            var contentType = "text/plain";
            var fileName = "test.txt";
            var contentDisposition = "attachment";
            var expectedBodyContentType = "multipart/mixed";

            var messageBuilder = CreateMessageBuilder();
            var filesToAttach = new List<MimePart>
            {
                { new MimePart(contentType)
                {
                    FileName = fileName,
                    ContentDisposition = new ContentDisposition(contentDisposition),
                } },
            };

            // Act
            messageBuilder.AddAttachments(filesToAttach);

            // Assert
            var message = messageBuilder.Message;
            var attachment = message.BodyParts.ElementAt(0);

            Assert.True(message.BodyParts.Count() == filesToAttach.Count,
                $"Number of Body Parts: actual '{message.BodyParts.Count()}', expected '{filesToAttach.Count}'");
            Assert.True(message.Body.ContentType.MimeType == expectedBodyContentType,
                $"Body MimeType: actual '{message.Body.ContentType.MimeType}', expected '{expectedBodyContentType}'");
            Assert.True(attachment.ContentType.MimeType == contentType,
                $"Attachment MimeType: actual '{attachment.ContentType.MimeType}', expected '{contentType}'");
            Assert.True(attachment.ContentType.Name == fileName,
                $"FileName: actual '{attachment.ContentType.Name}', expected '{fileName}'");
            Assert.True(attachment.ContentDisposition.Disposition == contentDisposition,
                $"ContentDisposition: actual '{attachment.ContentDisposition.Disposition}', expected '{contentDisposition}'");

            mockRepository.VerifyAll();
        }

        [Fact]
        public void AddAttachments_DoubleAttachment_SetsMultipartAddsTwoFiles()
        {
            // Arrange
            var contentType1 = "text/plain";
            var fileName1 = "test.txt";
            var contentDisposition1 = "attachment";
            var contentType2 = "image/png";
            var fileName2 = "test.png";
            var contentDisposition2 = "inline";
            var expectedBodyContentType = "multipart/mixed";

            var messageBuilder = CreateMessageBuilder();
            var filesToAttach = new List<MimePart>
            {
                { new MimePart(contentType1)
                {
                    FileName = fileName1,
                    ContentDisposition = new ContentDisposition(contentDisposition1),
                } },
                { new MimePart(contentType2)
                {
                    FileName = fileName2,
                    ContentDisposition = new ContentDisposition(contentDisposition2),
                } },
            };

            // Act
            messageBuilder.AddAttachments(filesToAttach);

            // Assert
            var message = messageBuilder.Message;
            var attachment1 = message.BodyParts.ElementAt(0);
            var attachment2 = message.BodyParts.ElementAt(1);

            Assert.True(message.BodyParts.Count() == filesToAttach.Count,
                $"Number of Body Parts: actual '{message.BodyParts.Count()}', expected '{filesToAttach.Count}'");
            Assert.True(message.Body.ContentType.MimeType == expectedBodyContentType,
                $"Body MimeType: actual '{message.Body.ContentType.MimeType}', expected '{expectedBodyContentType}'");

            Assert.True(attachment1.ContentType.MimeType == contentType1,
                $"Attachment 1 MimeType: actual '{attachment1.ContentType.MimeType}', expected '{contentType1}'");
            Assert.True(attachment1.ContentType.Name == fileName1,
                $"FileName 1: actual '{attachment1.ContentType.Name}', expected '{fileName1}'");
            Assert.True(attachment1.ContentDisposition.Disposition == contentDisposition1,
                $"ContentDisposition 1: actual '{attachment1.ContentDisposition.Disposition}', expected '{contentDisposition1}'");

            Assert.True(attachment2.ContentType.MimeType == contentType2,
                $"Attachment 2 MimeType: actual '{attachment2.ContentType.MimeType}', expected '{contentType2}'");
            Assert.True(attachment2.ContentType.Name == fileName2,
                $"FileName 1: actual '{attachment2.ContentType.Name}', expected '{fileName2}'");
            Assert.True(attachment2.ContentDisposition.Disposition == contentDisposition2,
                $"ContentDisposition 1: actual '{attachment2.ContentDisposition.Disposition}', expected '{contentDisposition2}'");

            mockRepository.VerifyAll();
        }

        [Fact]
        public void AddAttachments_RunTwice_SecondRunAddsFileAndDoesntRemoveFirstFile()
        {
            // Arrange
            var contentType1 = "text/plain";
            var fileName1 = "test.txt";
            var contentDisposition1 = "attachment";
            var contentType2 = "image/png";
            var fileName2 = "test.png";
            var contentDisposition2 = "inline";
            var expectedBodyContentType = "multipart/mixed";

            var messageBuilder = CreateMessageBuilder();
            var filesToAttach1 = new List<MimePart>
            {
                { new MimePart(contentType1)
                {
                    FileName = fileName1,
                    ContentDisposition = new ContentDisposition(contentDisposition1),
                } },
            };
            var filesToAttach2 = new List<MimePart>
            {
                { new MimePart(contentType2)
                {
                    FileName = fileName2,
                    ContentDisposition = new ContentDisposition(contentDisposition2),
                } },
            };

            // Act
            messageBuilder.AddAttachments(filesToAttach1);
            messageBuilder.AddAttachments(filesToAttach2);

            // Assert
            var message = messageBuilder.Message;
            var attachment1 = message.BodyParts.ElementAt(0);
            var attachment2 = message.BodyParts.ElementAt(1);

            Assert.True(message.BodyParts.Count() == filesToAttach1.Count + filesToAttach2.Count,
                $"Number of Body Parts: actual '{message.BodyParts.Count()}', expected '{filesToAttach1.Count + filesToAttach2.Count}'");
            Assert.True(message.Body.ContentType.MimeType == expectedBodyContentType,
                $"Body MimeType: actual '{message.Body.ContentType.MimeType}', expected '{expectedBodyContentType}'");

            Assert.True(attachment1.ContentType.MimeType == contentType1,
                $"Attachment 1 MimeType: actual '{attachment1.ContentType.MimeType}', expected '{contentType1}'");
            Assert.True(attachment1.ContentType.Name == fileName1,
                $"FileName 1: actual '{attachment1.ContentType.Name}', expected '{fileName1}'");
            Assert.True(attachment1.ContentDisposition.Disposition == contentDisposition1,
                $"ContentDisposition 1: actual '{attachment1.ContentDisposition.Disposition}', expected '{contentDisposition1}'");

            Assert.True(attachment2.ContentType.MimeType == contentType2,
                $"Attachment 2 MimeType: actual '{attachment2.ContentType.MimeType}', expected '{contentType2}'");
            Assert.True(attachment2.ContentType.Name == fileName2,
                $"FileName 1: actual '{attachment2.ContentType.Name}', expected '{fileName2}'");
            Assert.True(attachment2.ContentDisposition.Disposition == contentDisposition2,
                $"ContentDisposition 1: actual '{attachment2.ContentDisposition.Disposition}', expected '{contentDisposition2}'");

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

        [Fact]
        public void GetMailboxAddressObj_ValidEmailAddressOnly_ReturnsSimpleEmailAddress()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            var emailAddress = "test@email.com";

            // Act
            var mailboxAddressObj = messageBuilder.GetMailboxAddressObj(emailAddress);

            // Assert
            Assert.Equal(mailboxAddressObj.Address, emailAddress);

            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetMailboxAddressObj_InvalidEmailAddress_ThrowsFormatExceptionError()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            var emailAddress = "test?email.com";

            // Act & Assert
            try
            {
                var _ = messageBuilder.GetMailboxAddressObj(emailAddress);
                Assert.False(true,
                    $"Exception expected but not thrown");
            }
            catch(Exception e)
            {
                Assert.True(e is FormatException,
                    $"FormatException expected, actual exception thrown: {e.GetType()}");
            }

            mockRepository.VerifyAll();
        }

        [Fact]
        public void GetMailboxAddressObj_ValidEmailAddressWithDisplayName_ReturnsCompoundEmailAddress()
        {
            // Arrange
            var messageBuilder = CreateMessageBuilder();
            var emailAddress = "test@email.com";
            var displayName = "Testy McTesterson";

            var compoundAddress = $"{displayName} <{emailAddress}>";

            // Act
            var mailboxAddressObj = messageBuilder.GetMailboxAddressObj(compoundAddress);

            // Assert
            Assert.Equal(mailboxAddressObj.Address, emailAddress);
            Assert.Equal(mailboxAddressObj.Name, displayName);

            mockRepository.VerifyAll();
        }
    }
}
