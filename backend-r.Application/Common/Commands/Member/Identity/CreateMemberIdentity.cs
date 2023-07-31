namespace backend_r.Application.Common.Commands.Member.Identity
{
    public class CreateMemberIdentity : IRequest<string>
    {
        public int Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public int MemberId { get; set; }
    }

    public class CreateMemberIdentityHandler : IRequestHandler<CreateMemberIdentity, string>
    {
        private readonly IIdentityRepository _identityRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateMemberIdentityHandler> _logger;

        public CreateMemberIdentityHandler(IIdentityRepository identityRepository, IMemberRepository memberRepository,
            ILogger<CreateMemberIdentityHandler> logger, IUserRepository userRepository)
        {
            _identityRepo = identityRepository;
            _memberRepo = memberRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateMemberIdentity request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var member = await _memberRepo.CheckExistence(request.MemberId);

            if (!member)
                throw new DomainException("Related member doesn't exist!");

            var type = IdentityType.List().SingleOrDefault(c => c.Id == request.Type).Name;

            var identity = new Domain.Entities.Identity("Member", type, request.Description, request.Number,
                request.MemberId, user.Email);

            _logger.LogInformation("--------Creating new identity for a member {0}--------------", request.MemberId);

            await _identityRepo.AddAsync(identity);
            await _identityRepo.UnitOfWork.SaveChanges();

            return "Created.";
        }

        public class CreateMemberIdentityValidator : AbstractValidator<CreateMemberIdentity>
        {
            public CreateMemberIdentityValidator()
            {
                RuleFor(c => c.Type)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .InclusiveBetween(1, 4).WithMessage("{PropertyName} is null reference!");

                RuleFor(c => c.Description)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");

                RuleFor(c => c.Number)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");

                RuleFor(c => c.MemberId)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
            }
        }
    }
}