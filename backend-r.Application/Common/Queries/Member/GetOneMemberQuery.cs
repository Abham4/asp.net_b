namespace backend_r.Application.Common.Queries.Member
{
    public class GetOneMemberQuery : IRequest<OneMemberVm>
    {
        public int Id { get; set; }
    }

    public class GetOneMemberHandler : IRequestHandler<GetOneMemberQuery, OneMemberVm>
    {
        private readonly IMemberRepository _memberRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IIdentityRepository _identityRepo;
        private readonly IAccountMapRepository _accountMapRepo;
        private readonly ILogger<GetOneMemberHandler> _logger;

        public GetOneMemberHandler(IAddressRepository addressRepository, IIdentityRepository identityRepository
        , IMemberRepository repository, ILogger<GetOneMemberHandler> logger, IAccountMapRepository accountMapRepository)
        {
            _addressRepo = addressRepository;
            _identityRepo = identityRepository;
            _memberRepo = repository;
            _accountMapRepo = accountMapRepository;
            _logger = logger;
        }
        public async Task<OneMemberVm> Handle(GetOneMemberQuery request, CancellationToken cancellationToken)
        {
            var member = await _memberRepo.GetByIdAsync(request.Id);

            if(member == null)
                return null;

            var addresses = await _addressRepo.GetByReferenceAndOwnerType(member.Id, "Member");
            var occupationAddresses = await _addressRepo.GetByReferenceAndOwnerType(member.Id, "Occupation");
            var identites = await _identityRepo.GetByReferenceAndOwnerType(member.Id, "Member");
            var accountMaps = await _accountMapRepo.GetAccountMapByReferenceAndOwner(member.Id, "Member");

            var oneMember = new OneMemberVm
            {
                Id = member.Id,
                Title = member.Title,
                Code = member.Code,
                FirstName = member.FirstName,
                MiddleName = member.MiddleName,
                LastName = member.LastName,
                PassbookCode = member.MemberPassBooks[0].PassBook.Code,
                MotherName = member.MotherName,
                RegDate = member.CreatedDate,
                LastObjectState = member.LastObjectState,
                Gender = new Vms.Gender{
                    Id = member.Gender.Id,
                    Name = member.Gender.Name
                },
                DOB = member.DOB,
                ProfileImg = member.ProfileImg,
                Signature = member.Signature,
                Organization = new OneOrganizationVm{
                    Id = member.MemberOrganizations[0].Organization.Id,
                    Name = member.MemberOrganizations[0].Organization.Name
                },
                Addresses = addresses.Select(x => new AddressVm
                {
                    Id = x.Id,
                    Reference = x.Reference,
                    AddressType = x.AddressType,
                    Attribute = x.Attribute,
                    Value = x.Value
                }).ToList(),
                Identities = identites.Select(x => new IdentityVm
                {
                    Id = x.Id,
                    Type = x.Type,
                    Description = x.Description,
                    Number = x.Number,
                    Reference = x.Reference
                }).ToList(),
                Occupations = member.Occupations.Count() > 0 ? member.Occupations.Select(x => new OccupationVm
                {
                    Name = x.Name,
                    Company = x.Company,
                    Position = x.Position,
                    Income = x.Income,
                    WorkTypeName = x.WorkType.Name,
                    IsActive = x.IsActive,
                    Addresses = occupationAddresses.Select(y => new AddressVm
                    {
                        AddressType = y.AddressType,
                        Attribute = y.Attribute,
                        Value = y.Value
                    }).ToList()
                }).ToList() : null,
                Educations = member.Educations.Count() > 0 ? member.Educations.Select(x => new EducationVm
                {
                    Level = x.Level,
                    Institution = x.Institution,
                    FieldOfStudy = x.FieldOfStudy,
                    IsActive = x.IsActive,
                    CreatedDate = x.CreatedDate
                }).ToList() : null,
                Spouses = member.Spouses.Count() > 0 ? member.Spouses.Select(x => new SpouseVm
                {
                    Id = x.Id,
                    Title = x.Title,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Gender = new Vms.Gender{
                        Id = x.Gender.Id,
                        Name = x.Gender.Name
                    },
                    RegDate = x.CreatedDate,
                    IsActive = x.IsActive,
                    DOB = x.DOB
                }).ToList() : null,
                Guardians = member.Guardians.Count() > 0 ? member.Guardians.Select(x => new GuardianVm
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Type = x.Type
                }).ToList() : null,
                AccountMaps = accountMaps.Select(c => new Vms.AccountMap{
                    Accounts = new List<Vms.Account>{
                        new Vms.Account{
                            Id = c.Id,
                            Code = c.Account.Code,
                            Description = c.Account.Description,
                            ControlAccount = c.Account.ControlAccount,
                            AccountProductTypeName = c.Account.AccountProductType.Name,
                            CreatedDate = c.Account.CreatedDate
                        }
                    }
                }).ToList()
            };

            _logger.LogInformation("------Getting a member-------");
            
            return oneMember;
        }
    }
}