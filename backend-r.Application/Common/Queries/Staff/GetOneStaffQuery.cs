namespace backend_r.Application.Common.Queries.Staff
{
    public class GetOneStaffQuery : IRequest<OneStaffVm>
    {
        public int Id { get; set; }
    }

    public class GetOneStaffHandler : IRequestHandler<GetOneStaffQuery, OneStaffVm>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IIdentityRepository _identityRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly ILogger<GetOneStaffHandler> _logger;

        public GetOneStaffHandler(IStaffRepository staffRepository, IAddressRepository addressRepository,
            IIdentityRepository identityRepository, ILogger<GetOneStaffHandler> logger)
        {
            _staffRepo = staffRepository;
            _addressRepo = addressRepository;
            _identityRepo = identityRepository;
            _logger = logger;
        }

        public async Task<OneStaffVm> Handle(GetOneStaffQuery request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepo.GetByIdAsync(request.Id);

            _logger.LogInformation("----------Getting One Staff------------");

            if (staff == null)
                return null;

            var addresses = await _addressRepo.GetByReferenceAndOwnerType(request.Id, "Staff");
            var identities = await _identityRepo.GetByReferenceAndOwnerType(request.Id, "Staff");

            var oneStaff = new OneStaffVm
            {
                Id = staff.Id,
                Title = staff.Title,
                FirstName = staff.FirstName,
                MiddleName = staff.MiddleName,
                LastName = staff.LastName,
                Gender = new Vms.Gender
                {
                    Id = staff.Gender.Id,
                    Name = staff.Gender.Name
                },
                EmpDate = staff.EmpDate,
                DOB = staff.DOB,
                LastObjectState = staff.LastObjectState,
                Organization = new OneOrganizationVm
                {
                    Id = staff.StaffOrganizations.Count() > 0 ? staff.StaffOrganizations[0]
                        .Organization.Id : 0,
                    Name = staff.StaffOrganizations.Count() > 0 ? staff.StaffOrganizations[0]
                        .Organization.Name : null
                },
                Addresses = addresses != null ? addresses.Select(c => new AddressVm
                {
                    Id = c.Id,
                    OwnerType = c.OwnerType,
                    AddressType = c.AddressType,
                    Attribute = c.Attribute,
                    Value = c.Value,
                    Reference = c.Reference
                }).ToList() : null,
                Identities = identities != null ? identities.Select(d => new IdentityVm
                {
                    Id = d.Id,
                    Owner = d.Owner,
                    Type = d.Type,
                    Description = d.Description,
                    Number = d.Number,
                    Reference = d.Reference
                }).ToList() : null,
                ProfileImg = staff.ProfileImg
            };

            return oneStaff;
        }
    }
}