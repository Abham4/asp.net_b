namespace backend_r.Application.Common.Queries.Member
{
    public class GetAnnualMembershipGrowth : IRequest<List<MemberShipVm>> {}

    public class GetAnnualMembershipGrowthHandler : IRequestHandler<GetAnnualMembershipGrowth, List<MemberShipVm>>
    {
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<GetAnnualMembershipGrowthHandler> _logger;

        public GetAnnualMembershipGrowthHandler(IMemberRepository memberRepository, ILogger<GetAnnualMembershipGrowthHandler> logger)
        {
            _memberRepo = memberRepository;
            _logger = logger;
        }

        public async Task<List<MemberShipVm>> Handle(GetAnnualMembershipGrowth request, CancellationToken cancellationToken)
        {
            var members = await _memberRepo.GetAllAsync();
            var years = new HashSet<int>();
            var memberShips  = new List<MemberShipVm>();
            
            foreach (var member in members)
            {
                years.Add(member.CreatedDate.Year);
            }

            foreach (var year in years)
            {
                var membersByYear = await _memberRepo.GetMembersByYear(year);

                var genderCounts = new [] { membersByYear.Where(c => c.Gender.Name == Domain.Enumerations.Gender.Male.Name)
                    .Count(), membersByYear.Where(c => c.Gender.Name == Domain.Enumerations.Gender.Female.Name).Count(),
                    membersByYear.Where(c => c.Gender.Name == Domain.Enumerations.Gender.Not_Specifed.Name).Count() };

                var datas = new List<Data>();

                for (int i = 0; i < genderCounts.Count(); i++)
                {
                    datas.Add(new Data{
                        Gender = Domain.Enumerations.Gender.List().ToArray()[i].Name,
                        Count = genderCounts[i]
                    });
                }
                
                memberShips.Add(new MemberShipVm{
                    Year = year,
                    Datas = datas
                });
            }

            _logger.LogInformation("-------Getting Membership Annual Growth--------");

            return memberShips;
        }
    }
}