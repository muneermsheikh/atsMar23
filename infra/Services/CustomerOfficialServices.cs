using core.Entities.Admin;
using core.Interfaces;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class CustomerOfficialServices : ICustomerOfficialServices
	{
        private readonly ATSContext _context;
		public CustomerOfficialServices(ATSContext context)
		{
            _context = context;
		}

		public async Task<CustomerOfficialDto> GetCustomerOfficialDetail(int CustomerOfficialId)
		{
            var off = await (from c in _context.Customers
                    .Where(x => x.CustomerStatus==EnumCustomerStatus.Active )
                    join o in _context.CustomerOfficials on c.Id equals o.CustomerId where o.IsValid && o.Id == CustomerOfficialId
                    select new CustomerOfficialDto(o.Id, c.Id,  c.CustomerName, c.City, c.Country, o.Title, 
                    o.OfficialName, o.Designation, o.Email, o.Mobile, c.Introduction, c.KnownAs)
                    ).FirstOrDefaultAsync();
               
               return off;			
		}

	}
}