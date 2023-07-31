namespace backend_r.Application.Common.Commands.Member
{
    public class UpdateMemberCommand : IRequest<string>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public int GenderId { get; set; }
        public DateTime DOB { get; set; }
        public IFormFile ProfileImage { get; set; }
        public IFormFile SignatureImage { get; set; }
    }

    class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, string>
    {
        private readonly IMemberRepository _repo;
        private readonly ILogger<UpdateMemberCommandHandler> _logger;

        public UpdateMemberCommandHandler(IMemberRepository repository, ILogger<UpdateMemberCommandHandler> logger)
        {
            _logger = logger;
            _repo = repository;
        }

        public async Task<string> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
        {
            var member = await _repo.GetByIdAsync(request.Id);

            if (member == null)
                return null;

            member.Title = request.Title != null ? request.Title : member.Title;
            member.FirstName = request.FirstName != null ? request.FirstName : member.FirstName;
            member.MiddleName = request.MiddleName != null ? request.MiddleName : member.MiddleName;
            member.LastName = request.LastName != null ? request.LastName : member.LastName;
            member.MotherName = request.MotherName != null ? request.MotherName : member.MotherName;
            member.GenderId = request.GenderId != 0 ? request.GenderId : member.GenderId;
            member.DOB = request.DOB != member.DOB ? request.DOB : member.DOB;

            if(request.ProfileImage != null)
            {
                _repo.DeleteFile(member.ProfileImg);
                member.ProfileImg = await _repo.SavePicture(request.ProfileImage, "Member");
            }

            if(request.SignatureImage != null)
            {
                _repo.DeleteFile(member.Signature);
                member.Signature = await _repo.SaveSignature(request.SignatureImage);
            }            

            _logger.LogInformation("------Updating Member-------");

            _repo.UpdateAsync(member);
            await _repo.UnitOfWork.SaveChanges();
            
            return "Updated";
        }
    }

    public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
    {
        public UpdateMemberCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} can'be null");
        }
    }
}