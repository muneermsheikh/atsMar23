using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Users
{
    public class UserAttachment: BaseEntity
    {
        public UserAttachment()
        {
        }

        public UserAttachment(string appUserId, int candidateid, string fileName, string attachmentType, long attachmentSizeInBytes, 
            string attachmentUrl, int uploadedByEmployeeId, DateTime dtUploaded)
        {
            CandidateId = candidateid;
            AppUserId = appUserId;
            AttachmentType = attachmentType;
            AttachmentSizeInBytes = attachmentSizeInBytes;
            url = attachmentUrl;
            UploadedByEmployeeId = uploadedByEmployeeId;
            DateUploaded = dtUploaded;
            FileName = fileName;
        }

        public int CandidateId { get; set; }
        public string AppUserId { get; set; }
        public string AttachmentType { get; set; }      //cv, photo, educertificate, expcertificate, pp
        public long AttachmentSizeInBytes { get; set; }
        //public Candidate Candidate {get; set;}
        public string FileName {get; set;}
        public string url  { get; set; }
        public DateTime DateUploaded {get; set;}
        public int UploadedByEmployeeId {get; set;} 
        
    }
}