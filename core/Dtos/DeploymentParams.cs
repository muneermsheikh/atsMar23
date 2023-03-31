using System;
using core.Params;

namespace core.Dtos
{
    public class DeploymentParams: ParamPages
    {
        public int OrderDetailId { get; set; }
        public int OrderId {get; set;}
    }
}