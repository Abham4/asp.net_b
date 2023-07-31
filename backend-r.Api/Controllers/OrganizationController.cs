namespace backend_r.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrganizationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize("AuthorizedTo.Organization.View")]
        public async Task<ActionResult<IEnumerable<AllOrganizationVm>>> GetAllOrganization()
        {
            var getAllOrganizationQuery = new GetAllOrganizationQuery();
            var organizations = await _mediator.Send(getAllOrganizationQuery);

            return Ok(organizations);
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.Organization.View")]
        public async Task<ActionResult<OneOrganizationVm>> GetOrganizationById(int id)
        {
            var getOneOrganizationQuery = new GetOneOrganizationQuery { Id = id };
            var organization = await _mediator.Send(getOneOrganizationQuery);

            if (organization == null)
                return NotFound(string.Format("Organization with id `{0}` not found!", id));
            else
                return Ok(organization);
        }

        [HttpPost]
        [Authorize("AuthorizedTo.Organization.Add")]
        public async Task<ActionResult> CreateOrganization(CreateOrganizationCommand command)
        {
            await _mediator.Send(command);
            return StatusCode(201);
        }

        [HttpPut]
        [Authorize("AuthorizedTo.Organization.Edit")]
        public async Task<ActionResult> UpdateOrganization(UpdateOrganizationCommand command)
        {
            var updated = await _mediator.Send(command);

            if (updated == null)
                return NotFound(string.Format("Member with id `{0}` not found!", command.Id));
            else
                return NoContent();
        }

        [HttpGet("GetMemberShipGrowthForHigher")]
        [Authorize("AuthorizedTo.Organization.MemberGrowthsSummaryHigher")]
        public async Task<ActionResult<IEnumerable<MemberShipForHighersVm>>> GetMemberShipGrowthForHighers()
        {
            var getAnnualMembershipGrowthForHighers = new GetAnnualMembershipGrowthForHighers();
            var members = await _mediator.Send(getAnnualMembershipGrowthForHighers);

            return Ok(members);
        }

        [HttpGet("GetMemberCarrersForHigher")]
        [Authorize("AuthorizedTo.Organization.MemberCarrersSummaryHigher")]
        public async Task<ActionResult<IEnumerable<MemberCarrersForHigersVm>>> GetMemberCarrersForHighers()
        {
            var getMemberCarrersForHigers = new GetMemberCarrersForHigers();
            var members = await _mediator.Send(getMemberCarrersForHigers);

            return Ok(members);
        }

        [HttpGet("ClosedTransactions")]
        [Authorize("AuthorizedTo.Organization.ClosedTransaction")]
        public async Task<ActionResult<IEnumerable<AllVoucherReference>>> ClosedOrganizationTransactions()
        {
            var getAllOrganizationSummeries = new GetAllOrganizationSummeries();
            var voucherReferences = await _mediator.Send(getAllOrganizationSummeries);

            if (voucherReferences == null)
                return Ok(Array.Empty<AllVoucherReference>());

            return Ok(voucherReferences);
        }

        [HttpGet("ShowTransactions/{organizationId}&{closingPeriodId}")]
        [Authorize("AuthorizedTo.Organization.ShowTransaction")]
        public async Task<ActionResult<IEnumerable<AllVoucher>>> OrganizationTransactions(
            int organizationId, int closingPeriodId)
        {
            var getAllVoucher = new GetAllVoucherByOrganizationAndTimestamp
            {
                ClosingPeriodId = closingPeriodId,
                OrganizationId = organizationId
            };
            var vouchers = await _mediator.Send(getAllVoucher);

            return Ok(vouchers);
        }

        [HttpPost("Close")]
        [Authorize("AuthorizedTo.Organization.Close")]
        public async Task<ActionResult<string>> OrganizationClose(CreateOrganizationClosing command)
        {
            var organizationalClose = await _mediator.Send(command);
            return StatusCode(201, organizationalClose);
        }
    }
}
