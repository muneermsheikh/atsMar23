using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Entities.Admin
{
    public class VendorSpecialty: BaseEntity
    {
        public VendorSpecialty(int customerId, int vendorFacilityId, string name)
        {
            CustomerId = customerId;
            VendorFacilityId = vendorFacilityId;
            Name = name;
        }

        public int CustomerId { get; set; }
        public int VendorFacilityId { get; set; }
        public string Name { get; set; }
    }
}