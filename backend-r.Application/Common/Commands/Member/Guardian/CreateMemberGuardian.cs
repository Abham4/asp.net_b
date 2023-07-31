namespace backend_r.Application.Common.Commands.Member.Guardian
{
    public class CreateMemberGuardian : IRequest<string>
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    public class CreateMemberGuardianHandler : IRequestHandler<CreateMemberGuardian, string>
    {
        private readonly IGuardianRepository _guardianRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateMemberGuardianHandler> _logger;

        public CreateMemberGuardianHandler(IGuardianRepository guardianRepository, IMemberRepository memberRepository,
            ILogger<CreateMemberGuardianHandler> logger, IUserRepository userRepository)
        {
            _guardianRepo = guardianRepository;
            _memberRepo = memberRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateMemberGuardian request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var member = await _memberRepo.GetByIdAsync(request.MemberId);

            if (member == null)
                throw new DomainException("Related member doesn't exist!");

            if (member.Spouses.Count() > 0)
                throw new DomainException("Already have spouse can't have guardian!");

            var guardian = new Domain.Entities.Guardian(request.FirstName, request.MiddleName, request.LastName, request.MemberId,
                user.Email);

            _logger.LogInformation("---------Creating member {0} guardian-----------", request.MemberId);

            await _guardianRepo.AddAsync(guardian);
            await _guardianRepo.UnitOfWork.SaveChanges();

            return "Created.";
        }
    }

    public class CreateMemberGuardianValidator : AbstractValidator<CreateMemberGuardian>
    {
        public CreateMemberGuardianValidator()
        {
            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.MiddleName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}