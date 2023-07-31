namespace backend_r.Application.Common.Commands.Member.Address
{
    public class CreateMemberAddress : IRequest<string>
    {
        public int Reference { get; set; }
        public int AddressType { get; set; }
        public int Attribute { get; set; }
        public string Value { get; set; }
    }

    public class CreateMemberAddressHandler : IRequestHandler<CreateMemberAddress, string>
    {
        private readonly IAddressRepository _addressRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateMemberAddressHandler> _logger;

        public CreateMemberAddressHandler(IAddressRepository addressRepository, IMemberRepository memberRepository,
            ILogger<CreateMemberAddressHandler> logger, IUserRepository userRepository)
        {
            _addressRepo = addressRepository;
            _memberRepo = memberRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateMemberAddress request, CancellationToken cancellationToken)
        {
            var member = await _memberRepo.CheckExistence(request.Reference);
            var user = await _userRepo.GetAuthenticatedUser();

            if (!member)
                throw new DomainException("Refered member doesn't exist!");

            var addressType = AddressType.List().SingleOrDefault(c => c.Id == request.AddressType).Name;
            var attribute = AttributeType.List().SingleOrDefault(c => c.Id == request.Attribute).Name;

            var address = new Domain.Entities.Address("Member", addressType, request.Value, attribute, request.Reference,
                user.Email);

            _logger.LogInformation("---------Adding New Member Address--------");

            await _addressRepo.AddAsync(address);
            await _addressRepo.UnitOfWork.SaveChanges();

            return "Created";
        }
    }

    public class CreateMemberAddressValidator : AbstractValidator<CreateMemberAddress>
    {
        [Obsolete]
        public CreateMemberAddressValidator()
        {
            RuleFor(c => c.Reference)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.AddressType)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3).WithMessage("{PropertyName} is null reference!");

            When(c => c.AddressType == 1 || c.AddressType == 2, () =>
            {
                RuleFor(c => c.Attribute)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .InclusiveBetween(1, 8).WithMessage("{PropertyName} is null reference!");

                When(c => c.Attribute == 1, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Region)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Region)} can't be null!");
                });

                When(c => c.Attribute == 2, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.City)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.City)} can't be null!");
                });

                When(c => c.Attribute == 3, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Sub_City_or_Zone)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Sub_City_or_Zone)} can't be null!");
                });

                When(c => c.Attribute == 4, () =>
                {
                    RuleFor(c => c.Value)
                        .Must(IsDigit).WithMessage($"{nameof(AttributeType.Woreda)} must be digit/s only!")
                        .Matches("^[\\d]{1,5}$").WithMessage($"{nameof(AttributeType.Woreda)} must match be with range of 5 digit!")
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Woreda)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Woreda)} can't be null!");
                });

                When(c => c.Attribute == 5, () =>
                {
                    RuleFor(c => c.Value)
                        .Must(IsDigit).WithMessage($"{nameof(AttributeType.Kebele)} must be digit/s only!")
                        .Matches("^[\\d]{1,5}$").WithMessage($"{nameof(AttributeType.Kebele)} must match be with range of 5 digit!")
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Kebele)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Kebele)} can't be null!");
                });

                When(c => c.Attribute == 6, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.House_Number)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.House_Number)} can't be null!");
                });

                When(c => c.Attribute == 7, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.TelePhone)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.TelePhone)} can't be null!")
                        .Matches("^\\+[0-9]{1,3}[0-9]{4,14}$").WithMessage($"{nameof(AttributeType.TelePhone)} is not in a correct format!");
                });

                When(c => c.Attribute == 8, () =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage($"{nameof(AttributeType.Mobile)} can't be empty!")
                        .NotNull().WithMessage($"{nameof(AttributeType.Mobile)} can't be null!")
                        .Matches("^\\+[0-9]{1,3}[0-9]{4,14}$").WithMessage($"{nameof(AttributeType.Mobile)} is not in a correct format!");
                });
            });

            When(c => c.AddressType == 3, () =>
            {
                RuleFor(c => c.Attribute)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .InclusiveBetween(9, 14).WithMessage("{PropertyName} is null reference!");

                When(c => c.Attribute == 9, () =>
                {
                    RuleFor(c => c.Value)
                        .EmailAddress(FluentValidation.Validators.EmailValidationMode.Net4xRegex)
                            .WithMessage($"{nameof(AttributeType.Email)} is invalid!")
                        .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                        .NotNull().WithMessage("{PropertyName} can't be null!");
                })
                .Otherwise(() =>
                {
                    RuleFor(c => c.Value)
                        .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                        .NotNull().WithMessage("{PropertyName} can't be null!")
                        .Must(IsValidUrl).WithMessage("{PropertyName} is not valid url!");
                });
            });
        }

        private bool IsDigit(string value)
        {
            return value.All(Char.IsDigit);
        }

        private bool IsValidUrl(string value)
        {
            Uri uriResult;
            return Uri.TryCreate(value, UriKind.Absolute, out uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}