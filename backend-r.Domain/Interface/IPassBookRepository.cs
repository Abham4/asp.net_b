namespace backend_r.Domain.Interface
{
    public interface IPassBookRepository : IBaseRepository<PassBook>
    {
        bool PassBookExist(string passBook);
    }
}