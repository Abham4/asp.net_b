namespace backend_r.Application.Common.Commands.Staff.Address
{
    public class UpdateStaffAddress : IRequest<string>
    {
        public int Id { get; set; }
        public int Reference { get; set; }
        public string AddressType { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
    }

    public class UpdateStaffAddressHandler : IRequestHandler<UpdateStaffAddress, string>
    {
        private readonly IAddressRepository _addressRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly ILogger<UpdateStaffAddressHandler> _logger;

        public UpdateStaffAddressHandler(IAddressRepository addressRepository, IStaffRepository staffRepository,
            ILogger<UpdateStaffAddressHandler> logger)
        {
            _addressRepo = addressRepository;
            _staffRepo = staffRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateStaffAddress request, CancellationToken cancellationToken)
        {
            var address = await _addressRepo.GetByIdAsync(request.Id);
            var staff = await _staffRepo.GetByIdAsync(request.Reference);

            if (address == null)
                return null;

            if (staff == null)
                throw new DomainException("Related staff doesn't exist!");

            address.AddressType = request.AddressType != null ? request.AddressType : address.AddressType;
            address.Attribute = request.Attribute != null ? request.Attribute : address.Attribute;
            address.Value = request.Value != null ? request.Value : address.Value;

            _logger.LogInformation("---------Updating stafff address------------");

            _addressRepo.UpdateAsync(address);
            await _addressRepo.UnitOfWork.SaveChanges();

            return "Updated";
        }
    }

    public class UpdateStaffAddressValidator : AbstractValidator<UpdateStaffAddress>
    {
        public UpdateStaffAddressValidator()
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