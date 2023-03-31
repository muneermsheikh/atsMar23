using System;
using System.Collections.Generic;
using core.Entities;
using core.Entities.Admin;

namespace core.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string KnownAs { get; set; }
        public string CustomerType { get; set; }
        public string Add { get; set; }
        public string Add2 { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string District {get; set;}
        public string State { get; set; }
        public string Country { get; set; }
        public string IntroducedBy { get; set; }
        public string Email {get; set;}
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Website { get; set; }
        public string Introduction { get; set; }
        public DateTime DateCreated {get; set;}
        public EnumCustomerStatus CustomerStatus {get; set;}
        public bool IsActive { get; set; }
        public ICollection<CustomerIndustry> CustomerIndustries { get; set; }
        public ICollection<AgencySpecialty> AgencySpecialties {get; set;}
        public ICollection<CustomerOfficial> CustomerOfficials {get; set;}
    }
}