namespace backend_r.Application.Common.Queries.Product
{
    public class GetAllProductQuery : IRequest<IEnumerable<AllProductVm>> {}

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, IEnumerable<AllProductVm>>
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<GetAllProductQueryHandler> _logger;

        public GetAllProductQueryHandler(IProductRepository productRepository,
            ILogger<GetAllProductQueryHandler> logger)
        {
            _productRepo = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllProductVm>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetAllAsync();

            _logger.LogInformation("--------Getting Products---------");

            return products.Select(c => new AllProductVm
            {
                Id = c.Id,
                ProductTypeName = c.AccountProductType.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                ClosedDate = c.ClosedDate
            });
        }
    }
}