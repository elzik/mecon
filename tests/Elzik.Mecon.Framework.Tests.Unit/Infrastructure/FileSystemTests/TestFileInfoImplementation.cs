using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests;

public class TestFileInfoImplementation : IFileInfo
{
    public TestFileInfoImplementation(string fullName, string name, long length)
    {
        FullName = fullName;
        Name = name;
        Length = length;
        Exists = false;
        Extension = null;
        LinkTarget = null;
        Directory = null;
        DirectoryName = null;
        FileSystem = null;
    }

    public IFileSystem FileSystem { get; }
    public FileAttributes Attributes { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public bool Exists { get; }
    public string Extension { get; }
    public string FullName { get; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public string LinkTarget { get; }
    public string Name { get; }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public StreamWriter AppendText()
    {
        throw new NotImplementedException();
    }

    public IFileInfo CopyTo(string destFileName)
    {
        throw new NotImplementedException();
    }

    public IFileInfo CopyTo(string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public Stream Create()
    {
        throw new NotImplementedException();
    }

    public StreamWriter CreateText()
    {
        throw new NotImplementedException();
    }

    public void Decrypt()
    {
        throw new NotImplementedException();
    }

    public void Encrypt()
    {
        throw new NotImplementedException();
    }

    public FileSecurity GetAccessControl()
    {
        throw new NotImplementedException();
    }

    public FileSecurity GetAccessControl(AccessControlSections includeSections)
    {
        throw new NotImplementedException();
    }

    public void MoveTo(string destFileName)
    {
        throw new NotImplementedException();
    }

    public void MoveTo(string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode, FileAccess access)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode, FileAccess access, FileShare share)
    {
        throw new NotImplementedException();
    }

    public Stream OpenRead()
    {
        throw new NotImplementedException();
    }

    public StreamReader OpenText()
    {
        throw new NotImplementedException();
    }

    public Stream OpenWrite()
    {
        throw new NotImplementedException();
    }

    public IFileInfo Replace(string destinationFileName, string destinationBackupFileName)
    {
        throw new NotImplementedException();
    }

    public IFileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
    {
        throw new NotImplementedException();
    }

    public void SetAccessControl(FileSecurity fileSecurity)
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo Directory { get; }
    public string DirectoryName { get; }
    public bool IsReadOnly { get; set; }
    public long Length { get; }
}