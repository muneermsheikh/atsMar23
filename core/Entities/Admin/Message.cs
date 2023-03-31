using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace core.Entities.Admin
{
     public class essage: BaseEntity
    {
        public int SequenceNo { get; set; }
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        [ForeignKey("SenderId")]
        //public AppUser Sender { get; set; }
        public string RecipientId { get; set; }
        [ForeignKey("RecipientId")]
        //public AppUser Recipient { get; set; }
        public string RecipientUsername { get; set; }
        
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}