using System;
using core.Params;

namespace core.Params
{
    public class CategorySpecParams: ParamPages
    {
        public int? Id { get; set; }
        public string Name {get; set;}
        //public int? IndustryId {get; set;}
    }
}