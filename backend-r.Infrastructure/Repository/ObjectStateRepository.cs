namespace backend_r.Infrastructure.Repository
{
    public class ObjectStateRepository : BaseRepository<ObjectState>, IObjectStateRepository
    {
        private readonly JoshuaContext _context;
        private readonly IUserRepository _userRepo;
        public ObjectStateRepository(JoshuaContext context, IUserRepository userRepository) : base(context)
        {
            _context = context;
            _userRepo = userRepository;
        }

        public async Task<string> ToActive(int memberId)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var member = await _context.Members.SingleOrDefaultAsync(c => c.Id == memberId);

            if (member.LastObjectState != ObjectStateEnumeration.Created.Name)
            {
                throw new DomainException("The member last state must be created");
            }

            var objectStateDef = await _context.ObjectStateDefns.SingleOrDefaultAsync(c => c.Resource.Type == "Member" &&
                c.Name == ObjectStateEnumeration.Active.Name);
            var resource = await _context.Resources.SingleOrDefaultAsync(c => c.Type == "Member");

            var objectState = new ObjectState(objectStateDef.Id, resource.Id, objectStateDef.Name, DateTime.Now, member.Id,
                user.Email);

            await _context.ObjectStates.AddAsync(objectState);
            await UnitOfWork.SaveChanges();

            member.LastObjectState = objectStateDef.Name;

            _context.Members.Update(member);
            await UnitOfWork.SaveChanges();

            return "State Changed";
        }

        public async Task<string> ToTerminate(int memberId)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var member = await _context.Members.SingleOrDefaultAsync(c => c.Id == memberId);

            if (member.LastObjectState != ObjectStateEnumeration.Created.Name)
            {
                throw new DomainException("The member last state must be created");
            }

            var objectStateDef = await _context.ObjectStateDefns.SingleOrDefaultAsync(c => c.Resource.Type == "Member" && c.Name ==
                ObjectStateEnumeration.Terminated.Name);
            var resource = await _context.Resources.SingleOrDefaultAsync(c => c.Type == "Member");

            var objectState = new ObjectState(objectStateDef.Id, resource.Id, objectStateDef.Name, DateTime.Now,
                member.Id, user.Email);

            await _context.ObjectStates.AddAsync(objectState);
            await UnitOfWork.SaveChanges();

            member.LastObjectState = objectStateDef.Name;

            _context.Members.Update(member);
            await UnitOfWork.SaveChanges();

            return "State Changed";
        }
    }
}
