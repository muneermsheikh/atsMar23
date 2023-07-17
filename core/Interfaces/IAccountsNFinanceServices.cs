using core.Dtos;
using core.Dtos.fiance;
using core.Entities.AccountsNFinance;
using core.Entities.Attachments;
using core.Params;

namespace core.Interfaces
{
     public interface IAccountsNFinanceServices
    {
        Task<Pagination<Coa>> GetCOAs(CoaParams coaParams);
        Task<ICollection<Coa>> GetCOAsForAccountGroup(string accountgroup);
        Task<ICollection<Coa>>GetCoasForPayments(int applicationno);
        Task<CoaDto> GetOrCreateCoaForCandidate(int applicationno, bool create);
        Task<ICollection<PendingDebitApprovalDto>> GetPendingDebitApprovals();
        Task<bool>UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> updateDto);
        Task<ICollection<Coa>> GetCOAList();
        Task<Coa> AddNewCOA(COAToAddDto coa);
        Task<Coa> EditCOA(Coa coa);
        Task<bool> DeleteCOA(int id);
        Task<Pagination<FinanceVoucher>> GetFinanceVouchers(TransactionParams transactionParams);
        Task<FinanceVoucher> GetFinanceVoucher(int id);
        Task<FinanceVoucher> AddNewVoucher(VoucherToAddDto dto, int loggedInEmployeeId);
        Task<VoucherWithNewAttachmentDto> UpdateFinanceVoucherWithFileUploads(FinanceVoucher financeVoucher);
        Task<bool> DeleteFinanceVoucher(int id);
        Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateTime fromDate, DateTime uptoDate);
        Task<long> GetClosingBalIncludingSuspense(int accountid);
        Task<ICollection<string>> GetMatchingCOANames(string testName);
        Task<string> AddVoucherAttachments(ICollection<VoucherAttachment> voucherattachments);
    }
}