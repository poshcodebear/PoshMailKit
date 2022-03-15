using MimeKit;
using System.IO;
using System.Text.RegularExpressions;

namespace PoshMailKit.Internals
{
    internal class FileProcessor
    {
        internal string WorkingDirectory { get; set; }
        private readonly Regex pathPattern = new Regex(@"^[a-zA-Z]:");

        internal FileProcessor(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        internal string GetFullPathName(string fileName)
        {
            if (!pathPattern.IsMatch(fileName))
                return Path.GetFullPath($"{WorkingDirectory}\\{fileName}");
            else
                return fileName;
        }

        internal MimePart GetFileMimePart(string fileName)
        {
            return GetFileMimePart(fileName, new ContentDisposition(ContentDisposition.Attachment));
        }

        internal MimePart GetFileMimePart(string fileName, ContentDisposition contentDisposition, string lable = null)
        {
            Stream fileStream = File.OpenRead(GetFullPathName(fileName));

            ContentType contentType = MimeMap.GetMimeMap(Path.GetExtension(fileName));

            MimePart mimePart = new MimePart(contentType)
            {
                Content = new MimeContent(fileStream),
                ContentDisposition = contentDisposition,
                FileName = Path.GetFileName(fileName),
            };

            if (lable != null)
                mimePart.ContentId = lable;

            return mimePart;
        }
    }
}
