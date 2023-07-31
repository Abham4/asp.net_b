namespace backend_r.Application.Common.Commands.Role
{
    public class CreateRoleCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string NormalizedName => Name.ToUpper();
        public List<RoleClaim> RoleClaims { get; set; }
    }

    public class RoleClaim
    {
        public int Entity { get; set; }
        public List<string> Value { get; set; }
    }

    public enum Entity
    {
        Organization,
        Member,
        PassBook,
        Product,
        PurchasedProduct,
        Role,
        Staff,
        User,
        Voucher
    }

    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, string>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IUserRepository _userRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly IPassBookRepository _passBookRepo;
        private readonly IProductRepository _productRepo;
        private readonly IPurchasedProductRepository _purchasedProductRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<CreateRoleHandler> _logger;

        public CreateRoleHandler(IRoleRepository roleRepository, ILogger<CreateRoleHandler> logger,
            IOrganizationRepository organizationRepository, IStaffRepository staffRepository,
            IUserRepository userRepository, IVoucherRepository voucherRepository,
            IPassBookRepository passBookRepository, IProductRepository productRepository,
            IPurchasedProductRepository purchasedProductRepository, IMemberRepository memberRepository)
        {
            _logger = logger;
            _roleRepo = roleRepository;
            _organizationRepo = organizationRepository;
            _staffRepo = staffRepository;
            _userRepo = userRepository;
            _voucherRepo = voucherRepository;
            _passBookRepo = passBookRepository;
            _productRepo = productRepository;
            _purchasedProductRepo = purchasedProductRepository;
            _memberRepo = memberRepository;
        }

        public async Task<string> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            foreach (var roleClaim in request.RoleClaims)
            {
                var entity = Enum.GetName(typeof(Entity), roleClaim.Entity);

                if (entity == null)
                    throw new DomainException("Entity Not Found!");

                foreach (var value in roleClaim.Value)
                {
                    if (entity == Entity.Organization.ToString())
                    {
                        var organizationPermissions = _organizationRepo.DefaultPermission();

                        if (!organizationPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.Member.ToString())
                    {
                        var memberPermissions = _memberRepo.DefaultPermission();

                        if (!memberPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.PassBook.ToString())
                    {
                        var passbookPermissions = _passBookRepo.DefaultPermission();

                        if (!passbookPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.Product.ToString())
                    {
                        var productPermissions = _productRepo.DefaultPermission();

                        if (!productPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.PurchasedProduct.ToString())
                    {
                        var purchasedProductPermissions = _purchasedProductRepo.DefaultPermission();

                        if (!purchasedProductPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.Staff.ToString())
                    {
                        var staffPermissions = _staffRepo.DefaultPermission();

                        if (!staffPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.Voucher.ToString())
                    {
                        var voucherPermissions = _voucherRepo.DefaultPermission();

                        if (!voucherPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.Role.ToString())
                    {
                        var rolePermissions = _roleRepo.DefaultPermission();

                        if (!rolePermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }

                    else if (entity == Entity.User.ToString())
                    {
                        var userPermissions = _userRepo.DefaultPermission();

                        if (!userPermissions.Any(c => c == value))
                            throw new DomainException("Permission Not Found!");
                    }
                }
            }

            var roleClaims = new List<Domain.Entities.RoleClaim>();

            foreach (var roleClaim in request.RoleClaims)
            {
                foreach (var value in roleClaim.Value)
                {
                    roleClaims.Add(new Domain.Entities.RoleClaim(value));
                }
            }

            var role = new Domain.Entities.Role(request.Name, request.NormalizedName, roleClaims, user.Email);

            _logger.LogInformation("-------Creating Role------");

            await _roleRepo.AddAsync(role);
            await _roleRepo.UnitOfWork.SaveChanges();

            return "Created";
        }
    }

    public class RoleClaimValidator : AbstractValidator<RoleClaim>
    {
        public RoleClaimValidator()
        {
            RuleFor(c => c.Entity)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be positive interger!");

            RuleFor(c => c.Value.Count())
                .GreaterThanOrEqualTo(1).WithMessage("Must have 1 or more claim value!");

            RuleFor(c => c.Value)
                .Must(IsDistnict).WithMessage("{PropertyName} can't have duplicate values!");
        }

        private bool IsDistnict(List<string> values)
        {
            if (values == null)
                return true;

            var uniques = new HashSet<string>();

            foreach (var value in values)
            {
                if (uniques.Any(c => c == value))
                    return false;

                uniques.Add(value);
            }

            return true;
        }
    }

    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.RoleClaims.Count())
                .GreaterThanOrEqualTo(1).WithMessage("Must have 1 or more claim!");

            RuleFor(c => c.RoleClaims)
                .Must(IsDistnict).WithMessage("Role claims must be distinct!");

            RuleForEach(c => c.RoleClaims).SetValidator(new RoleClaimValidator());
        }

        private bool IsDistnict(List<RoleClaim> roleClaims)
        {
            var uniques = new HashSet<int>();

            foreach (var roleClaim in roleClaims)
            {
                if (uniques.Any(c => c == roleClaim.Entity))
                    return false;

                uniques.Add(roleClaim.Entity);
            }

            return true;
        }
    }
}