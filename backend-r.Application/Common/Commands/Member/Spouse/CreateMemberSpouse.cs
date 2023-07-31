namespace backend_r.Application.Common.Commands.Member.Spouse
{
    public class CreateMemberSpouse : IRequest<string>
    {
        public int MemberId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public DateTime DOB { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMemberSpouseHandler : IRequestHandler<CreateMemberSpouse, string>
    {
        private readonly ISpouseRepository _spouseRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateMemberSpouseHandler> _logger;

        public CreateMemberSpouseHandler(ISpouseRepository spouseRepository, IMemberRepository memberRepository,
            ILogger<CreateMemberSpouseHandler> logger, IUserRepository userRepository)
        {
            _memberRepo = memberRepository;
            _spouseRepo = spouseRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateMemberSpouse request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var member = await _memberRepo.CheckExistence(request.MemberId);

            if (!member)
                throw new DomainException("Related member doesn't exist!");

            var spouse = new Domain.Entities.Spouse(request.Title, request.FirstName, request.MiddleName, request.LastName,
                request.GenderId, request.DOB, request.IsActive, request.MemberId, user.Email);

            _logger.LogInformation("--------Creating member {0} spouse---------", request.MemberId);

            await _spouseRepo.AddAsync(spouse);
            await _spouseRepo.UnitOfWork.SaveChanges();

            return "Created.";
        }
    }

    public class CreateMemberSpouseValidator : AbstractValidator<CreateMemberSpouse>
    {
        public CreateMemberSpouseValidator()
        {
            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.MiddleName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.GenderId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3);
        }
    }
}