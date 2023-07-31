namespace backend_r.Application.Common.Queries.Member
{
    public class GetMemberCarrersQuery : IRequest<List<MemberCarrersVm>> {}

    public class GetMemberCarrersQueryHanlder : IRequestHandler<GetMemberCarrersQuery, List<MemberCarrersVm>>
    {
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<GetMemberCarrersQueryHanlder> _logger;

        public GetMemberCarrersQueryHanlder(ILogger<GetMemberCarrersQueryHanlder> logger, IMemberRepository memberRepository)
        {
            _logger = logger;
            _memberRepo = memberRepository;
        }

        public async Task<List<MemberCarrersVm>> Handle(GetMemberCarrersQuery request, CancellationToken cancellationToken)
        {
            var memberCarrers = new List<MemberCarrersVm>();
            var numberOfCarrers = await _memberRepo.GetMemberCarrers();

            _logger.LogInformation("---------Getting Member Carrers Data---------");
            
            for (int i = 0; i < numberOfCarrers.Count(); i++)
            {
                memberCarrers.Add(new MemberCarrersVm{
                    WorkType = WorkType.List().ToArray()[i].Name,
                    Numbers = numberOfCarrers[i]
                });
            }

            return memberCarrers;
        }
    }
}