namespace backend_r.Application.Common.Commands.Member.Identity
{
    public class UpdateMemberIdentity : IRequest<string>
    {
        public int Id { get; set; }
        public int Reference { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }

    public class UpdateMemberIdentityHandler : IRequestHandler<UpdateMemberIdentity, string>
    {
        private readonly IIdentityRepository _identityRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<UpdateMemberIdentityHandler> _logger;

        public UpdateMemberIdentityHandler(IIdentityRepository identityRepository, IMemberRepository memberRepository,
            ILogger<UpdateMemberIdentityHandler> logger)
        {
            _identityRepo = identityRepository;
            _memberRepo = memberRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateMemberIdentity request, CancellationToken cancellationToken)
        {
            var identity = await _identityRepo.GetByIdAsync(request.Id);
            var member = await _memberRepo.CheckExistence(request.Reference);

            if (identity == null)
                return null;

            if (!member)
                throw new DomainException("Related member doesn't exist!");

            identity.Type = request.Type != null ? request.Type : identity.Type;
            identity.Description = request.Description != null ? request.Description : identity.Description;
            identity.Number = request.Number != null ? request.Number : identity.Number;

            _logger.LogInformation("--------Updating member {0} identity------------", request.Reference);

            _identityRepo.UpdateAsync(identity);
            await _identityRepo.UnitOfWork.SaveChanges();

            return "Updated.";
        }
    }

    public class UpdateMemberIdentityValidator : AbstractValidator<UpdateMemberIdentity>
    {
        public UpdateMemberIdentityValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Reference)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}