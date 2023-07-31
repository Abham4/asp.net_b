namespace backend_r.Application.Common.Queries.Staff
{
    public class GetAllStaffQuery : IRequest<IEnumerable<AllStaffVm>> {}

    public class GetAllStaffHandler : IRequestHandler<GetAllStaffQuery, IEnumerable<AllStaffVm>>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly ILogger<GetAllStaffHandler> _logger;

        public GetAllStaffHandler(IStaffRepository repository, ILogger<GetAllStaffHandler> logger)
        {
            _staffRepo = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllStaffVm>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
        {
            var staffs = await _staffRepo.GetAllAsync();

            _logger.LogInformation("--------Getting Staffs-----------");

            return staffs.Select(c => new AllStaffVm
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
                OrganizationName = c.StaffOrganizations.Count() > 0 ? c.StaffOrganizations[0].Organization.Name : null
            });
        }
    }
}