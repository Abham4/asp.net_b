namespace backend_r.Domain.Interface
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        bool DoesStaffConnectedToOrganization(int id);
        Task<List<Staff>> UnRegisteredStaffs();
    }
}
