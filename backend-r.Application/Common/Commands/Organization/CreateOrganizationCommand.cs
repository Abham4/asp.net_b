namespace backend_r.Application.Common.Commands.Organization
{
    public class CreateOrganizationCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int OrganizationTypeId { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public bool IsContactPerson { get; set; }
        public int StaffId { get; set; }
        public List<Address> Addresses { get; set; }
        public List<IdDefinition> IdDefinitions { get; set; }
    }

    public class Address
    {
        public int AddressType { get; set; }
        public int Attribute { get; set; }
        public string Value { get; set; }
    }

    public class IdDefinition
    {
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string PrefSep { get; set; }
        public string SuffSep { get; set; }
        public int NextValue { get; set; }
        public int Length { get; set; }
    }

    class CreateOrganizationHandler : IRequestHandler<CreateOrganizationCommand, string>
    {
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IResourceRepository _resourceRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateOrganizationHandler> _logger;

        public CreateOrganizationHandler(IOrganizationRepository repository, IAddressRepository addressRepository,
            IResourceRepository resourceRepository, ILogger<CreateOrganizationHandler> logger, IUserRepository userRepository)
        {
            _organizationRepo = repository;
            _addressRepo = addressRepository;
            _resourceRepo = resourceRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var resourceMember = await _resourceRepo.GetResourceByType("Member");
            var resourceStaff = await _resourceRepo.GetResourceByType("Staff");
            var resourceLoanPrincipal = await _resourceRepo.GetResourceByType(VoucherType.Loan_Principal.Name);
            var resourceLoanInterest = await _resourceRepo.GetResourceByType(VoucherType.Loan_Interest.Name);
            var resourceLoanPenality = await _resourceRepo.GetResourceByType(VoucherType.Loan_Penality.Name);
            var resourceSavePrincipal = await _resourceRepo.GetResourceByType(VoucherType.Save_Principal.Name);
            var resourceSaveInterest = await _resourceRepo.GetResourceByType(VoucherType.Save_Interest.Name);
            var resourceSavePenality = await _resourceRepo.GetResourceByType(VoucherType.Save_Penality.Name);
            var resourceSharePrincipal = await _resourceRepo.GetResourceByType(VoucherType.Share_Principal.Name);
            var resourceShareInterest = await _resourceRepo.GetResourceByType(VoucherType.Share_Interest.Name);
            var resourceSharePenality = await _resourceRepo.GetResourceByType(VoucherType.Share_Penality.Name);
            var resourceDeposit = await _resourceRepo.GetResourceByType(VoucherType.Deposit.Name);
            var resourceWithdrawal = await _resourceRepo.GetResourceByType(VoucherType.Withdrawal.Name);
            var resourceDailyLoan = await _resourceRepo.GetResourceByType(VoucherType.Daily_Loan_Bigining_Balance.Name);
            var resourceDailySave = await _resourceRepo.GetResourceByType(VoucherType.Daily_Save_Bigining_Balance.Name);
            var resourceDailyShare = await _resourceRepo.GetResourceByType(VoucherType.Daily_Share_Bigining_Balance.Name);
            var resourceMonthlyLoan = await _resourceRepo.GetResourceByType(VoucherType.Monthly_Loan_Bigining_Balance
                .Name);
            var resourceMonthlySave = await _resourceRepo.GetResourceByType(VoucherType.Monthly_Save_Bigining_Balance
                .Name);
            var resourceMonthlyShare = await _resourceRepo.GetResourceByType(VoucherType.Monthly_Share_Bigining_Balance
                .Name);
            var resourceYearlyLoan = await _resourceRepo.GetResourceByType(VoucherType.Yearly_Loan_Bigining_Balance.Name);
            var resourceYearlySave = await _resourceRepo.GetResourceByType(VoucherType.Yearly_Save_Bigining_Balance.Name);
            var resourceYearlyShare = await _resourceRepo.GetResourceByType(VoucherType.Yearly_Share_Bigining_Balance
                .Name);
            var resourceDisbursement = await _resourceRepo.GetResourceByType(VoucherType.Disbursement.Name);

            int[] resIds = { resourceMember.Id, resourceStaff.Id, resourceLoanPrincipal.Id, resourceLoanInterest.Id,
                resourceLoanPenality.Id, resourceSavePrincipal.Id, resourceSaveInterest.Id, resourceSavePenality.Id,
                resourceSharePrincipal.Id, resourceShareInterest.Id, resourceSharePenality.Id, resourceDeposit.Id,
                resourceWithdrawal.Id, resourceDailyLoan.Id, resourceDailySave.Id, resourceDailyShare.Id,
                resourceMonthlyLoan.Id, resourceMonthlySave.Id, resourceMonthlyShare.Id, resourceYearlyLoan.Id,
                resourceYearlySave.Id, resourceYearlyShare.Id, resourceDisbursement.Id };
            var idDefinitions = new List<Domain.Entities.IdDefinition>();

            for (int i = 0; i < 23; i++)
            {
                idDefinitions.Add(new Domain.Entities.IdDefinition(resIds[i], request.IdDefinitions[i].Prefix,
                    request.IdDefinitions[i].Suffix, request.IdDefinitions[i].PrefSep, request.IdDefinitions[i].SuffSep,
                    request.IdDefinitions[i].NextValue, request.IdDefinitions[i].Length, user.Email));
            }

            var staffOrganization = request.StaffId != 0 ? new List<Domain.Entities.StaffOrganization>()
                {
                    new Domain.Entities.StaffOrganization
                    {
                        StaffId = request.StaffId,
                        AssignmentDate = DateTime.Now,
                        IsContactPerson = request.IsContactPerson
                    }
                } : null;

            var organization = new Domain.Entities.Organization();

            if (request.OrganizationTypeId == OrganizationType.Area.Id)
                organization = new Domain.Entities.Organization(request.Name, request.ShortName, request.Description,
                    request.OrganizationTypeId, 1, request.IsActive, staffOrganization, idDefinitions, user.Email);

            else if (request.OrganizationTypeId == OrganizationType.Branch.Id)
            {
                var org = await _organizationRepo.GetByIdAsync(request.ParentId);

                if (org == null)
                    throw new DomainException("Parent doesn't exist!");

                if (org.OrganizationTypeId != OrganizationType.Area.Id)
                    throw new DomainException("Parent Organization Type must be Area!");

                organization = new Domain.Entities.Organization(request.Name, request.ShortName, request.Description,
                    request.OrganizationTypeId, request.ParentId, request.IsActive, staffOrganization, idDefinitions,
                    user.Email);
            }

            else if (request.OrganizationTypeId == OrganizationType.Sub_Branch.Id)
            {
                var org = await _organizationRepo.GetByIdAsync(request.ParentId);

                if (org == null)
                    throw new DomainException("Parent doesn't exist!");

                if (org.OrganizationTypeId != OrganizationType.Branch.Id)
                    throw new DomainException("Parent Organization Type must be Branch!");

                organization = new Domain.Entities.Organization(request.Name, request.ShortName, request.Description,
                    request.OrganizationTypeId, request.ParentId, request.IsActive, staffOrganization, idDefinitions,
                    user.Email);
            }

            _logger.LogInformation("----------Creating Organization--------");

            await _organizationRepo.AddAsync(organization);
            await _organizationRepo.UnitOfWork.SaveChanges();

            for (int i = 0; i < request.Addresses.Count(); i++)
            {
                var addressType = AddressType.List().SingleOrDefault(c => c.Id == request.Addresses[i].AddressType).Name;
                var attribute = AttributeType.List().SingleOrDefault(c => c.Id == request.Addresses[i].Attribute).Name;

                var address = new Domain.Entities.Address("Organization", addressType, request.Addresses[i].Value,
                    attribute, organization.Id, user.Email);

                _logger.LogInformation("----------Creating Organization Address---------");

                await _addressRepo.AddAsync(address);
                await _addressRepo.UnitOfWork.SaveChanges();
            }

            return "Created";
        }
    }

    public class IdDefinitionValidator : AbstractValidator<IdDefinition>
    {
        public IdDefinitionValidator()
        {
            RuleFor(c => c.Length)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.NextValue)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Prefix)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.PrefSep)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Suffix)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.SuffSep)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }

    public class AddressValidator : AbstractValidator<Address>
    {
        [Obsolete]
        public AddressValidator()
        {
            RuleFor(c => c.AddressType)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3).WithMessage("{PropertyName} with null reference!");

            When(c => c.AddressType == 1 || c.AddressType == 2, () =>
            {
                RuleFor(c => c.Attribute)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .InclusiveBetween(1, 8).WithMessage("{PropertyName} is null reference!");

                When(c => c.Attribute == 1, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Region)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Region)} can't be null!");
                });

                When(c => c.Attribute == 2, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.City)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.City)} can't be null!");
                });

                When(c => c.Attribute == 3, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Sub_City_or_Zone)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Sub_City_or_Zone)} can't be null!");
                });

                When(c => c.Attribute == 4, () =>
                {
                    RuleFor(c => c.Value)
                        .Must(IsDigit).WithMessage($"{nameof(AttributeType.Woreda)} must be digit/s only!")
                        .Matches("^[\\d]{1,5}$").WithMessage($"{nameof(AttributeType.Woreda)} must match be with range of 5 digit!")
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Woreda)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Woreda)} can't be null!");
                });

                When(c => c.Attribute == 5, () =>
                {
                    RuleFor(c => c.Value)
                        .Must(IsDigit).WithMessage($"{nameof(AttributeType.Kebele)} must be digit/s only!")
                        .Matches("^[\\d]{1,5}$").WithMessage($"{nameof(AttributeType.Kebele)} must match be with range of 5 digit!")
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Kebele)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Kebele)} can't be null!");
                });

                When(c => c.Attribute == 6, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.House_Number)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.House_Number)} can't be null!");
                });

                When(c => c.Attribute == 7, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                        .NotNull().WithMessage("{PropertyName} can't be null!")
                        .Matches("^\\+[0-9]{1,3}[0-9]{4,14}$").WithMessage($"{nameof(AttributeType.TelePhone)} is not in a correct format!"); ;
                });

                When(c => c.Attribute == 8, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                        .NotNull().WithMessage("{PropertyName} can't be null!")
                        .Matches("^\\+[0-9]{1,3}[0-9]{4,14}$").WithMessage($"{nameof(AttributeType.Mobile)} is not in a correct format!");
                });
            });

            When(c => c.AddressType == 3, () =>
            {
                RuleFor(c => c.Attribute)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .InclusiveBetween(9, 14).WithMessage("{PropertyName} is null reference!");

                When(c => c.Attribute == 9, () =>
                {
                    RuleFor(c => c.Value)
                        .EmailAddress(FluentValidation.Validators.EmailValidationMode.Net4xRegex)
                            .WithMessage($"{nameof(AttributeType.Email)} is invalid!")
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Email)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Email)} can't be null!");
                })
                .Otherwise(() =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                        .NotNull().WithMessage("{PropertyName} can't be null!")
                        .Must(IsValidUrl).WithMessage("{PropertyName} is not valid url!");
                });
            });
        }

        private bool IsDigit(string value)
        {
            return value.All(Char.IsDigit);
        }

        private bool IsValidUrl(string value)
        {
            Uri uriResult;
            return Uri.TryCreate(value, UriKind.Absolute, out uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }

    public class CreateOrganizationValidator : AbstractValidator<CreateOrganizationCommand>
    {
        public CreateOrganizationValidator()
        {
            RuleFor(c => c.IdDefinitions.Count())
                .Equal(23).WithMessage("IdDefinition must have 23 objects");

            RuleFor(c => c.Addresses.Count())
                .GreaterThanOrEqualTo(1).WithMessage("Atleast one address is needed");

            RuleFor(c => c.OrganizationTypeId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ShortName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .Must(IsLetter).WithMessage("{PropertyName} must be letters only")
                .Must(Count3).WithMessage("{PropertyName} must be 3 letters only")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }

        private bool IsLetter(string name)
        {
            return name.All(Char.IsLetter);
        }

        private bool Count3(string name)
        {
            return name.Count() == 3;
        }
    }
}