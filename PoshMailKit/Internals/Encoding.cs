namespace PoshMailKit.Internals;

public enum Encoding
{
    ASCII,
    BigEndianUnicode,
    BigEndianUTF32,
    //OEM, // Not supporting this for now
    Unicode,
    UTF7,
    UTF8,
    UTF8BOM,
    UTF8NoBOM,
    UTF32
}
