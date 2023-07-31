namespace backend_r.Application.Common.Queries.Organization
{
    public class GetMemberCarrersForHigers : IRequest<List<MemberCarrersForHigersVm>> {}

    public class GetMemberCarrersForHigersHandler : IRequestHandler<GetMemberCarrersForHigers, List<MemberCarrersForHigersVm>>
    {
        private readonly IOrganizationRepository _organizationRepo;
        private readonly ILogger<GetMemberCarrersForHigersHandler> _logger;

        public GetMemberCarrersForHigersHandler(IOrganizationRepository organizationRepository,
            ILogger<GetMemberCarrersForHigersHandler> logger)
        {
            _organizationRepo = organizationRepository;
            _logger = logger;
        }
        
        public async Task<List<MemberCarrersForHigersVm>> Handle(GetMemberCarrersForHigers request, CancellationToken cancellationToken)
        {
            var organizations = await _organizationRepo.ListofMembersOccupations();
            var memberCarrers = new List<MemberCarrersForHigersVm>();
            var organizationNames = new HashSet<string>();
            
            foreach (var organization in organizations)
            {
                organizationNames.Add(organization.Name);
            }

            foreach (var organizationName in organizationNames)
            {
                var memberCarrersForHigers = new List<MemberCarrersVm>();
                organizations.ForEach(organization => {
                    if(organization.Name == organizationName)
                    {
                        int privateWorkers = 0;
                        int govtWorkers = 0;
                        int nonGovtWorkers = 0;

                        organization.MemberOrganizations.ForEach(memOrg => {
                            if(memOrg.Member.Occupations.Count() > 0)
                                if(memOrg.Member.Occupations[0].WorkTypeId == WorkType.Private.Id)
                                    privateWorkers ++;
                                else if (memOrg.Member.Occupations[0].WorkTypeId == WorkType.Governmental.Id)
                                    govtWorkers++;
                                else if (memOrg.Member.Occupations[0].WorkTypeId == WorkType.Non_Governmental.Id)
                                    nonGovtWorkers++;
                        });

                        var numbers = new [] { privateWorkers, govtWorkers, nonGovtWorkers };

                        if(privateWorkers == 1 || govtWorkers == 1 || nonGovtWorkers == 1)
                            for (int i = 0; i < 3; i++)
                            {
                                memberCarrersForHigers.Add(new MemberCarrersVm{
                                    WorkType = WorkType.List().ToArray()[i].Name,
                                    Numbers = numbers[i]
                                });
                            }
                    }
                });

                memberCarrers.Add(new MemberCarrersForHigersVm{
                    Organization = organizationName,
                    MemberCarrers = memberCarrersForHigers
                });
            }

            _logger.LogInformation("-------Getting Member Carrers for Highers---------");

            return memberCarrers;
        }
    }
}