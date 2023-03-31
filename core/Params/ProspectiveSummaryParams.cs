using System;
using core.Entities.Admin;

namespace core.Params
{
    public class ProspectiveSummaryParams: ParamPages
    {
        public string CategoryRef {get; set;}
        public DateTime Dated { get; set; }
        public string Status {get; set;}
    }
}