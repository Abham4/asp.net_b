namespace backend_r.Application.Common.Commands.Member
{
    public class CreateMemberCommand : IRequest<string>
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public int GenderId { get; set; }
        public DateTime DOB { get; set; }
        public IFormFile ProfileImg { get; set; }
        public IFormFile Signature { get; set; }
        public int OrganizationId { get; set; }
        public string PassBookCode { get; set; }
        public string Spouses { get; set; }
        public string Educations { get; set; }
        public string Type { get; set; }
        public string Addresses { get; set; }
        public string Identity { get; set; }
        public string Occupations { get; set; }
        public string Guardians { get; set; }
    }

    public class Occupation
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public int WorkTypeId { get; set; }
        public double Income { get; set; }
        public bool IsActive { get; set; }
        public List<AddressVm> Addresses { get; set; }
    }

    public class IdentityVm
    {
        public int Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }

    public class AddressVm
    {
        public int AddressType { get; set; }
        public int Attribute { get; set; }
        public string Value { get; set; }
    }

    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, string>
    {
        private readonly IMemberRepository _memberRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IIdentityRepository _identityRepo;
        private readonly IObjectStateRepository _objectStateRepo;
        private readonly IObjectStateDefnRepository _objectStateDefnRepo;
        private readonly IIdDefinitionRepository _idDefinitionRepo;
        private readonly IResourceRepository _resourceRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IPassBookRepository _passBookRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateMemberCommandHandler> _logger;

        public CreateMemberCommandHandler(IMemberRepository memberRepository, ILogger<CreateMemberCommandHandler> logger,
            IAddressRepository addressRepository, IIdentityRepository identityRepository,
            IObjectStateRepository objectStateRepository, IIdDefinitionRepository idDefinitionRepository,
            IResourceRepository resourceRepository, IObjectStateDefnRepository objectStateDefnRepository,
            IAccountRepository accountRepository, IPassBookRepository passBookRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _accountRepo = accountRepository;
            _memberRepo = memberRepository;
            _addressRepo = addressRepository;
            _identityRepo = identityRepository;
            _objectStateRepo = objectStateRepository;
            _idDefinitionRepo = idDefinitionRepository;
            _resourceRepo = resourceRepository;
            _objectStateDefnRepo = objectStateDefnRepository;
            _passBookRepo = passBookRepository;
            _userRepo = userRepository;
        }

        private int countDigit(int number)
        {
            if (number / 10 == 0)
                return 1;

            return 1 + countDigit(number / 10);
        }

        public async Task<string> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            List<Education> educations = null;
            List<AddressVm> addresses = null;
            List<IdentityVm> identities = null;
            List<Occupation> occupations = null;
            List<Domain.Entities.Guardian> guardians = null;
            List<Domain.Entities.Spouse> spouses = null;

            var memberOrganizations = new List<MemberOrganization>{
                    new MemberOrganization{
                        OrganizationId = request.OrganizationId
                    }
                };

            if (_passBookRepo.PassBookExist(request.PassBookCode))
                throw new DomainException("PassBook Already Exist!");

            var memberPassBooks = new List<MemberPassBook>{
                    new MemberPassBook{
                        PassBook = new PassBook{
                            Code = request.PassBookCode,
                            RegistrationDate = DateTime.Now
                        }
                    }
                };

            // var attachments = JsonConvert.DeserializeObject<AttachmentViewModel>(model.Attachments);

            educations = request.Educations != null ? JsonConvert.DeserializeObject<List<Education>>(request.Educations)
                : null;
            addresses = JsonConvert.DeserializeObject<List<AddressVm>>(request.Addresses);
            identities = JsonConvert.DeserializeObject<List<IdentityVm>>(request.Identity);
            occupations = request.Occupations != null ? JsonConvert.DeserializeObject<List<Occupation>>
                (request.Occupations) : null;

            List<Domain.Entities.Occupation> occupations1 = new List<Domain.Entities.Occupation>();

            var occupationsCount = occupations != null ? occupations.Count() : 0;

            for (int i = 0; i < occupationsCount; i++)
            {
                occupations1.Add(new Domain.Entities.Occupation
                {
                    Name = occupations[i].Name,
                    Company = occupations[i].Company,
                    Income = occupations[i].Income,
                    WorkTypeId = occupations[i].WorkTypeId,
                    Position = occupations[i].Position,
                    IsActive = occupations[i].IsActive
                });
            }

            Domain.Entities.Member member = new Domain.Entities.Member();

            if (request.Type == "Adult")
            {
                spouses = request.Spouses != null ? JsonConvert.DeserializeObject<List<Domain.Entities.Spouse>>(request.Spouses) : null;

                member = new Domain.Entities.Member(request.Title, request.FirstName, request.MiddleName, request.LastName,
                request.MotherName, request.GenderId, request.DOB, memberOrganizations, spouses, educations,
                request.Occupations != null ? occupations1 : null, memberPassBooks, user.Email);
            }
            else
            {
                guardians = request.Guardians != null ? JsonConvert.DeserializeObject<List<Domain.Entities.Guardian>>(request.Guardians)
                    : null;

                member = new Domain.Entities.Member(request.Title, request.FirstName, request.MiddleName, request.LastName,
                request.MotherName, request.GenderId, request.DOB, memberOrganizations, guardians, educations,
                request.Occupations != null ? occupations1 : null, memberPassBooks, user.Email);
            }

            var organizationIdDefinition = await _idDefinitionRepo.
                GetByResourseTypeAndOrganizationId(request.OrganizationId, "Member");

            if (organizationIdDefinition == null)
                throw new DomainException("Organization has no Id Definition! Error on organization.");

            var value = "";

            var digits = organizationIdDefinition.Length - countDigit(organizationIdDefinition.NextValue);

            while (digits > 0)
            {
                value += '0';
                digits--;
            }

            value += organizationIdDefinition.NextValue;

            member.Code = organizationIdDefinition.Prefix + organizationIdDefinition.PrefSep + value +
                organizationIdDefinition.SuffSep + organizationIdDefinition.Suffix;
            member.ProfileImg = request.ProfileImg != null ? await _memberRepo.SavePicture(request.ProfileImg, "Member") :
                null;
            member.Signature = await _memberRepo.SaveSignature(request.Signature);

            organizationIdDefinition.NextValue = organizationIdDefinition.NextValue + 1;

            _logger.LogInformation("------Updating Id Definition Next Value------");

            _idDefinitionRepo.UpdateAsync(organizationIdDefinition);
            await _idDefinitionRepo.UnitOfWork.SaveChanges();

            _logger.LogInformation("------Creating Member------");

            await _memberRepo.AddAsync(member);
            await _memberRepo.UnitOfWork.SaveChanges();

            var objectStateDef = await _objectStateDefnRepo.GetObjectStateDefnByResourseTypeAndName("Member",
                ObjectStateEnumeration.Created.Name);

            var objectState = new Domain.Entities.ObjectState(objectStateDef.Id, objectStateDef.ResourceId, objectStateDef.Name,
                DateTime.Now, member.Id, user.Email);

            _logger.LogInformation("------Tracking New State------");

            await _objectStateRepo.AddAsync(objectState);
            await _objectStateRepo.UnitOfWork.SaveChanges();

            member.LastObjectState = objectState.State;
            member.RegDate = member.CreatedDate;

            _logger.LogInformation("------Updating Member State-------");

            _memberRepo.UpdateAsync(member);
            await _memberRepo.UnitOfWork.SaveChanges();

            for (int i = 0; i < addresses.Count(); i++)
            {
                var addressType = AddressType.List().SingleOrDefault(c => c.Id == addresses[i].AddressType).Name;
                var attribute = AttributeType.List().SingleOrDefault(c => c.Id == addresses[i].Attribute).Name;

                var address = new Domain.Entities.Address("Member", addressType, addresses[i].Value,
                    attribute, member.Id, user.Email);

                _logger.LogInformation("------Adding Member Address-------");

                await _addressRepo.AddAsync(address);
                await _addressRepo.UnitOfWork.SaveChanges();
            }

            for (int i = 0; i < occupationsCount; i++)
            {
                for (int j = 0; j < occupations[i].Addresses.Count(); j++)
                {
                    var addressType = AddressType.List().SingleOrDefault(c => c.Id == occupations[i].Addresses[j].AddressType).Name;
                    var attribute = AttributeType.List().SingleOrDefault(c => c.Id == occupations[i].Addresses[j].Attribute).Name;

                    var address = new Domain.Entities.Address("Occupation", addressType, occupations[i].Addresses[j].Value, attribute,
                        member.Id, user.Email);

                    _logger.LogInformation("------Adding Member Occupations Address-------");

                    await _addressRepo.AddAsync(address);
                    await _addressRepo.UnitOfWork.SaveChanges();
                }
            };

            if (identities != null)
            {
                for (int i = 0; i < identities.Count(); i++)
                {
                    var identityName = IdentityType.List().SingleOrDefault(c => c.Id == identities[i].Type).Name;

                    var identity = new Domain.Entities.Identity("Member", identityName, identities[i].Description,
                        identities[i].Number, member.Id, user.Email);

                    _logger.LogInformation("------Adding Member Identity-------");

                    await _identityRepo.AddAsync(identity);
                    await _identityRepo.UnitOfWork.SaveChanges();
                }
            }

            var lastAccount = await _accountRepo.GetAllAsync();
            int accoutNo = 1000;
            int[] accountTypeIds = { AccountProductType.Loan.Id, AccountProductType.Sharing.Id, AccountProductType.Saving.Id };
            string[] accountTypeDescriptions = {AccountProductType.Loan.Name, AccountProductType.Sharing.Name,
                AccountProductType.Saving.Name};

            if (lastAccount.Count() > 0)
            {
                var acc = lastAccount.OrderBy(c => c.Id).Last();

                try
                {
                    var index = acc.Code.IndexOf('-');

                    if (index != -1)
                        accoutNo = int.Parse(acc.Code.Substring(0, index));

                }
                catch (DomainException)
                {
                    throw new DomainException("Account Creation Failed!");
                }
            }


            for (int i = 0; i < 3; i++)
            {
                var account = new Domain.Entities.Account(++accoutNo + "-" + member.Code, accountTypeDescriptions[i],
                    accountTypeIds[i], new List<Domain.Entities.AccountMap>{
                            new Domain.Entities.AccountMap{
                                Owner = "Member",
                                Reference = member.Id
                            }
                        }, user.Email);

                await _accountRepo.AddAsync(account);
                await _accountRepo.UnitOfWork.SaveChanges();
            }

            return "Created";
        }
    }

    public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
    {
        public CreateMemberCommandValidator()
        {
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

            RuleFor(c => c.MotherName)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().MaximumLength(100).WithMessage("{PropertyName} length not exceed 100 characters");

            RuleFor(c => c.Type)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .Must(IsLetter).WithMessage("{PropertyName} must be letters only")
                .Length(5, 10)
                .NotNull().MaximumLength(10).WithMessage("{PropertyName} length not exceed 10 characters");

            RuleFor(c => c.OrganizationId)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} can not be null");

            RuleFor(c => c.PassBookCode)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} can not be null");

            RuleFor(c => c.Identity)
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .NotNull().WithMessage("{PropertyName} must not be null!")
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

            RuleFor(c => c.Addresses)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} must not be null!")
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
                                    context.AddFailure(AttributeType.Email.Name, AttributeType.Email.Name + " Invalid Email Address!");
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

            RuleFor(c => c.Signature)
                .NotNull().WithMessage("{PropertyName} must be uploaded")
                .ChildRules(c =>
                {
                    c.RuleFor(c => c.ContentType)
                        .Must(IsValidType).WithMessage("{PropertyName} must be image type of jpg or png!");

                    c.RuleFor(c => c.Length)
                        .LessThanOrEqualTo(2097152).WithMessage("{PropertyName} must be less than or equal to 2MB!");
                });

            When(c => c.ProfileImg != null, () =>
            {
                RuleFor(c => c.ProfileImg)
                    .ChildRules(c =>
                    {
                        c.RuleFor(c => c.ContentType)
                            .Must(IsValidType).WithMessage("{PropertyName} must be image type of jpg or png!");

                        c.RuleFor(c => c.Length)
                            .LessThanOrEqualTo(2097152).WithMessage("{PropertyName} must be less than or equal to 2MB!");
                    });
            });

            RuleFor(c => c.GenderId)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} can not be null")
                .InclusiveBetween(1, 3).WithMessage("{PropertyName} with null reference");
        }

        private bool IsLetter(string name)
        {
            return name.All(Char.IsLetter);
        }

        private bool IsValidType(string type)
        {
            return type == "image/png" || type == "image/jpg" || type == "image/jpeg";
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