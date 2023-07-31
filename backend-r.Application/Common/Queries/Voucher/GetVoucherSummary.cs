namespace backend_r.Application.Common.Queries.Voucher
{
    public class GetVoucherSummary : IRequest<List<VoucherSummary>> {}

    public class GetVoucherSummaryHandler : IRequestHandler<GetVoucherSummary, List<VoucherSummary>>
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly ILogger<GetVoucherSummaryHandler> _logger;

        public GetVoucherSummaryHandler(ILogger<GetVoucherSummaryHandler> logger, IVoucherRepository voucherRepository)
        {
            _logger = logger;
            _voucherRepo = voucherRepository;
        }

        public async Task<List<VoucherSummary>> Handle(GetVoucherSummary request, CancellationToken cancellationToken)
        {
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

            var summary = await _voucherRepo.Summary();
            var voucherSummary = new List<VoucherSummary>();
            var counter = 0;

            for (int i = 0; i < 5; i++)
            {
                var t = i * 3;
                var p = t + 3;
                var periods = new List<Vms.Period>();

                for (int j = t; j < p; j++)
                {
                    if(counter > 2)
                        counter = 0;

                    periods.Add(new Vms.Period{
                        PeriodName = PeriodNames[counter],
                        Summary = summary[j]
                    });

                    counter ++;
                }

                voucherSummary.Add(new VoucherSummary{
                    IncomeOutcomeType = CashFlow[i],
                    Periods = periods
                });
            }

            return voucherSummary;
        }
    }
}