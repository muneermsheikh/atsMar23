using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace core.Entities.Admin
{
    public enum EnumCustomerStatus
    {
        [Display(Name = "Active"), EnumMember(Value="Active")] Active = 100,
        [Display(Name = "Closed"), EnumMember(Value="Closed")] PhoneNotReachable = 200,
        [Display(Name = "Blacklisted"), EnumMember(Value="Blacklisted")] Blacklisted = 300
    }
}