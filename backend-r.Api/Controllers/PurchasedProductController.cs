namespace backend_r.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PurchasedProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchasedProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize("AuthorizedTo.PurchasedProduct.Add")]
        public async Task<ActionResult> CreatePurchaseProduct(CreatePurchaseProductCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet]
        [Authorize("AuthorizedTo.PurchasedProduct.View")]
        public async Task<ActionResult<IEnumerable<AllPurchasedProductVm>>> GetAllPurchasedProduct()
        {
            var getAllPurchasedProductQuery = new GetAllPurchasedProductQuery();
            var purchasedProducts = await _mediator.Send(getAllPurchasedProductQuery);

            return Ok(purchasedProducts);
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.PurchasedProduct.View")]
        public async Task<ActionResult<IEnumerable<OnePurchasedProductVm>>> GetOnePurchasedProduct(int id)
        {
            var getOnePurchasedProductQuery = new GetOnePurchasedProductQuery { Id = id };
            var purchasedProduct = await _mediator.Send(getOnePurchasedProductQuery);

            if (purchasedProduct == null)
                return NotFound(string.Format("PurchasedProduct with id `{0}` not found!", id));

            return Ok(purchasedProduct);
        }

        [HttpGet("GetPurchasedProductsByAccountId/{accountId}")]
        [Authorize("AuthorizedTo.PurchasedProduct.View")]
        public async Task<ActionResult<IEnumerable<OnePurchasedProductVm>>> GetAllPurchasedProductByAccountId(int accountId)
        {
            var getAllPurchasedProductByAccountIdQuery = new GetAllPurchasedProductByAccountIdQuery { AccountId = accountId };
            var purchasedProducts = await _mediator.Send(getAllPurchasedProductByAccountIdQuery);

            if (purchasedProducts == null)
                return NotFound(string.Format("PurchasedProduct with account id `{0}` not found!", accountId));

            return Ok(purchasedProducts);
        }

        [HttpPut]
        [Authorize("AuthorizedTo.PurchasedProduct.Edit")]
        public async Task<ActionResult> UpdatePurchasedProduct(UpdatePurchasedProductCommand command)
        {
            var updated = await _mediator.Send(command);

            if (updated == null)
                return NotFound(string.Format("PurchasedProduct with id `{0}` not found!", command.Id));

            return Ok(updated);
        }
    }
}