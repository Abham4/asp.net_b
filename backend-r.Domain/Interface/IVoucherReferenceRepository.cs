namespace backend_r.Domain.Interface
{
    public interface IVoucherReferenceRepository : IBaseRepository<VoucherReference> 
    {
        Task OrganizationalClose(int closingPeriodId, int organizationId);
        Task MemberClose(int closingPeriodId, int memberId);
        Task<List<VoucherReference>> GetVoucherReferencesBySummaryVoucher(int summaryVoucherId);
    }
}