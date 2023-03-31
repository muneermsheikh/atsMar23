using System.Collections.Generic;
using core.Entities.AccountsNFinance;
using core.Entities.Attachments;

namespace core.Dtos
{
	public class VoucherWithNewAttachmentDto
    {
        public FinanceVoucher FinanceVoucher { get; set; }
        public ICollection<VoucherAttachment> NewAttachments { get; set; }
    }
}