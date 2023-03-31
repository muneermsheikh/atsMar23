using System;
using core.Params;

namespace core.Params
{
    public class CandidatesMatchingParams: ParamPages
    {
        public int InterviewId { get; set; }
        public int CategoryId { get; set; }
        public int InterviewItemId {get; set;}
        public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public string Status {get; set;}
        public bool IncludeItems {get; set;}

    }
}