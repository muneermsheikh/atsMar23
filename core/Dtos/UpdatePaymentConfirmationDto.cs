using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class UpdatePaymentConfirmationDto
    {
        public int VoucherEntryId { get; set; }
        public bool DrEntryApproved { get; set; }
        public DateTime DrEntryReviewedOn { get; set; }
    }
}