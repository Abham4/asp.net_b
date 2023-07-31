namespace backend_r.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class StaffController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StaffController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize("AuthorizedTo.Staff.View")]
        public async Task<ActionResult<IEnumerable<AllStaffVm>>> GetAllStaff()
        {
            var getAllStaffQuery = new GetAllStaffQuery();
            var staffes = await _mediator.Send(getAllStaffQuery);
            return Ok(staffes);
        }

        [HttpGet("UnRegisteredStaffs")]
        [Authorize("AuthorizedTo.Staff.Unregistered")]
        public async Task<ActionResult<IEnumerable<AllStaffVm>>> GetAllUnRegisteredStaffs()
        {
            var getAllUnRegisteredStaff = new GetAllUnRegisteredStaff();
            var staffes = await _mediator.Send(getAllUnRegisteredStaff);

            return Ok(staffes);
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.Staff.View")]
        public async Task<ActionResult<OneStaffVm>> GetStaffById(int id)
        {
            var getOneStaffQuery = new GetOneStaffQuery { Id = id };
            var staff = await _mediator.Send(getOneStaffQuery);

            if (staff == null)
                return NotFound(string.Format("Staff with id `{0}` not found!", id));
            else
                return Ok(staff);
        }

        [HttpPost]
        [Authorize("AuthorizedTo.Staff.Add")]
        public async Task<ActionResult> CreateStaff([FromForm] CreateStaffCommand command)
        {
            await _mediator.Send(command);
            return StatusCode(201);
        }

        [HttpPost("AddStaffAddress")]
        [Authorize("AuthorizedTo.Staff.Add")]
        public async Task<ActionResult> CreateStaffAddress(CreateStaffAddress staffAddress)
        {
            await _mediator.Send(staffAddress);
            return StatusCode(201);
        }

        [HttpPost("AddStaffIdentity")]
        [Authorize("AuthorizedTo.Staff.Add")]
        public async Task<ActionResult> CreateStaffIdentity(CreateStaffIdentity identity)
        {
            await _mediator.Send(identity);
            return StatusCode(201);
        }

        [HttpPut]
        [Authorize("AuthorizedTo.Staff.Edit")]
        public async Task<ActionResult> UpdateStaff([FromForm] UpdateStaffCommand command)
        {
            var updated = await _mediator.Send(command);

            if (updated == null)
                return NotFound(string.Format("Staff with id `{0}` not found!", command.Id));

            return NoContent();
        }

        [HttpPut("EditStaffAddress")]
        [Authorize("AuthorizedTo.Staff.Edit")]
        public async Task<ActionResult> UpdateStaffAddress(UpdateStaffAddress staffAddress)
        {
            var updated = await _mediator.Send(staffAddress);

            if (updated == null)
                return NotFound(string.Format("Staff with id `{0}` not found!", staffAddress.Id));

            return NoContent();
        }

        [HttpPut("EditStaffIdentity")]
        [Authorize("AuthorizedTo.Staff.Edit")]
        public async Task<ActionResult> UpdateStaffIdentity(UpdateStaffIdentity identity)
        {
            var updated = await _mediator.Send(identity);

            if (updated == null)
                return NotFound(string.Format("Staff with id `{0}` not found!", identity.Id));

            return NoContent();
        }
    }
}
