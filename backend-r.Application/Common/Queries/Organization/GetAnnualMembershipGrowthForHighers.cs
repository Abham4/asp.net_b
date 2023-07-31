namespace backend_r.Application.Common.Queries.Organization
{
    public class GetAnnualMembershipGrowthForHighers : IRequest<List<MemberShipForHighersVm>> {}

    public class GetAnnualMembershipGrowthForHighersHandler : IRequestHandler<GetAnnualMembershipGrowthForHighers,
        List<MemberShipForHighersVm>>
    {
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<GetAnnualMembershipGrowthForHighersHandler> _logger;

        public GetAnnualMembershipGrowthForHighersHandler(IOrganizationRepository organizationRepository,
            ILogger<GetAnnualMembershipGrowthForHighersHandler> logger, IMemberRepository memberRepository)
        {
            _logger = logger;
            _organizationRepo = organizationRepository;
            _memberRepo = memberRepository;
        }

        public async Task<List<MemberShipForHighersVm>> Handle(GetAnnualMembershipGrowthForHighers request,
            CancellationToken cancellationToken)
        {
            var organizations = await _organizationRepo.ListofMembers();
            var memberShipFors = new List<MemberShipForHighersVm>();
            var organizationNames = new HashSet<string>();
            var years = new HashSet<int>();

            foreach (var organization in organizations)
            {
                organizationNames.Add(organization.Name);
                organization.MemberOrganizations.ForEach(c => {
                    years.Add(c.Member.CreatedDate.Year);
                });
            }

            foreach (var organizationName in organizationNames)
            {
                var fullData = new List<FullData>();
                
                organizations.ForEach(organization => {
                    if(organization.Name == organizationName)
                    {   
                        fullData = new List<FullData>();

                        foreach (var year in years)
                        {
                            int female = 0;
                            int male = 0;
                            int notSpecified = 0;

                            organization.MemberOrganizations.ForEach(c => {
                                if(c.Member.CreatedDate.Year == year)
                                {
                                    if(c.Member.GenderId == Domain.Enumerations.Gender.Female.Id)
                                        female++;
                                    if (c.Member.GenderId == Domain.Enumerations.Gender.Male.Id)
                                        male++;
                                    if (c.Member.GenderId == Domain.Enumerations.Gender.Not_Specifed.Id)
                                        notSpecified++;
                                }
                            });

                            if(male != 0 || female != 0 || notSpecified != 0)
                            {
                                var data = new List<Data>();
                                var genders = new [] { male, female, notSpecified };

                                for (int i = 0; i < 3; i++)
                                {
                                    data.Add(new Data{
                                        Gender = Domain.Enumerations.Gender.List().ToArray()[i].Name,
                                        Count = genders[i]
                                    });
                                }

                                fullData.Add(new FullData{
                                    Year = year,
                                    Datas = data
                                });
                            }
                        }
                    }
                });

                _logger.LogInformation("----------Getting Annual Membership Growth for Highers-----------");

                memberShipFors.Add(new MemberShipForHighersVm{
                    Organization = organizationName,
                    FullDatas = fullData
                });
            }

            return memberShipFors;
        }
    }
}