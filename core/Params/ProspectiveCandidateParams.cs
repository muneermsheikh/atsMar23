using System;
using core.Entities.Admin;
using core.Params;

namespace core.Dtos
{
    public class ProspectiveCandidateParams: ParamPages
    {
        public int? Id {get; set;}
        public int? HeaderId {get; set;}
        public string CategoryRef {get; set;}
        public DateTime DateAdded { get; set; }
        public string CandidateNameLike {get; set;}
        public string Status {get; set;}
        public bool Closed {get; set;}
    }
}