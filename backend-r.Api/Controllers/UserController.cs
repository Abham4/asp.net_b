namespace backend_r.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginCommand command)
        {
            var isLogged = await _mediator.Send(command);

            if (isLogged == "Unauthorized")
                return Unauthorized(isLogged);
            else
                return Ok(isLogged);
        }

        [HttpPost("Logout")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Logout(LogoutCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("Register")]
        [Authorize("AuthorizedTo.User.Add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Register(RegisterCommand command)
        {
            var response = await _mediator.Send(command);

            if (response != "User Created.")
                return BadRequest(response);
            else
                return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.User.View")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<OneUserVm>> GetUser(int id)
        {
            var getOneUserQuery = new GetOneUserQuery { Id = id };
            var result = await _mediator.Send(getOneUserQuery);

            if (result == null)
                return NotFound(string.Format("User with id `{0}` not found!", id));
            else
                return Ok(result);
        }

        [HttpGet("Get-Authenticated-User")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<OneUserVm>> GetAuthenticatedUser()
        {
            var getAuthenticatedUserQuery = new GetAuthenticatedUserQuery();
            var result = await _mediator.Send(getAuthenticatedUserQuery);

            return Ok(result);
        }

        [HttpGet("Get-Users")]
        [Authorize("AuthorizedTo.User.View")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<AllUserVm>>> GetUsers()
        {
            var getAllUserQuery = new GetAllUserQuery();
            var response = await _mediator.Send(getAllUserQuery);

            return Ok(response);
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize("AuthorizedTo.User.Edit")]
        public async Task<ActionResult> Update(UpdateCommand command)
        {
            var updated = await _mediator.Send(command);

            if (updated == null)
                return NotFound(string.Format("User with id `{0}` not found!", command.Id));

            return NoContent();
        }

        [HttpGet("NotExist")]
        [AllowAnonymous]
        public ActionResult NotExist() =>  NotFound("Sorry, The path you're trying doesn't exist!");
    }
}