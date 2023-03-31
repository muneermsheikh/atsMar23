namespace core.Entities.Admin
{
    public class AgencySpecialty
    {
		public AgencySpecialty()
		{
		}

		public AgencySpecialty(int id, int customerId, int professionId, int industryId, string name)
		{
			Id = id;
			CustomerId = customerId;
			ProfessionId = professionId;
			IndustryId = industryId;
			Name = name;
		}

		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int ProfessionId { get; set; }
		public int IndustryId { get; set; }
		public string Name { get; set; }
        	//public Customer Customer { get; set; }
    }
}