namespace backend_r.Application.Common.Commands.Member.Address
{
    public class UpdateMemberAddress : IRequest<string>
    {
        public int Id { get; set; }
        public int Reference { get; set; }
        public string AddressType { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
    }

    public class UpdateMemberAddressHandler : IRequestHandler<UpdateMemberAddress, string>
    {
        private readonly IAddressRepository _addressRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly ILogger<UpdateMemberAddressHandler> _logger;

        public UpdateMemberAddressHandler(IAddressRepository addressRepository, IMemberRepository memberRepository,
            ILogger<UpdateMemberAddressHandler> logger)
        {
            _addressRepo = addressRepository;
            _memberRepo = memberRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateMemberAddress request, CancellationToken cancellationToken)
        {
            var address = await _addressRepo.GetByIdAsync(request.Id);
            var member = await _memberRepo.CheckExistence(request.Reference);

            if (address == null)
                return null;

            if (!member)
                throw new DomainException("Related member doesn't exist!");

            address.AddressType = request.AddressType != null ? request.AddressType : address.AddressType;
            address.Attribute = request.Attribute != null ? request.Attribute : address.Attribute;
            address.Value = request.Value != null ? request.Value : address.Value;

            _logger.LogInformation("---------- Updating Member Address ----------");

            _addressRepo.UpdateAsync(address);
            await _addressRepo.UnitOfWork.SaveChanges();

            return "Updated";
        }
    }

    public class UpdateMemberAddressValidator : AbstractValidator<UpdateMemberAddress>
    {
        public UpdateMemberAddressValidator()
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