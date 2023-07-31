namespace backend_r.Application.Common.Commands.Staff
{
    public class CreateStaffCommand : IRequest<string>
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public DateTime DOB { get; set; }
        public IFormFile ProfileImg { get; set; }
        public int OrganizationId { get; set; }
        public string Addresses { get; set; }
        public string Identities { get; set; }
    }

    public class AddressVm
    {
        public int AddressType { get; set; }
        public int Attribute { get; set; }
        public string Value { get; set; }
    }

    public class IdentityVm
    {
        public int Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }

    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, string>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IIdentityRepository _identityRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateStaffCommandHandler> _logger;

        public CreateStaffCommandHandler(IStaffRepository repository, IIdentityRepository identityRepository,
            IAddressRepository addressRepository, ILogger<CreateStaffCommandHandler> logger,
            IMemberRepository memberRepository, IUserRepository userRepository)
        {
            _staffRepo = repository;
            _addressRepo = addressRepository;
            _identityRepo = identityRepository;
            _logger = logger;
            _memberRepo = memberRepository;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var addresses = JsonConvert.DeserializeObject<List<AddressVm>>(request.Addresses);
            var identities = JsonConvert.DeserializeObject<List<IdentityVm>>(request.Identities);
            var staffOrganization = new List<Domain.Entities.StaffOrganization>{
                new Domain.Entities.StaffOrganization{
                    OrganizationId = request.OrganizationId
                }
            };
            var profileImg = request.ProfileImg != null ? await _memberRepo.SavePicture(request.ProfileImg, "Staff")
                : null;
            var staff = new Domain.Entities.Staff(request.Title, request.FirstName, request.MiddleName, request.LastName,
                request.GenderId, request.DOB, staffOrganization, profileImg, user.Email);

            _logger.LogInformation("--------Creating Staff-----------");

            await _staffRepo.AddAsync(staff);
            await _staffRepo.UnitOfWork.SaveChanges();

            for (int i = 0; i < addresses.Count(); i++)
            {
                var addressType = AddressType.List().SingleOrDefault(c => c.Id == addresses[i].AddressType).Name;
                var attribute = AttributeType.List().SingleOrDefault(c => c.Id == addresses[i].Attribute).Name;

                var address = new Domain.Entities.Address("Staff", addressType, addresses[i]
                    .Value, attribute, staff.Id, user.Email);

                _logger.LogInformation("--------Creating Staff Address----------");

                await _addressRepo.AddAsync(address);
                await _addressRepo.UnitOfWork.SaveChanges();
            }

            if (identities != null)
            {
                for (int i = 0; i < identities.Count(); i++)
                {
                    var identityType = IdentityType.List().SingleOrDefault(c => c.Id == identities[i].Type).Name;

                    var identity = new Domain.Entities.Identity("Staff", identityType, identities[i].Description,
                        identities[i].Number, staff.Id, user.Email);

                    _logger.LogInformation("--------Creating Staff Identity-----------");

                    await _identityRepo.AddAsync(identity);
                    await _identityRepo.UnitOfWork.SaveChanges();
                }
            }

            return "Created";
        }
    }

    public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
    {
        public CreateStaffCommandValidator()
        {
            RuleFor(c => c.Addresses)
                .NotEmpty().WithMessage("{PropertyName} atleast one address is required")
                .Custom((address, context) =>
                {
                    var addresses = new List<AddressVm>();
                    var regexNum = new Regex(@"^[\d]{1,5}$");
                    var regexMobile = new Regex(@"^\+[0-9]{1,3}[0-9]{4,14}$");

                    if (address != null)
                    {
                        try
                        {
                            addresses = JsonConvert.DeserializeObject<List<AddressVm>>(address);
                        }
                        catch (Exception)
                        {
                            throw new DomainException("Broken Address!");
                        }
                    }

                    foreach (var addr in addresses)
                    {
                        if (addr.AddressType < 1 || addr.AddressType > 3)
                            context.AddFailure("AddressType", "AddressType with null referece!");

                        if (addr.AddressType == 1 || addr.AddressType == 2)
                        {
                            if (addr.Attribute < 1 || addr.Attribute > 8)
                                context.AddFailure("Attribute", "Attribute with null reference!");

                            if (addr.Attribute == 1)
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.Region.Name,
                                        AttributeType.Region.Name + " can't be empty or null!");

                            if (addr.Attribute == 2)
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.City.Name,
                                        AttributeType.City.Name + " can't be empty or null!");

                            if (addr.Attribute == 3)
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.Sub_City_or_Zone.Name,
                                        AttributeType.Sub_City_or_Zone.Name + " can't be empty or null!");

                            if (addr.Attribute == 4)
                            {
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.Woreda.Name,
                                        AttributeType.Woreda.Name + " can't be empty or null!");

                                if (!regexNum.IsMatch(addr.Value))
                                    context.AddFailure(AttributeType.Woreda.Name,
                                        AttributeType.Woreda.Name + " can be numbers only!");
                            }

                            if (addr.Attribute == 5)
                            {
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.Kebele.Name,
                                        AttributeType.Kebele.Name + " can't be empty or null!");

                                if (!regexNum.IsMatch(addr.Value))
                                    context.AddFailure(AttributeType.Kebele.Name,
                                        AttributeType.Kebele.Name + " can be numbers only!");
                            }

                            if (addr.Attribute == 6)
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.House_Number.Name,
                                        AttributeType.House_Number.Name + " can't be empty or null!");

                            if (addr.Attribute == 7)
                            {
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.TelePhone.Name,
                                        AttributeType.TelePhone.Name + " can't be empty or null!");

                                if (!regexMobile.IsMatch(addr.Value))
                                    context.AddFailure(AttributeType.TelePhone.Name,
                                        AttributeType.TelePhone.Name + " Invalid Telephone Number!");
                            }

                            if (addr.Attribute == 8)
                            {
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure(AttributeType.Mobile.Name,
                                        AttributeType.Mobile.Name + " can't be empty or null!");

                                if (!regexMobile.IsMatch(addr.Value))
                                    context.AddFailure(AttributeType.Mobile.Name,
                                        AttributeType.Mobile.Name + " Invalid Mobile Number!");
                            }
                        }

                        else
                        {
                            if (addr.Attribute < 9 || addr.Attribute > 14)
                                context.AddFailure("Attribute", "Attribute with null reference!");

                            if (addr.Attribute == 9)
                            {
                                if (!IsValidEmail(addr.Value))
                                    context.AddFailure(AttributeType.Email.Name, AttributeType.Email.Name +
                                        " Invalid Email Address!");
                            }

                            else
                            {
                                if (addr.Value == null || addr.Value == string.Empty)
                                    context.AddFailure("Value", "Value can't be empty or null!");

                                if (!IsValidUrl(addr.Value))
                                    context.AddFailure("Value", "Value is not valid url!");
                            }
                        }
                    }
                });


            When(c => c.Identities != null, () =>
            {
                RuleFor(c => c.Identities)
                    .Custom((identity, context) =>
                    {
                        var identities = new List<IdentityVm>();

                        if (identity != null)
                        {
                            try
                            {
                                identities = JsonConvert.DeserializeObject<List<IdentityVm>>(identity);
                            }
                            catch (Exception)
                            {
                                throw new DomainException("Broken Identity!");
                            }
                        }

                        foreach (var iden in identities)
                        {
                            if (iden.Type < 1 || iden.Type > 4)
                            {
                                context.AddFailure("Identity", "Identity with null referece!");
                            }
                        }
                    });
            });

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} must not be null");

            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .Must(IsLetter).WithMessage("{PropertyName} must be letters only")
                .NotNull().MaximumLength(100).WithMessage("{PropertyName} length not exceed 100 characters");

            RuleFor(c => c.MiddleName)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .Must(IsLetter).WithMessage("{PropertyName} must be letters only")
                .NotNull().MaximumLength(100).WithMessage("{PropertyName} length not exceed 100 characters");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .Must(IsLetter).WithMessage("{PropertyName} must be letters only")
                .NotNull().MaximumLength(100).WithMessage("{PropertyName} length not exceed 100 characters");

            RuleFor(c => c.OrganizationId)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} must not be null");

            RuleFor(c => c.GenderId)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .InclusiveBetween(1, 3).WithMessage("{PropertyName} with null reference")
                .NotNull().WithMessage("{PropertyName} must not be null");
        }

        private bool IsLetter(string name)
        {
            return name.All(Char.IsLetter);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsValidUrl(string value)
        {
            Uri uriResult;
            return Uri.TryCreate(value, UriKind.Absolute, out uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}