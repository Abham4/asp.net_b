namespace backend_r.Application.Common.Commands.User
{
    public class RegisterCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public string WhoToRegister { get; set; }
        public int StaffId { get; set; }
        public int MemberId { get; set; }
    }

    public class UserRole
    {
        public int RoleId { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly IUserRepository _userRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(IUserRepository userRepository, ILogger<RegisterCommandHandler> logger,
            IMemberRepository memberRepository, IRoleRepository roleRepository,
            IStaffRepository staffRepository)
        {
            _logger = logger;
            _userRepo = userRepository;
            _memberRepo = memberRepository;
            _roleRepo = roleRepository;
            _staffRepo = staffRepository;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var authenticatedUser = await _userRepo.GetAuthenticatedUser();
            var userRoles = new List<Domain.Entities.UserRole>();
            var branchManagerRole = await _roleRepo.GetByName("Branch Manager");
            var cashierRole = await _roleRepo.GetByName("Cashier");
            var promotionOfficerRole = await _roleRepo.GetByName("Promotion Officer");
            var staff = new Domain.Entities.Staff();
            var user = new Domain.Entities.User();

            if (request.WhoToRegister == "Staff")
            {
                staff = await _staffRepo.GetByIdAsync(request.StaffId);

                if (staff == null)
                    throw new DomainException("Staff Doesn't Exist!");

                request.UserRoles.ForEach(e =>
                {
                    if ((e.RoleId == branchManagerRole.Id || e.RoleId == cashierRole.Id ||
                        e.RoleId == promotionOfficerRole.Id) &&
                        !_staffRepo.DoesStaffConnectedToOrganization(request.StaffId))
                        throw new DomainException("Staff must have an Organization!");
                });

                for (int i = 0; i < request.UserRoles.Count(); i++)
                {
                    userRoles.Add(new Domain.Entities.UserRole(request.UserRoles[i].RoleId));
                }

                user = new Domain.Entities.User(request.Email, Guid.NewGuid().ToString(), request.Email,
                    request.PhoneNumber, request.ConfirmPassword, userRoles, staff, authenticatedUser.Email);
            }

            if (request.WhoToRegister == "Member")
            {
                var member = await _memberRepo.GetByIdAsync(request.MemberId);

                if (member == null)
                    throw new DomainException("Member Doesn't Exist!");

                user = new Domain.Entities.User(request.Email, Guid.NewGuid().ToString(), request.Email,
                    request.PhoneNumber, request.ConfirmPassword, member, authenticatedUser.Email);
            }

            var result = await _userRepo.Register(user);

            _logger.LogInformation("------Creating User------{@result}", result);

            return result;
        }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(c => c.Email)
                .EmailAddress()
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .Length(8, 20)
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ConfirmPassword)
                .Equal(c => c.Password).WithMessage("{PropertyName} doesn't match with Password");

            RuleFor(c => c.WhoToRegister)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .Must(c => c == "Staff" || c == "Member").WithMessage("{PropertyName} must be 'Staff' or 'Member'!");

            When(c => c.WhoToRegister == "Staff", () =>
            {
                RuleFor(c => c.StaffId)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty")
                    .NotNull().WithMessage("{PropertyName} can't be null");

                RuleFor(c => c.UserRoles.Count())
                    .GreaterThanOrEqualTo(1).WithMessage("User must've atleast 1 role!");
            });

            When(c => c.WhoToRegister == "Member", () =>
            {
                RuleFor(c => c.MemberId)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty")
                    .NotNull().WithMessage("{PropertyName} can't be null");
            });
        }
    }
}