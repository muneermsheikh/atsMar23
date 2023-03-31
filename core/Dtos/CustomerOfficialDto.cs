namespace core.Dtos
{
    public class CustomerOfficialDto
    {
        public CustomerOfficialDto()
        {
        }

          public CustomerOfficialDto(int officialid, int customerId, string customerName, 
            string city, string counry, string title, string officialName, string designation, 
            string officialEmailId, string mobileNo, string aboutCompany, string knownas)
          {
               OfficialId = officialid;
               CustomerId = customerId;
               CustomerName = customerName;
               AboutCompany = aboutCompany;
               CompanyKnownAs = knownas;
               City = city;
               Country = Country;
               Title = title;
               OfficialName = officialName;
               Designation = designation;
               OfficialEmailId = officialEmailId;
               MobileNo = mobileNo;
          }

        public int OfficialId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName {get; set;}
        public string AboutCompany {get; set;}
        public string CompanyKnownAs {get; set;}
        public string City {get; set;}
        public string Country {get; set;}
        public string Title { get; set; }
        public string OfficialName { get; set; }
        public string Designation { get; set; }
        public string OfficialEmailId { get; set; }
        public string MobileNo { get; set; }
        public bool Checked {get; set;}
        public bool CheckedPhone {get; set;}
        public bool CheckedMobile {get; set;}

    }
}