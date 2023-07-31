namespace backend_r.Application.Common.Commands.Staff
{
    public class UpdateStaffCommand : IRequest<string>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public DateTime DOB { get; set; }
        public IFormFile ProfileImg { get; set; }
    }

    public class UpdateStaffHandler : IRequestHandler<UpdateStaffCommand, string>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<UpdateStaffHandler> _logger;

        public UpdateStaffHandler(IStaffRepository repository, ILogger<UpdateStaffHandler> logger,
            IMemberRepository memberRepository)
        {
            _staffRepo = repository;
            _logger = logger;
            _memberRepo = memberRepository;
        }

        public async Task<string> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepo.GetByIdAsync(request.Id);

            if (staff == null)
                return null;

            staff.GenderId = Domain.Enumerations.Gender.List().Any(c => c.Id == request.GenderId) && request.GenderId != staff.GenderId ?
                request.GenderId : staff.GenderId;
            staff.DOB = request.DOB != staff.DOB ? request.DOB : staff.DOB;
            staff.Title = request.Title != null ? request.Title : staff.Title;
            staff.FirstName = request.FirstName != null ? request.FirstName : staff.FirstName;
            staff.MiddleName = request.MiddleName != null ? request.MiddleName : staff.MiddleName;
            staff.LastName = request.LastName != null ? request.LastName : staff.LastName;

            if (request.ProfileImg != null)
            {
                _memberRepo.DeleteFile(staff.ProfileImg);
                staff.ProfileImg = await _memberRepo.SavePicture(request.ProfileImg, "Staff");
            }

            _logger.LogInformation("--------Updating Staff----------");

            _staffRepo.UpdateAsync(staff);
            await _staffRepo.UnitOfWork.SaveChanges();

            return "Updated";
        }
    }

    public class UpdateStaffValidator : AbstractValidator<UpdateStaffCommand>
    {
        public UpdateStaffValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }
    }
}