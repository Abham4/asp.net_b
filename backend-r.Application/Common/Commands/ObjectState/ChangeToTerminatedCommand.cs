namespace backend_r.Application.Common.Commands.ObjectState
{
    public class ChangeToTerminatedCommand : IRequest<string>
    {
        public int MemberId { get; set; }
    }

    class ChangeToTerminatedHandler : IRequestHandler<ChangeToTerminatedCommand, string>
    {
        private readonly IObjectStateRepository _objectStateRepo;
        private readonly ILogger<ChangeToActiveHandler> _logger;
        private readonly IMemberRepository _memberRepo;

        public ChangeToTerminatedHandler(ILogger<ChangeToActiveHandler> logger, 
            IObjectStateRepository objectStateRepository, IMemberRepository memberRepository)
        {
            _logger = logger;
            _objectStateRepo = objectStateRepository;
            _memberRepo = memberRepository;
        }

        public async Task<string> Handle(ChangeToTerminatedCommand request, CancellationToken cancellationToken)
        {   
            var member = await _memberRepo.GetByIdAsync(request.MemberId);
            
            if (member == null)
                return null;

            _logger.LogInformation("-------Changing member {0} state to terminated------", request.MemberId);

            var objectState = await _objectStateRepo.ToTerminate(request.MemberId);

            return objectState;
        }
    }

    class ChangeToTerminatedValidator : AbstractValidator<ChangeToTerminatedCommand>
    {
        public ChangeToTerminatedValidator()
        {
            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }
    }
}