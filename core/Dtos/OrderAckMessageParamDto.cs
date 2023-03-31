using core.Entities.Orders;
using core.Entities.Users;

namespace core.Dtos
{
    public class OrderMessageParamDto
    {
        public Order Order { get; set; }
        public bool DirectlySendMessage { get; set; }=false;
    }
}