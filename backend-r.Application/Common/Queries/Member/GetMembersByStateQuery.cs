namespace backend_r.Application.Common.Queries.Member
{
    public class GetMembersByStateQuery : IRequest<IEnumerable<AllMemberVm>>
    {
        public string State { get; set; }
    }

    public class GetMembersByStateQueryHandler : IRequestHandler<GetMembersByStateQuery, IEnumerable<AllMemberVm>>
    {
        private readonly IMemberRepository _repo;
        private readonly ILogger<GetMembersByStateQueryHandler> _logger;

        public GetMembersByStateQueryHandler(IMemberRepository repository, ILogger<GetMembersByStateQueryHandler> logger)
        {
            _logger = logger;
            _repo = repository;
        }

        public async Task<IEnumerable<AllMemberVm>> Handle(GetMembersByStateQuery request, CancellationToken cancellationToken)
        {
            var members = await _repo.GetMembersByState(request.State);

            _logger.LogInformation("------Getting members by their state--------");
            
            return members.Select(x => new AllMemberVm
            {
                Id = x.Id,
                Title = x.Title,
                Code = x.Code,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                MotherName = x.MotherName,
                RegDate = x.RegDate,
                LastObjectState = x.LastObjectState,
                GenderName = x.Gender.Name,
                DOB = x.DOB,
                ProfileImg = x.ProfileImg,
                Signature = x.Signature
            });
        }
    }

    public class GetMembersByStateQueryValidator : AbstractValidator<GetMembersByStateQuery>
    {
        public GetMembersByStateQueryValidator()
        {
            RuleFor(c => c.State)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .Must(IsLetter).WithMessage("{PropertyName} must be letters only")
                .NotNull().MaximumLength(50).WithMessage("Length not exceed 50 characters");
        }

        private bool IsLetter(string name)
        {
            return name.All(Char.IsLetter);
        }
    }
}