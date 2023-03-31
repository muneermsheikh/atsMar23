using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class ComposeMessageDtoForProspects
    {
        public ComposeMessageDtoForProspects()
        {
        }

        public ComposeMessageDtoForProspects(int id, string candidateName, string email, string phone, string source, string city, string country)
        {
            Id = id;
            CandidateName = candidateName;
            Email = email;
            PhoneNo = phone;
            Source = source;
            City = city;
            Country = country;
            MessageComposedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Source { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CategoryName { get; set; }
        public string CategoryRef { get; set; }
        public DateTime? MessageComposedOn {get; set;}
    }
}