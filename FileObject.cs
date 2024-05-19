using System;

namespace FTPClient
{
    internal class FileObject
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime DateModified { get; set; }
    }
}