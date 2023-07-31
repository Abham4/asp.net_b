namespace backend_r.Application.Common.Commands.VoucherReference
{
    public class CreateMemberClosing : IRequest<string>
    {
        public int ClosingPeriodId { get; set; }
        public int MemberId { get; set; }
    }

    public class CreateMemberClosingHandler : IRequestHandler<CreateMemberClosing, string>
    {
        private readonly IVoucherReferenceRepository _voucherReferenceRepo;
        private readonly ILogger<CreateMemberClosingHandler> _logger;

        public CreateMemberClosingHandler(IVoucherReferenceRepository voucherReferenceRepository, 
            ILogger<CreateMemberClosingHandler> logger)
        {
            _logger = logger;
            _voucherReferenceRepo = voucherReferenceRepository;
        }

        public async Task<string> Handle(CreateMemberClosing request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("---------Member Closure-----------");

            await _voucherReferenceRepo.MemberClose(request.ClosingPeriodId, request.MemberId);

            return "Close Done.";
        }
    }

    public class CreateMemberClosingValidator : AbstractValidator<CreateMemberClosing>
    {
        public CreateMemberClosingValidator()
        {
            RuleFor(c => c.ClosingPeriodId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3);

            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}