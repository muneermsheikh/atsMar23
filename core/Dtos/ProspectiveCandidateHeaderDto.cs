using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class ProspectiveCandidateHeaderDto
    {
		public ProspectiveCandidateHeaderDto()
		{
		}

		public ProspectiveCandidateHeaderDto(string categoryRef, int? orderItemId, string source, string city, string status, int? statusByUserId, string userName)
		{
			CategoryRef = categoryRef;
			OrderItemId = orderItemId;
			Source = source;
			City = city;
			Status = status;
			StatusByUserId = statusByUserId;
			UserName = userName;
		}

		public string CategoryRef {get; set;}
        public int? OrderItemId {get; set;}
        public string Source { get; set; }
        public DateTime? Date { get; set; }
        public string City {get; set;}
        public string Status { get; set; }
        public int? StatusByUserId {get; set;}
        public string UserName {get; set;}
    }
}