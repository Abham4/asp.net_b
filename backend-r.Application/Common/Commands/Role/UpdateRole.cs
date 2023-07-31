namespace backend_r.Application.Common.Commands.Role
{
    public class UpdateRole : IRequest<string>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName => Name.ToUpper();
        public List<RoleClaim> RoleClaims { get; set; }
    }

    public class UpdateRoleHandler : IRequestHandler<UpdateRole, string>
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
        private readonly ILogger<UpdateRoleHandler> _logger;

        public UpdateRoleHandler(ILogger<UpdateRoleHandler> logger, IRoleRepository roleRepository,
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

        public async Task<string> Handle(UpdateRole request, CancellationToken cancellationToken)
        {
            var role = await _roleRepo.GetByIdAsync(request.Id);

            if (role == null)
                return null;

            if (request.RoleClaims != null)
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

            if (request.RoleClaims != null)
                foreach (var roleClaim in request.RoleClaims)
                {
                    foreach (var value in roleClaim.Value)
                    {
                        roleClaims.Add(new Domain.Entities.RoleClaim(value));
                    }
                }

            role.Name = request.Name != null ? request.Name : role.Name;
            role.NormalizedName = request.Name != null ? request.NormalizedName : role.NormalizedName;
            role.RoleClaims = roleClaims.Count() > 0 ? roleClaims : role.RoleClaims;

            _roleRepo.Modify(role);
            await _roleRepo.UnitOfWork.SaveChanges();

            return "Updated";
        }
    }

    public class UpdateRoleValidator : AbstractValidator<UpdateRole>
    {
        public UpdateRoleValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.RoleClaims)
                .Must(IsDistnict).WithMessage("{PropertyName} can't have duplicate values!");

            RuleForEach(c => c.RoleClaims).SetValidator(new RoleClaimValidator());
        }

        private bool IsDistnict(List<RoleClaim> roleClaims)
        {
            if (roleClaims == null)
                return true;

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