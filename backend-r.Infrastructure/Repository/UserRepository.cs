namespace backend_r.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private IUnitOfWork unitOfWork;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (unitOfWork == null)
                {
                    unitOfWork = new UnitOfWork(this._context);
                }

                return unitOfWork;
            }
            set
            {
                unitOfWork = new UnitOfWork(this._context);
            }
        }
        private readonly UserManager<User> _userManager;
        private readonly JoshuaContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public UserRepository(IRoleRepository repository, IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager, JoshuaContext context)
        {
            _context = context;
            _userManager = userManager;
            _httpContext = httpContextAccessor;
        }

        public async Task<User> GetAuthenticatedUser()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "user_id").Value;

            return await GetUser(int.Parse(userId));
        }

        public async Task<string> Login(string emailOrPhone, string password)
        {
            var user = await GetUserByEmailorPhone(emailOrPhone);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                if (user.Member != null)
                    throw new DomainException("We're not finished building for you to login, Sorry... 🤔🤔");

                var userClaims = await _userManager.GetClaimsAsync(user);
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("user_id", user.Id.ToString()),
                    new Claim("email", user.Email)
                };

                foreach (var userClaim in userClaims)
                {
                    authClaims.Add(new Claim(userClaim.Type, userClaim.Value));
                }

                foreach (var userRole in userRoles)
                {
                    var role = await _context.Roles.SingleOrDefaultAsync(c => c.Name == userRole);
                    var userRoleClaims = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync();

                    foreach (var userRoleClaim in userRoleClaims)
                    {
                        authClaims.Add(new Claim(userRoleClaim.ClaimType, userRoleClaim.ClaimValue));
                    }

                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var claimIdentity = new ClaimsIdentity(
                    authClaims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true
                };

                await _httpContext.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimIdentity),
                    authProperties
                );

                _httpContext.HttpContext.User = new ClaimsPrincipal(claimIdentity);

                if (user.IsLogged)
                    user.LoggedCount = user.LoggedCount + 1;
                else
                {
                    user.IsLogged = true;
                    user.LoggedCount = 1;
                }

                await _userManager.UpdateAsync(user);

                return "Logged Successfully";
            }
            return "Unauthorized";
        }

        public async Task<string> Logout(User user)
        {
            if (user.LoggedCount == 1)
                user.IsLogged = false;

            if (user.LoggedCount > 0)
                user.LoggedCount = user.LoggedCount - 1;

            await _userManager.UpdateAsync(user);

            await _httpContext.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return "Logged out Successfully";
        }

        public async Task<string> Register(User user)
        {
            var userExist = await _userManager.FindByEmailAsync(user.Email);

            if (userExist != null)
                return "User Already Exist!";

            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (!result.Succeeded)
            {
                string errors = "";
                foreach (var item in result.Errors)
                {
                    errors += item.Description;
                    errors += " ";
                }

                return errors;
            }

            return "User Created.";
        }

        public async Task<IReadOnlyList<User>> GetUsers()
        {
            return await _context.Users
                .Include(c => c.Staff)
                .Include(c => c.Member)
                .AsSingleQuery()
                .ToListAsync();
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users
                .Include(c => c.UserRoles)
                .ThenInclude(c => c.Role)
                .ThenInclude(c => c.RoleClaims)
                .Include(c => c.Staff)
                .ThenInclude(c => c.Gender)
                .Include(c => c.Staff)
                .ThenInclude(c => c.StaffOrganizations)
                .ThenInclude(c => c.Organization)
                .Include(c => c.Member)
                .ThenInclude(c => c.Gender)
                .Include(c => c.Member)
                .ThenInclude(c => c.MemberOrganizations)
                .ThenInclude(c => c.Organization)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<User> GetUserByEmailorPhone(string emailOrPhone)
        {
            return await _context.Users
                .Include(c => c.Member)
                .SingleOrDefaultAsync(c => c.Email == emailOrPhone || c.PhoneNumber == emailOrPhone);
        }

        public async Task<string> Modify(User model)
        {
            var user = await GetUser(model.Id);

            if (user == null)
                return null;

            user.UserRoles.ForEach(userRole =>
            {
                if (userRole.RoleId != model.UserRoles[0].RoleId)
                    user.UserRoles.Add(new UserRole(model.UserRoles[0].RoleId));
            });

            user.Email = model.Email != null ? model.Email : user.Email;
            user.PhoneNumber = model.PhoneNumber != null ? model.PhoneNumber : user.PhoneNumber;

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(user, model.PasswordHash);
            user.PasswordHash = hashedPassword;

            await _userManager.UpdateAsync(user);

            return "Updated.";
        }

        public async Task<string> Reset(int id, string password)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return null;

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(user, password);

            user.PasswordHash = hashedPassword;

            await _userManager.UpdateAsync(user);

            return "Reset.";
        }

        public async Task<bool> CheckExistence(int id)
        {
            return await _context.Users.AnyAsync(c => c.Id == id);
        }

        public List<string> DefaultPermission()
        {
            return new List<string>
            {
                $"AuthorizedTo.{nameof(User)}.Add",
                $"AuthorizedTo.{nameof(User)}.View",
                $"AuthorizedTo.{nameof(User)}.Edit",
                $"AuthorizedTo.{nameof(User)}.Remove",
            };
        }
    }
}