namespace backend_r.Infrastructure.Repository
{
    public class AddressRepository : BaseRepository<Address> , IAddressRepository
    {
        private readonly JoshuaContext _context;
        public AddressRepository(JoshuaContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetByReference(int id)
        {
            return await _context.Addresses.Where(c => c.Reference == id).ToListAsync();
        }

        public async Task<List<Address>> GetByReferenceAndOwnerType(int reference, string ownerType)
        {
            return await _context.Addresses.Where(c => c.Reference == reference && c.OwnerType == ownerType).ToListAsync();
        }
    }
}
