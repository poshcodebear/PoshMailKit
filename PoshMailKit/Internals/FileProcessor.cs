﻿using MimeKit;
using System.IO;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace PoshMailKit.Internals
{
    public class FileProcessor
    {
        public string WorkingDirectory { get; set; }
        private readonly IFileSystem FileSystem;
        private readonly Regex pathPattern = new Regex(@"^[a-zA-Z]:");

        public FileProcessor(string workingDirectory, IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            WorkingDirectory = workingDirectory;
        }

        public FileProcessor(string workingDirectory)
            : this(workingDirectory, fileSystem: new FileSystem())
        { }

        public string GetFullPathName(string fileName)
        {
            if (!pathPattern.IsMatch(fileName))
                return Path.GetFullPath($"{WorkingDirectory}\\{fileName}");
            else
                return fileName;
        }

        public MimePart GetFileMimePart(string fileName)
        {
            return GetFileMimePart(fileName, new ContentDisposition(ContentDisposition.Attachment));
        }

        public MimePart GetFileMimePart(string fileName, ContentDisposition contentDisposition, string lable = null)
        {
            Stream fileStream = FileSystem.File.OpenRead(GetFullPathName(fileName));

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
