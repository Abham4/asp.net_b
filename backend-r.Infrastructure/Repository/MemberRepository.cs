namespace backend_r.Infrastructure.Repository
{
    public class MemberRepository : BaseRepository<Member>, IMemberRepository
    {
        private readonly JoshuaContext _context;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;

        public MemberRepository(JoshuaContext context, IWebHostEnvironment hostEnvironment, IUserRepository userRepository,
            IRoleRepository roleRepository) : base(context)
        {
            _context = context;
            _hostEnv = hostEnvironment;
            _userRepo = userRepository;
            _roleRepo = roleRepository;
        }

        public override List<string> DefaultPermission()
        {
            return new List<string>
            {
                $"AuthorizedTo.{nameof(Member)}.Add",
                $"AuthorizedTo.{nameof(Member)}.View",
                $"AuthorizedTo.{nameof(Member)}.Edit",
                $"AuthorizedTo.{nameof(Member)}.Remove",
                $"AuthorizedTo.{nameof(Member)}.Activate",
                $"AuthorizedTo.{nameof(Member)}.Terminate",
                $"AuthorizedTo.{nameof(Member)}.ByState",
                $"AuthorizedTo.{nameof(Member)}.GrowthSummary",
                $"AuthorizedTo.{nameof(Member)}.GrowthSummaryHigher",
                $"AuthorizedTo.{nameof(Member)}.Unregistered",
                $"AuthorizedTo.{nameof(Member)}.CarrerSummary",
                $"AuthorizedTo.{nameof(Member)}.CarrerSummaryHigher",
                $"AuthorizedTo.{nameof(Member)}.Close",
                $"AuthorizedTo.{nameof(Member)}.ClosedTransaction",
                $"AuthorizedTo.{nameof(Member)}.ShowTransaction",
            };
        }

        public override async Task<IReadOnlyList<Member>> GetAllAsync()
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var cashierRole = await _roleRepo.GetByName("Cashier");

            if(user.Staff.StaffOrganizations.Any() && !user.UserRoles.Where(c => c.RoleId == cashierRole.Id).Any())
                return await _context.Members
                    .Include(c => c.MemberOrganizations)
                    .ThenInclude(c => c.Organization)
                    .Include(c => c.Spouses)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.Occupations)
                    .ThenInclude(c => c.WorkType)
                    .Include(c => c.Gender)
                    .Include(c => c.Educations)
                    .Include(c => c.Guardians)
                    .Include(c => c.MemberPassBooks)
                    .ThenInclude(c => c.PassBook)
                    .AsSingleQuery()
                    .Where(c => c.MemberOrganizations.Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .Any())
                    .ToListAsync();

            else
                return await _context.Members
                    .Include(c => c.MemberOrganizations)
                    .ThenInclude(c => c.Organization)
                    .Include(c => c.Spouses)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.Occupations)
                    .ThenInclude(c => c.WorkType)
                    .Include(c => c.Gender)
                    .Include(c => c.Educations)
                    .Include(c => c.Guardians)
                    .Include(c => c.MemberPassBooks)
                    .ThenInclude(c => c.PassBook)
                    .AsSingleQuery()
                    .ToListAsync();
        }

        public override async Task<Member> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var cashierRole = await _roleRepo.GetByName("Cashier");

            if(user.Staff.StaffOrganizations.Any() && !user.UserRoles.Where(c => c.RoleId == cashierRole.Id).Any())
                return  await _context.Members
                    .Include(c => c.MemberOrganizations)
                    .ThenInclude(c => c.Organization)
                    .Include(c => c.Spouses)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.Occupations)
                    .ThenInclude(c => c.WorkType)
                    .Include(c => c.Gender)
                    .Include(c => c.Educations)
                    .Include(c => c.Guardians)
                    .Include(c => c.MemberPassBooks)
                    .ThenInclude(c => c.PassBook)
                    .AsSingleQuery()
                    .SingleOrDefaultAsync(c => c.Id == id && c.MemberOrganizations.Where(c => c.OrganizationId == user.Staff
                        .StaffOrganizations[0].OrganizationId).Any());
                
            else
                return await _context.Members
                    .Include(c => c.MemberOrganizations)
                    .ThenInclude(c => c.Organization)
                    .Include(c => c.Spouses)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.Occupations)
                    .ThenInclude(c => c.WorkType)
                    .Include(c => c.Gender)
                    .Include(c => c.Educations)
                    .Include(c => c.Guardians)
                    .Include(c => c.MemberPassBooks)
                    .ThenInclude(c => c.PassBook)
                    .AsSingleQuery()
                    .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<string> SavePicture(IFormFile xFile, string holderType)
        {
            string xName = new String(Path.GetFileNameWithoutExtension(xFile.FileName)
                .Take(10)
                .ToArray())
                .Replace(' ', '_');

            xName = "Files/" + holderType + "_Pictures/" + xName + "-" + DateTime.Now.ToString("yymmssfff") +
                Path.GetExtension(xFile.FileName);

            var imagePath = Path.Combine(_hostEnv.ContentRootPath, xName);

            using (var image = new MagickImage(xFile.OpenReadStream()))
            {
                var size = new MagickGeometry(200, 200);

                size.IgnoreAspectRatio = true;

                image.Resize(size);

                await image.WriteAsync(imagePath);
            }

            return xName;
        }

        public async Task<string> SaveSignature(IFormFile xFile)
        {
            string xName = new String(Path.GetFileNameWithoutExtension(xFile.FileName)
                .Take(10)
                .ToArray())
                .Replace(' ', '_');

            xName = "Files/Signature/" + xName + "-" + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(xFile.FileName);

            var imagePath = Path.Combine(_hostEnv.ContentRootPath, xName);

            using (var image = new MagickImage(xFile.OpenReadStream()))
            {
                var size = new MagickGeometry(300, 300);

                size.IgnoreAspectRatio = true;

                image.Resize(size);

                await image.WriteAsync(imagePath);
            }

            return xName;
        }

        public async Task<string> SaveAttachment(IFormFile xFile)
        {
            string xName = new String(Path.GetFileNameWithoutExtension(xFile.FileName)
                .Take(10)
                .ToArray())
                .Replace(' ', '_');

            xName = "Files/Attachments/" + xName + "-" + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(xFile.FileName);

            var imagePath = Path.Combine(_hostEnv.ContentRootPath, xName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await xFile.CopyToAsync(fileStream);
            }

            return xName;
        }

        public void DeleteFile(string fileName)
        {
            if (fileName != null)
            {
                var imagePath = Path.Combine(_hostEnv.ContentRootPath, fileName);

                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
        }

        public async Task<List<Member>> GetMembersByState(string state)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if(user.Staff.StaffOrganizations.Any())
                return await _context.Members
                    .Where(c => c.LastObjectState == state)
                    .Include(c => c.Gender)
                    .AsSingleQuery()
                    .Where(c => c.MemberOrganizations.Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .Any())
                    .ToListAsync();
            else
                return await _context.Members
                    .Where(c => c.LastObjectState == state)
                    .Include(c => c.Gender)
                    .AsSingleQuery()
                    .ToListAsync();
        }

        public async Task<List<Member>> GetUnRegisteredMembers()
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if(user.Staff.StaffOrganizations.Any())
                return await _context.Members
                    .Include(c => c.MemberPassBooks)
                    .ThenInclude(c => c.PassBook)
                    .Include(c => c.Gender)
                    .Where(c => c.User == null && c.MemberOrganizations.Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0]
                        .OrganizationId).Any())
                    .ToListAsync();
            else
                return await _context.Members
                    .Include(c => c.MemberPassBooks)
                    .ThenInclude(c => c.PassBook)
                    .Include(c => c.Gender)
                    .Where(c => c.User == null)
                    .ToListAsync();
        }

        private async Task<List<List<Occupation>>> ListOfMemberOccupations()
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if(user.Staff.StaffOrganizations.Any())
                return await _context.Members
                    .Include(c => c.Occupations)
                    .ThenInclude(c => c.WorkType)
                    .Where(c => c.MemberOrganizations.Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .Any())
                    .Select(c => c.Occupations)
                    .ToListAsync();
            else
                return await _context.Members
                    .Include(c => c.Occupations)
                    .ThenInclude(c => c.WorkType)
                    .Select(c => c.Occupations)
                    .ToListAsync();
        }

        public async Task<List<int>> GetMemberCarrers()
        {
            var numberOfCarrers = new List<int>();
            var privates = 0;
            var gevornmentals = 0;
            var nonGevornmentals = 0;
            var occupations = await ListOfMemberOccupations();

            occupations.ForEach(member =>
            {
                member.ForEach(occupation =>
                {
                    if (occupation.WorkType.Id == WorkType.Private.Id)
                        privates += 1;
                    else if (occupation.WorkType.Id == WorkType.Governmental.Id)
                        gevornmentals += 1;
                    else
                        nonGevornmentals += 1;
                });
            });

            numberOfCarrers.AddRange(new List<int>() { privates, gevornmentals, nonGevornmentals });

            return numberOfCarrers;
        }

        public async Task<List<Member>> GetMembersByYear(int year)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if(user.Staff.StaffOrganizations.Any())
                return await _context.Members
                    .Include(c => c.Gender)
                    .Where(c => c.CreatedDate.Year == year && c.MemberOrganizations.Where(c => c.OrganizationId == user.Staff
                        .StaffOrganizations[0].OrganizationId).Any())
                    .ToListAsync();
            else
                return await _context.Members
                    .Include(c => c.Gender)
                    .Where(c => c.CreatedDate.Year == year)
                    .ToListAsync();
        }
    }
}
