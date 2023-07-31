namespace core.Entities.Admin
{
    public class VendorFacility: BaseEntity
    {
        public VendorFacility()
        {
        }

        public VendorFacility(string facilityName)
        {
            Name = facilityName;
        }

        public string Name { get; set; }
    }
}