namespace backend_r.Application.Common.Commands.ObjectState
{
    public class ChangeToActiveCommand : IRequest<string>
    {
        public int MemberId { get; set; }
    }

    class ChangeToActiveHandler : IRequestHandler<ChangeToActiveCommand, string>
    {
        private readonly IObjectStateRepository _objectStateRepo;
        private readonly ILogger<ChangeToActiveHandler> _logger;
        private readonly IMemberRepository _memberRepo;

        public ChangeToActiveHandler(ILogger<ChangeToActiveHandler> logger, 
            IObjectStateRepository objectStateRepository, IMemberRepository memberRepository)
        {
            _logger = logger;
            _objectStateRepo = objectStateRepository;
            _memberRepo = memberRepository;
        }

        public async Task<string> Handle(ChangeToActiveCommand request, CancellationToken cancellationToken)
        {   
            var member = await _memberRepo.GetByIdAsync(request.MemberId);
            
            if (member == null)
                return null;

            _logger.LogInformation("-------Changing member {0} state to active------", request.MemberId);

            var objectState = await _objectStateRepo.ToActive(request.MemberId);

            return objectState;
        }
    }

    class ChangeToActiveValidator : AbstractValidator<ChangeToActiveCommand>
    {
        public ChangeToActiveValidator()
        {
            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }
    }
}