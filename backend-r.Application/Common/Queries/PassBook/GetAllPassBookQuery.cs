namespace backend_r.Application.Common.Queries.PassBook
{
    public class GetAllPassBookQuery : IRequest<IEnumerable<Vms.GetAllPassBook>> { }

    public class GetAllPassBookQueryHandler : IRequestHandler<GetAllPassBookQuery, IEnumerable<Vms.GetAllPassBook>>
    {
        private readonly IPassBookRepository _passBookRepo;
        private readonly ILogger<GetAllPassBookQueryHandler> _logger;

        public GetAllPassBookQueryHandler(IPassBookRepository passBookRepository, ILogger<GetAllPassBookQueryHandler> logger)
        {
            _logger = logger;
            _passBookRepo = passBookRepository;
        }

        public async Task<IEnumerable<GetAllPassBook>> Handle(GetAllPassBookQuery request, CancellationToken cancellationToken)
        {
            var passBooks = await _passBookRepo.GetAllAsync();

            _logger.LogInformation("---------Getting All PassBook-----------");

            return passBooks.Select(c => new GetAllPassBook
            {
                Code = c.Code
            });
        }
    }
}