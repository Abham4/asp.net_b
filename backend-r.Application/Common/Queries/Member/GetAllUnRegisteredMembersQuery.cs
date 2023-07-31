namespace backend_r.Application.Common.Queries.Member
{
    public class GetAllUnRegisteredMembersQuery : IRequest<IEnumerable<AllMemberVm>> { }

    public class GetAllUnRegisteredMembersQueryHandler : IRequestHandler<GetAllUnRegisteredMembersQuery,
        IEnumerable<AllMemberVm>>
    {
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<GetAllUnRegisteredMembersQueryHandler> _logger;

        public GetAllUnRegisteredMembersQueryHandler(IMemberRepository memberRepository,
            ILogger<GetAllUnRegisteredMembersQueryHandler> logger)
        {
            _memberRepo = memberRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllMemberVm>> Handle(GetAllUnRegisteredMembersQuery request,
            CancellationToken cancellationToken)
        {
            var unRegisteredMembers = await _memberRepo.GetUnRegisteredMembers();

            _logger.LogInformation("------Getting UnRegistered Members----------");

            return unRegisteredMembers.Select(c => new AllMemberVm
            {
                Id = c.Id,
                Title = c.Title,
                Code = c.Code,
                FirstName = c.FirstName,
                MiddleName = c.MiddleName,
                LastName = c.LastName,
                PassbookCode = c.MemberPassBooks[0].PassBook.Code,
                MotherName = c.MotherName,
                RegDate = c.RegDate,
                LastObjectState = c.LastObjectState,
                GenderName = c.Gender.Name,
                DOB = c.DOB,
                ProfileImg = c.ProfileImg,
                Signature = c.Signature
            });
        }
    }
}