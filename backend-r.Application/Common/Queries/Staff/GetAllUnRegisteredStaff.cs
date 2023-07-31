namespace backend_r.Application.Common.Queries.Staff
{
    public class GetAllUnRegisteredStaff : IRequest<IEnumerable<AllStaffVm>> {}

    public class GetAllUnRegisteredStaffHandler : IRequestHandler<GetAllUnRegisteredStaff, IEnumerable<AllStaffVm>>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly ILogger<GetAllUnRegisteredStaffHandler> _logger;

        public GetAllUnRegisteredStaffHandler(IStaffRepository staffRepository,
            ILogger<GetAllUnRegisteredStaffHandler> logger)
        {
            _logger = logger;
            _staffRepo = staffRepository;
        }

        public async Task<IEnumerable<AllStaffVm>> Handle(GetAllUnRegisteredStaff request,
            CancellationToken cancellationToken)
        {
            var unRegisteredStaffs = await _staffRepo.UnRegisteredStaffs();

            _logger.LogInformation("--------Getting UnRegistered Staffs-----------");

            return unRegisteredStaffs.Select(c => new AllStaffVm
            {
                Id = c.Id,
                Title = c.Title,
                FirstName = c.FirstName,
                MiddleName = c.MiddleName,
                LastName = c.LastName,
                GenderName = c.Gender.Name,
                EmpDate = c.EmpDate,
                DOB = c.DOB,
                LastObjectState = c.LastObjectState,
            });
        }
    }
}