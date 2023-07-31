namespace backend_r.Application.Common.Commands.Member.Guardian
{
    public class UpdateMemberGuardian : IRequest<string>
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    public class UpdateMemberGuardianHandler : IRequestHandler<UpdateMemberGuardian, string>
    {
        private readonly IGuardianRepository _guardianRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<UpdateMemberGuardianHandler> _logger;

        public UpdateMemberGuardianHandler(IGuardianRepository guardianRepository, IMemberRepository memberRepository,
            ILogger<UpdateMemberGuardianHandler> logger)
        {
            _guardianRepo = guardianRepository;
            _memberRepo = memberRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateMemberGuardian request, CancellationToken cancellationToken)
        {
            var guardian = await _guardianRepo.GetByIdAsync(request.Id);
            var member = await _memberRepo.CheckExistence(request.MemberId);

            if (guardian == null)
                return null;

            if (!member)
                throw new DomainException("Related member doesn't exist!");

            guardian.FirstName = request.FirstName != null ? request.FirstName : guardian.FirstName;
            guardian.MiddleName = request.MiddleName != null ? request.MiddleName : guardian.MiddleName;
            guardian.LastName = request.LastName != null ? request.LastName : guardian.LastName;

            _logger.LogInformation("-------Updating member {0} guardian----------", request.MemberId);

            _guardianRepo.UpdateAsync(guardian);
            await _guardianRepo.UnitOfWork.SaveChanges();

            return "Updated.";
        }
    }

    public class UpdateMemberGuardianValidator : AbstractValidator<UpdateMemberGuardian>
    {
        public UpdateMemberGuardianValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName}  can't be null!");

            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName}  can't be null!");
        }
    }
}