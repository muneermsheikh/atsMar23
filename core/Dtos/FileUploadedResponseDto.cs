using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class FileUploadedResponseDto
    {
        public string ErrorMessage { get; set; }
        public List<FileUploadResponseData> Data { get; set; }
    }
}