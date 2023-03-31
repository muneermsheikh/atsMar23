using System;

namespace core.Params
{
    public class DeployParams: ParamPages
    {
        public int CVRefId { get; set; }
        public int CandidateId {get; set;}
        public int ApplicationNo {get; set;}
        public string CandidateName {get; set;}
        public DateTime SelectedOn {get; set;}
        public string DeployStageName {get; set;}
        public DateTime TransactionDate {get; set;}
        public string CategoryName {get; set;}
        public string CustomerName {get; set;}
        public int OrderNo {get; set;}
        public int CustomerId { get; set; }
        public string SearchOn {get; set;}
    }
}