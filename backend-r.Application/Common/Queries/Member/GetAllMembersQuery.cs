namespace backend_r.Application.Common.Queries.Member
{
    public class GetAllMembersQuery : IRequest<IEnumerable<AllMemberVm>> {}

    public class GetAllMembersHandler : IRequestHandler<GetAllMembersQuery, IEnumerable<AllMemberVm>>
    {
        private readonly IMemberRepository _repo;
        private readonly IAddressRepository _addressRepo;
        private readonly ILogger<GetAllMembersHandler> _logger;

        public GetAllMembersHandler(IMemberRepository repository, ILogger<GetAllMembersHandler> logger,
            IAddressRepository addressRepository)
        {
            _repo = repository;
            _logger = logger;
            _addressRepo = addressRepository;
        }

        public async Task<IEnumerable<AllMemberVm>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
        {
            var members = await _repo.GetAllAsync();
            var membersToReturn = new List<AllMemberVm>();

            foreach (var member in members)
            {
                var memberAddresses = await _addressRepo.GetByReferenceAndOwnerType(member.Id, "Member");
                
                var memberToReturn = new AllMemberVm()
                {
                    Id = member.Id,
                    Title = member.Title,
                    Code = member.Code,
                    FirstName = member.FirstName,
                    MiddleName = member.MiddleName,
                    LastName = member.LastName,
                    PassbookCode = member.MemberPassBooks[0].PassBook.Code,
                    MotherName = member.MotherName,
                    RegDate = member.RegDate,
                    LastObjectState = member.LastObjectState,
                    GenderName = member.Gender.Name,
                    DOB = member.DOB,
                    ProfileImg = member.ProfileImg,
                    Signature = member.Signature,
                    Organization = new OneOrganizationVm
                    {
                        Id = member.MemberOrganizations[0].Organization.Id,
                        Name = member.MemberOrganizations[0].Organization.Name
                    },
                    Addresses = memberAddresses.Select(c => new AddressVm{
                            AddressType = c.AddressType,
                            Attribute = c.Attribute,
                            Value = c.Value
                        }).ToList()
                };

                membersToReturn.Add(memberToReturn);
            }

            _logger.LogInformation("------Getting all members-------");

            return membersToReturn;
        }
    }
}