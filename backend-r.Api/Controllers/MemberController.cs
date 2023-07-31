namespace backend_r.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class MemberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MemberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize("AuthorizedTo.Member.View")]
        public async Task<ActionResult<IEnumerable<AllMemberVm>>> GetAllMember()
        {
            var getAllMembersQuery = new GetAllMembersQuery();
            var members = await _mediator.Send(getAllMembersQuery);

            return Ok(members);
        }

        [HttpGet("GetMemberShipGrowthData")]
        [Authorize("AuthorizedTo.Member.GrowthSummary")]
        public async Task<ActionResult<IEnumerable<MemberShipVm>>> GetMemberShipGrowths()
        {
            var getAllMembersQuery = new GetAnnualMembershipGrowth();
            var members = await _mediator.Send(getAllMembersQuery);

            return Ok(members);
        }

        [HttpGet("UnRegisteredMembers")]
        [Authorize("AuthorizedTo.Member.Unregistered")]
        public async Task<ActionResult<IEnumerable<AllMemberVm>>> GetAllUnRegisteredMember()
        {
            var getAllUnRegisteredMembersQuery = new GetAllUnRegisteredMembersQuery();
            var members = await _mediator.Send(getAllUnRegisteredMembersQuery);

            return Ok(members);
        }

        [HttpGet("{id}")]
        [Authorize("AuthorizedTo.Member.View")]
        public async Task<ActionResult<OneMemberVm>> GetMemberById(int id)
        {
            var getOneMemberQuery = new GetOneMemberQuery { Id = id };
            var member = await _mediator.Send(getOneMemberQuery);

            if (member == null)
                return NotFound(string.Format("Member with id `{0}` not found!", id));
            else
                return Ok(member);
        }

        [HttpPost]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> CreateMember([FromForm] CreateMemberCommand model)
        {
            await _mediator.Send(model);

            return StatusCode(201);
        }

        [HttpPost("AddMemberAddress")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> CreateMemberAddress(CreateMemberAddress address)
        {
            await _mediator.Send(address);

            return StatusCode(201);
        }

        [HttpPost("AddMemberIdentity")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> CreateMemberIdentity(CreateMemberIdentity identity)
        {
            await _mediator.Send(identity);

            return StatusCode(201);
        }

        [HttpPost("AddMemberSpouse")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> CreateMemberSpouse(CreateMemberSpouse spouse)
        {
            await _mediator.Send(spouse);

            return StatusCode(201);
        }

        [HttpPost("AddMemberGuardian")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> CreateMemberGuardian(CreateMemberGuardian guardian)
        {
            await _mediator.Send(guardian);

            return StatusCode(201);
        }

        [HttpPut]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> UpdateMember([FromForm] UpdateMemberCommand model)
        {
            var updated = await _mediator.Send(model);

            if (updated == null)
                return NotFound(string.Format("Member with id `{0}` not found!", model.Id));
            else
                return NoContent();
        }

        [HttpPut("EditMemberAddress")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> UpdateMemberAddress(UpdateMemberAddress address)
        {
            var updated = await _mediator.Send(address);

            if (updated == null)
                return NotFound(string.Format("Member with id `{0}` not found!", address.Id));
            else
                return NoContent();
        }

        [HttpPut("EditMemberIdentity")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> UpdateMemberIdentity(UpdateMemberIdentity identity)
        {
            var updated = await _mediator.Send(identity);

            if (updated == null)
                return NotFound(string.Format("Member with id `{0}` not found!", identity.Id));
            else
                return NoContent();
        }

        [HttpPut("EditMemberSpouse")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> UpdateMemberSpouse(UpdateMemberSpouse spouse)
        {
            var updated = await _mediator.Send(spouse);

            if (updated == null)
                return NotFound(string.Format("Member with id `{0}` not found!", spouse.Id));
            else
                return NoContent();
        }

        [HttpPut("EditMemberGuardian")]
        [Authorize("AuthorizedTo.Member.Add")]
        public async Task<ActionResult> UpdateMemberGuardian(UpdateMemberGuardian guardian)
        {
            var updated = await _mediator.Send(guardian);

            if (updated == null)
                return NotFound(string.Format("Member with id `{0}` not found!", guardian.Id));
            else
                return NoContent();
        }

        [HttpGet("GetMembersByState/{state}")]
        [Authorize("AuthorizedTo.Member.ByState")]
        public async Task<IEnumerable<AllMemberVm>> GetMembersByState(string state)
        {
            var getMembersByState = new GetMembersByStateQuery { State = state };
            var members = await _mediator.Send(getMembersByState);

            return members;
        }

        [HttpGet("GetMemberCarrers")]
        [Authorize("AuthorizedTo.Member.CarrerSummary")]
        public async Task<IEnumerable<MemberCarrersVm>> GetMemberCarrers()
        {
            var getMemberCarrers = new GetMemberCarrersQuery();
            var numberOfCarrers = await _mediator.Send(getMemberCarrers);

            return numberOfCarrers;
        }

        [HttpPost("ToActive/{memberId}")]
        [Authorize("AuthorizedTo.Member.Activate")]
        public async Task<ActionResult<string>> ToActive(int memberId)
        {
            var changeToActiveCommand = new ChangeToActiveCommand { MemberId = memberId };
            var result = await _mediator.Send(changeToActiveCommand);

            if (result == null)
                return NotFound(string.Format("Member with id `{0}` not found!", memberId));
            else
                return Ok(result);
        }

        [HttpPost("ToTerminated/{memberId}")]
        [Authorize("AuthorizedTo.Member.Terminate")]
        public async Task<ActionResult<string>> ToTerminated(int memberId)
        {
            var changeToTerminatedCommand = new ChangeToTerminatedCommand { MemberId = memberId };
            var result = await _mediator.Send(changeToTerminatedCommand);

            if (result == null)
                return NotFound(string.Format("Member with id `{0}` not found!", memberId));
            else
                return Ok(result);
        }

        [HttpGet("ShowTransaction/{closingPeriodId}&{memberId}")]
        [Authorize("AuthorizedTo.Member.ShowTransaction")]
        public async Task<ActionResult<IEnumerable<AllVoucher>>> MemberTransactions(
            int closingPeriodId, int memberId)
        {
            var getAllVoucher = new GetAllVoucherByMemberCodeAndTimestamp
            {
                ClosingPeriodId = closingPeriodId,
                MemberId = memberId
            };
            var vouchers = await _mediator.Send(getAllVoucher);

            return Ok(vouchers);
        }

        [HttpPost("Close")]
        [Authorize("AuthorizedTo.Member.Close")]
        public async Task<ActionResult<string>> MemberClose(CreateMemberClosing command)
        {
            var memberClose = await _mediator.Send(command);

            return StatusCode(201, memberClose);
        }

        [HttpGet("ClosedTransactions")]
        [Authorize("AuthorizedTo.Member.ClosedTransaction")]
        public async Task<ActionResult<IEnumerable<AllVoucherReference>>> ClosedMemberTransactions()
        {
            var getAllMemberSummeries = new GetAllMemberSummeries();
            var voucherReferences = await _mediator.Send(getAllMemberSummeries);

            if (voucherReferences == null)
                return Ok(Array.Empty<AllVoucherReference>());

            return Ok(voucherReferences);
        }
    }
}
