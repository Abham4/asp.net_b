namespace backend_r.Application.Common.Queries.Voucher
{
    public class GetVoucherSummaryforHighers : IRequest<List<VoucherSummaryforHighers>> {}

    public class GetVoucherSummaryforHighersHandler : IRequestHandler<GetVoucherSummaryforHighers,
        List<VoucherSummaryforHighers>>
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly IOrganizationRepository _organizationRepo;
        private readonly ILogger<GetVoucherSummaryforHighersHandler> _logger;

        public GetVoucherSummaryforHighersHandler(IVoucherRepository voucherRepository,
            ILogger<GetVoucherSummaryforHighersHandler> logger, IOrganizationRepository organizationRepository)
        {
            _voucherRepo = voucherRepository;
            _organizationRepo = organizationRepository;
            _logger = logger;
        }

        public async Task<List<VoucherSummaryforHighers>> Handle(GetVoucherSummaryforHighers request,
            CancellationToken cancellationToken)
        {
            var organizations = await _organizationRepo.GetAllAsync();
            var voucherSummaryforHighers  = new List<VoucherSummaryforHighers>();

            var CashFlow = new []
            {
                "Deposit",
                "Disbursement",
                "Repayment",
                "Share_Payment",
                "Withdrawal",
            };

            var PeriodNames = new []
            {
                "Day",
                "Week",
                "Month",
            };

            foreach (var organization in organizations)
            {
                var summaries = await _voucherRepo.VouchersByVoucherTypeforHighers(organization.Name);
                var voucherSummaries = new List<VoucherSummary>();

                for (int i = 0; i < summaries.Count(); i++)
                {
                    var voucherSummary = new VoucherSummary();
                    var periods = new List<Vms.Period>();

                    voucherSummary.IncomeOutcomeType = CashFlow[i];

                    if (summaries[i][0] != 0 || summaries[i][1] != 0 || summaries[i][2] != 0)
                        for (int j = 0; j < 3; j++)
                        {
                            periods.Add(new Vms.Period{
                                PeriodName = PeriodNames[j],
                                Summary = summaries[i][j]
                            });
                        }

                    voucherSummary.Periods = periods;
                    voucherSummaries.Add(voucherSummary);
                }

                voucherSummaryforHighers.Add(new VoucherSummaryforHighers{
                    Organization = organization.Name,
                    VoucherSummaries = voucherSummaries
                });
            }

            _logger.LogInformation("--------Getting Vouchers for Highers----------");

            return voucherSummaryforHighers;
        }
    }
}