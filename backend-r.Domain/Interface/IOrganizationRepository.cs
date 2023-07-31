namespace backend_r.Domain.Interface
{
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        Task<List<Organization>> ListofMembers();
        Task<List<Organization>> ListofMembersOccupations();
    }
}