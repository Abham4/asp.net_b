namespace backend_r.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize("AuthorizedTo.Product.Add")]
        public async Task<ActionResult> CreateProduct(CreateProductCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut]
        [Authorize("AuthorizedTo.Product.Edit")]
        public async Task<ActionResult> UpdateProduct(UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound(string.Format("Product with id `{0}` not found!", command.Id));
            else
                return NoContent();
        }

        [HttpGet]
        [Authorize("AuthorizedTo.Product.View")]
        public async Task<ActionResult<IEnumerable<AllProductVm>>> GetAllProduct()
        {
            var getAllProductQuery = new GetAllProductQuery();
            var products = await _mediator.Send(getAllProductQuery);

            return Ok(products);
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.Product.View")]
        public async Task<ActionResult<OneProductVm>> GetOneProduct(int id)
        {
            var getOneProductQuery = new GetOneProductQuery { Id = id };
            var product = await _mediator.Send(getOneProductQuery);

            if (product == null)
                return NotFound(string.Format("Product with id `{0}` not found!", id));

            return Ok(product);
        }
    }
}