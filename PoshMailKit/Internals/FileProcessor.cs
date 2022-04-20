using MimeKit;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace PoshMailKit.Internals;

public class FileProcessor
{
    public string WorkingDirectory { get; set; }
    private readonly IFileSystem FileSystem;
    private readonly Regex pathPattern = new(@"^([a-zA-Z]:|//|\\\\)");

        public FileProcessor(string workingDirectory, IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            WorkingDirectory = CleanWorkingDirectory(workingDirectory);
        }

    public FileProcessor(string workingDirectory)
        : this(workingDirectory, fileSystem: new FileSystem())
    { }

    public string GetFullPathName(string fileName)
    {
        if (!pathPattern.IsMatch(fileName))
            return Path.GetFullPath($"{WorkingDirectory}\\{fileName}");
        else
            return Path.GetFullPath(fileName);
    }

    public MimePart GetFileMimePart(string fileName, ContentDispositionType contentDisposition, string label = null)
    {
        Stream fileStream = FileSystem.File.OpenRead(GetFullPathName(fileName));

        ContentType contentType = MimeMap.GetContentType(Path.GetExtension(fileName));

        MimePart mimePart = new(contentType)
        {
            Content = new MimeContent(fileStream),
            ContentDisposition = new ContentDisposition(contentDisposition.ToString()),
            FileName = Path.GetFileName(fileName),
        };

        if (label is not null)
            mimePart.ContentId = label;

        return mimePart;
    }

    private string CleanWorkingDirectory(string workingDirectory)
    {
        return Regex.Replace(workingDirectory, "^.*::", "");
    }
}
public enum ContentDispositionType { Attachment, Inline }
