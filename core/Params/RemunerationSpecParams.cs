using System;
using core.Entities.HR;
using core.Dtos;

namespace core.Params
{
    public class RemunerationSpecParams : ParamPages
    {
        public int? OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? OrderNo { get; set; }
        public int? CategoryId {get; set;}

    }
}