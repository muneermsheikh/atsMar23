
namespace core.Dtos.Admin
{
    public class ForwardedCategoryDto
    {
        public int Id {get; set;}
        //public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public DateTime OrderDate { get; set; }
		public int OrderItemId {get; set;}
		//public int ForwardedToAgentId { get; set;}
        //public string AgentName {get; set;}
		//public int CategoryId {get; set;}
		public string CategoryRefAndName {get; set;}
		public int Charges {get; set;}
		public ICollection<ForwardedOfficialDto> ForwardedOfficials {get; set;}
 
    }
}