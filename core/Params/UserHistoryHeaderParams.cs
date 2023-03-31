using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Params
{
	public class UserHistoryHeaderParams : ParamPages
	{
		public UserHistoryHeaderParams()
		{
		}

        public int? Id { get; set; }
        public string CategoryRefCode { get; set; }
        public string CategoryRefName { get; set; }
        public string CustomerName { get; set; }
        public int? AssignedToId { get; set; }
        public int? AssignedById { get; set; }
        public string Status { get; set; }
        public bool? Concluded {get; set;}
	}
}