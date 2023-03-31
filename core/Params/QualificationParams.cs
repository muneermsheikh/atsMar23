using System;
using core.Params;

namespace core.Dtos
{
    public class QualificationParams: ParamPages
    {
        public int QualificationId { get; set; }
        public string QualificationNameLike {get; set;}
        
    }
}