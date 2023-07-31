namespace backend_r.Application.Common.Commands.VoucherReference
{
    public class CreateOrganizationClosing : IRequest<string>
    {
        public int ClosingPeriodId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class CreateOrganizationClosingHandler : IRequestHandler<CreateOrganizationClosing, string>
    {
        private readonly IVoucherReferenceRepository _voucherReferenceRepo;
        private readonly ILogger<CreateOrganizationClosingHandler> _logger;

        public CreateOrganizationClosingHandler(ILogger<CreateOrganizationClosingHandler> logger,
            IVoucherReferenceRepository voucherReferenceRepository)
        {
            _logger = logger;
            _voucherReferenceRepo = voucherReferenceRepository;
        }

        public async Task<string> Handle(CreateOrganizationClosing request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("------------Organizational Closure--------------");

            await _voucherReferenceRepo.OrganizationalClose(request.ClosingPeriodId, request.OrganizationId);

            return "Organizational Close Done.";
        }
    }

    public class CreateOrganizationClosingValidator : AbstractValidator<CreateOrganizationClosing>
    {
        public CreateOrganizationClosingValidator()
        {
            RuleFor(c => c.ClosingPeriodId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3);
            
            RuleFor(c => c.OrganizationId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}