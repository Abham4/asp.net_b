namespace backend_r.Application.Common.Commands.Member.Spouse
{
    public class UpdateMemberSpouse : IRequest<string>
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public DateTime DOB { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateMemberSpouseHandler : IRequestHandler<UpdateMemberSpouse, string>
    {
        private readonly ISpouseRepository _spouseRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<UpdateMemberSpouseHandler> _logger;

        public UpdateMemberSpouseHandler(ISpouseRepository spouseRepository, IMemberRepository memberRepository,
            ILogger<UpdateMemberSpouseHandler> logger)
        {
            _memberRepo = memberRepository;
            _spouseRepo = spouseRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateMemberSpouse request, CancellationToken cancellationToken)
        {
            var spouse = await _spouseRepo.GetByIdAsync(request.Id);
            var member = await _memberRepo.CheckExistence(request.MemberId);

            if (spouse == null)
                return null;

            if (!member)
                throw new DomainException("Related member doesn't exist!");

            spouse.Title = request.Title != null ? request.Title : spouse.Title;
            spouse.FirstName = request.FirstName != null ? request.FirstName : spouse.FirstName;
            spouse.MiddleName = request.MiddleName != null ? request.MiddleName : spouse.MiddleName;
            spouse.LastName = request.LastName != null ? request.LastName : spouse.LastName;
            spouse.GenderId = request.GenderId;
            spouse.IsActive = request.IsActive;

            _logger.LogInformation("--------Updating member {0} spouse---------", request.MemberId);

            _spouseRepo.UpdateAsync(spouse);
            await _spouseRepo.UnitOfWork.SaveChanges();

            return "Updated.";
        }
    }

    public class UpdateMemberSpouseValidator : AbstractValidator<UpdateMemberSpouse>
    {
        public UpdateMemberSpouseValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.GenderId)
                .InclusiveBetween(1, 3);
        }
    }
}