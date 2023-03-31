namespace core.Entities
{
    public class CustomerIndustry: BaseEntity
    {
		public CustomerIndustry()
		{
		}

		public CustomerIndustry(int customerId, int industryId, string name)
		{
			CustomerId = customerId;
			IndustryId = industryId;
			Name = name;
		}

		public int CustomerId { get; set; }
        public int IndustryId { get; set; }
        public string Name {get; set;}
        //public Customer Customer {get; set;}
    }
}