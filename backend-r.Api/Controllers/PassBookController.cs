namespace backend_r.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PassBookController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PassBookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize("AuthorizedTo.PassBook.View")]
        public async Task<ActionResult<IEnumerable<GetAllPassBook>>> GetAllPassBook()
        {
            var getAllPassBookQuery = new GetAllPassBookQuery();
            var passBooks = await _mediator.Send(getAllPassBookQuery);

            return Ok(passBooks);
        }
    }
}