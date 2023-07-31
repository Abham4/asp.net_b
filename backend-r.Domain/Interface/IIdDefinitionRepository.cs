namespace backend_r.Domain.Interface
{
    public interface IIdDefinitionRepository : IBaseRepository<IdDefinition>
    {
        Task<List<IdDefinition>> GetByOrganizationId(int id);
        Task<IdDefinition> GetByResourseTypeAndOrganizationId(int id, string resourceType);
    }
}
