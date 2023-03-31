using System;
using core.Entities.AccountsNFinance;

namespace core.Entities.Attachments
{
	public class VoucherAttachment: BaseEntity
    {
	public VoucherAttachment()
	{
	}

	public VoucherAttachment(int voucherId, int attachmentSizeInBytes, string fileName, 
		string url, DateTime dateUploaded, int uploadedByEmployeeId)
	{
		FinanceVoucherId = voucherId;
		AttachmentSizeInBytes = attachmentSizeInBytes;
		FileName = fileName;
		Url = url;
		DateUploaded = dateUploaded;
		UploadedByEmployeeId = uploadedByEmployeeId;
	}

	public int FinanceVoucherId { get; set; }
	public int AttachmentSizeInBytes { get; set; }
	public string FileName { get; set; }
	public string Url { get; set; }
	public DateTime DateUploaded { get; set; }
	public int UploadedByEmployeeId { get; set; }
	public FinanceVoucher FinanceVoucher {get; set;}

    }
}