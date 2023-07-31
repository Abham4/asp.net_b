namespace backend_r.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class VoucherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoucherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Payment")]
        [Authorize("AuthorizedTo.Voucher.Add")]
        public async Task<ActionResult<IEnumerable<AllVoucher>>> CreateVoucher(CreateVoucherCommand command)
        {
            var vouchers = await _mediator.Send(command);

            return Ok(vouchers);
        }

        [HttpPost("Withdraw")]
        [Authorize("AuthorizedTo.Voucher.Add")]
        public async Task<ActionResult<IEnumerable<AllVoucher>>> CreateWithdrawVoucher(CreateVoucherWithdraw withdraw)
        {
            var vouchers = await _mediator.Send(withdraw);

            return Ok(vouchers);
        }

        [HttpGet]
        [Authorize("AuthorizedTo.Voucher.View")]
        public async Task<ActionResult<IEnumerable<AllVoucher>>> GetVouchers()
        {
            var getAllVoucher = new GetAllVoucher();
            var vouchers = await _mediator.Send(getAllVoucher);

            return Ok(vouchers);
        }

        [HttpGet("Summary")]
        [Authorize("AuthorizedTo.Voucher.Summary")]
        public async Task<ActionResult<IEnumerable<GetVoucherSummary>>> Summary()
        {
            var voucherSummary = new GetVoucherSummary();
            var summary = await _mediator.Send(voucherSummary);

            return Ok(summary);
        }

        [HttpGet("SummaryforHighers")]
        [Authorize("AuthorizedTo.Voucher.SummaryHighers")]
        public async Task<ActionResult<IEnumerable<GetVoucherSummaryforHighers>>> SummaryforHighers()
        {
            var voucherSummaryforHighers = new GetVoucherSummaryforHighers();
            var summary = await _mediator.Send(voucherSummaryforHighers);

            return Ok(summary);
        }
    }
}