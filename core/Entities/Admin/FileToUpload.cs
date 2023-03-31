using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class FileToUpload
    {
        public string FileName { get; set; }
        public int FileSize  { get; set; }
        public string FileType  { get; set; }
        public long LastModifiedTime  { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string FileAsBase64 { get; set; }
        public byte[] FileAsByteArray { get; set; }
    }
}