namespace backend_r.Domain.Interface
{
    public interface IPurchasedProductRepository : IBaseRepository<PurchasedProduct>
    {
        Task<List<PurchasedProduct>> GetPurchasedProductsByAccountId(int id);
        Task Scheduler(AccountProductType type, int paymentCount, int scheduleHeaderId, DateTime startingDate, 
            double principalDue, double interest, double remain, float payCycle, double due, double rate);
        int NoofPrchasedShare(int accountId, int productId);
        Task<double> TotalPurchasedAmount(int accountId);
    }
}