namespace backend_r.Domain.Interface
{
    public interface IVoucherRepository : IBaseRepository<Voucher>
    {
        Task<List<Voucher>> Add(int organizationId, int purchasedProductId, double amount, string remark);
        Task<List<Voucher>> Withdraw(int organizationId, int purchasedProductId, double amount, string remark);
        Task<Voucher> GetVoucherSummaryByIdForOrganization(int id);
        Task<Voucher> GetVoucherSummaryByIdForMember(int id);
        Task<List<Voucher>> GetVoucherByOrganizationIdAndClosingPeriodId(int closingPeriodId, int organizationId);
        Task<List<Voucher>> GetVoucherByMemberCodeAndClosingPeriodId(int closingPeriodId, int memberId);
        Task<List<double>> Summary();
        Task<List<List<double>>> VouchersByVoucherTypeforHighers(string organizationName);
    }
}