namespace backend_r.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize("AuthorizedTo.Role.Add")]
        public async Task<ActionResult> CreateRole(CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut]
        [Authorize("AuthorizedTo.Role.Edit")]
        public async Task<ActionResult> UpdateRole(UpdateRole command)
        {
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound(string.Format("Role with id `{0}` not found!", command.Id));

            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.Role.View")]
        public async Task<ActionResult<OneRoleVm>> GetByIdAsync(int id)
        {
            var getAllRoleQuery = new GetOneRoleQuery { Id = id };
            var result = await _mediator.Send(getAllRoleQuery);

            if (result == null)
                return NotFound(string.Format("Role with id `{0}` not found!", id));

            return Ok(result);
        }

        [HttpGet]
        [Authorize("AuthorizedTo.Role.View")]
        public async Task<ActionResult<IEnumerable<AllRoleVm>>> GetAllRole()
        {
            var getAllRoleQuery = new GetAllRoleQuery();
            var result = await _mediator.Send(getAllRoleQuery);

            return Ok(result);
        }
    }
}