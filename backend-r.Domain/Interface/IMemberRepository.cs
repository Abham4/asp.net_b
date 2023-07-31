namespace backend_r.Domain.Interface
{
    public interface IMemberRepository : IBaseRepository<Member>
    {
        public Task<List<Member>> GetMembersByState(string state);
        public Task<string> SavePicture(IFormFile xFile, string holderType);
        public Task<string> SaveSignature(IFormFile xFile);
        public Task<string> SaveAttachment(IFormFile xFile);
        public void DeleteFile(string fileName);
        public Task<List<Member>> GetUnRegisteredMembers();
        public Task<List<Member>> GetMembersByYear(int year);
        public Task<List<int>> GetMemberCarrers();
    }
}
