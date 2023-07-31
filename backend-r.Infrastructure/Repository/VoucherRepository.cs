namespace backend_r.Infrastructure.Repository
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        private readonly JoshuaContext _context;
        private readonly IProductScheduleRepository _scheduleRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IProductRangeRepository _productRangeRepo;
        private readonly IResourceRepository _resourceRepo;
        private readonly IIdDefinitionRepository _idDefinitionRepo;
        private readonly IUserRepository _userRepo;
        private List<Voucher> ReturnableVouchers = new List<Voucher>();

        public VoucherRepository(JoshuaContext joshuaContext, IProductScheduleRepository scheduleRepository,
            IMemberRepository memberRepository, IProductRangeRepository productRangeRepository,
            IResourceRepository resourceRepository, IIdDefinitionRepository idDefinitionRepository,
            IUserRepository userRepository) : base(joshuaContext)
        {
            _context = joshuaContext;
            _scheduleRepo = scheduleRepository;
            _memberRepo = memberRepository;
            _productRangeRepo = productRangeRepository;
            _resourceRepo = resourceRepository;
            _idDefinitionRepo = idDefinitionRepository;
            _userRepo = userRepository;
        }

        private int countDigit(int number)
        {
            if (number / 10 == 0)
                return 1;

            return 1 + countDigit(number / 10);
        }

        public override List<string> DefaultPermission()
        {
            return new List<string>
            {
                $"AuthorizedTo.{nameof(Voucher)}.Add",
                $"AuthorizedTo.{nameof(Voucher)}.View",
                $"AuthorizedTo.{nameof(Voucher)}.Edit",
                $"AuthorizedTo.{nameof(Voucher)}.Remove",
                $"AuthorizedTo.{nameof(Voucher)}.Summary",
                $"AuthorizedTo.{nameof(Voucher)}.SummaryHighers"
            };
        }

        public async override Task<IReadOnlyList<Voucher>> GetAllAsync()
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .Where(e => e.VoucherTypeId != VoucherType.Daily_Loan_Bigining_Balance.Id &&
                        e.VoucherTypeId != VoucherType.Daily_Save_Bigining_Balance.Id && e.VoucherTypeId !=
                        VoucherType.Daily_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                        .Monthly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Monthly_Save_Bigining_Balance.Id
                        && e.VoucherTypeId != VoucherType.Monthly_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                        .Yearly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Yearly_Save_Bigining_Balance.Id &&
                        e.VoucherTypeId != VoucherType.Yearly_Share_Bigining_Balance.Id)
                    .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                    .ToListAsync();
            else
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .Where(e => e.VoucherTypeId != VoucherType.Daily_Loan_Bigining_Balance.Id &&
                        e.VoucherTypeId != VoucherType.Daily_Save_Bigining_Balance.Id && e.VoucherTypeId !=
                        VoucherType.Daily_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                        .Monthly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Monthly_Save_Bigining_Balance.Id
                        && e.VoucherTypeId != VoucherType.Monthly_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                        .Yearly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Yearly_Save_Bigining_Balance.Id &&
                        e.VoucherTypeId != VoucherType.Yearly_Share_Bigining_Balance.Id)
                    .ToListAsync();
        }

        public async override Task<Voucher> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                    .SingleOrDefaultAsync(e => e.Id == id);
            else
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task Pay(PurchasedProduct purchasedProduct, double amount, int voucherTypeId, int organizationId,
            string remark)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var reference = purchasedProduct.Account.AccountMaps[0].Reference;
            var member = await _memberRepo.GetByIdAsync(reference);
            var commingVoucherType = VoucherType.List().SingleOrDefault(c => c.Id == voucherTypeId);
            var resource = await _resourceRepo.GetResourceByType(commingVoucherType.Name);
            var idDefinition = await _idDefinitionRepo.GetByResourseTypeAndOrganizationId(organizationId, resource.Type);

            var value = "";

            var digits = idDefinition.Length - countDigit(idDefinition.NextValue);

            while (digits > 0)
            {
                value += '0';
                digits--;
            }

            value += idDefinition.NextValue;

            string code = idDefinition.Prefix + idDefinition.PrefSep + value + idDefinition.SuffSep + idDefinition.Suffix;

            idDefinition.NextValue += 1;

            _idDefinitionRepo.UpdateAsync(idDefinition);
            await _idDefinitionRepo.UnitOfWork.SaveChanges();

            var voucher = new Voucher(code, purchasedProduct.Id, voucherTypeId, DateTime.Now, amount, organizationId,
                member.Code, remark, user.Email);

            await AddAsync(voucher);
            await UnitOfWork.SaveChanges();

            ReturnableVouchers.Add(await GetByIdAsync(voucher.Id));
        }

        public async Task FullPay(PurchasedProduct purchasedProduct, List<ProductSchedule> productSchedules,
            int organizationId, string remark)
        {
            if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Loan.Id)
            {
                for (int i = 0; i < productSchedules.Count(); i++)
                {
                    await Pay(purchasedProduct, productSchedules[i].InterestDue, VoucherType.Loan_Interest.Id,
                        organizationId, remark);

                    await Pay(purchasedProduct, productSchedules[i].PricipalDue, VoucherType.Loan_Principal.Id,
                        organizationId, remark);

                    productSchedules[i].Status = true;

                    _scheduleRepo.UpdateAsync(productSchedules[i]);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                for (int i = 0; i < productSchedules.Count(); i++)
                {
                    await Pay(purchasedProduct, productSchedules[i].PricipalDue, VoucherType.Deposit.Id, organizationId,
                        remark);

                    productSchedules[i].Status = true;

                    _scheduleRepo.UpdateAsync(productSchedules[i]);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                for (int i = 0; i < productSchedules.Count(); i++)
                {
                    await Pay(purchasedProduct, productSchedules[i].PricipalDue, VoucherType.Share_Principal.Id,
                        organizationId, remark);

                    productSchedules[i].Status = true;

                    _scheduleRepo.UpdateAsync(productSchedules[i]);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
        }

        public async Task PayMultiple(int start, int end, PurchasedProduct purchasedProduct, List<ProductSchedule>
            productSchedules, int organizationId, string remark)
        {
            if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Loan.Id)
            {
                for (int i = start; i < end; i++)
                {
                    await Pay(purchasedProduct, productSchedules[i].InterestDue, VoucherType.Loan_Interest.Id,
                        organizationId, remark);

                    await Pay(purchasedProduct, productSchedules[i].PricipalDue, VoucherType.Loan_Principal.Id,
                        organizationId, remark);

                    productSchedules[i].Status = true;

                    _scheduleRepo.UpdateAsync(productSchedules[i]);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                for (int i = start; i < end; i++)
                {
                    await Pay(purchasedProduct, productSchedules[i].PricipalDue, VoucherType.Deposit.Id,
                        organizationId, remark);

                    productSchedules[i].Status = true;

                    _scheduleRepo.UpdateAsync(productSchedules[i]);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                for (int i = start; i < end; i++)
                {
                    await Pay(purchasedProduct, productSchedules[i].PricipalDue, VoucherType.Share_Principal.Id,
                        organizationId, remark);

                    productSchedules[i].Status = true;

                    _scheduleRepo.UpdateAsync(productSchedules[i]);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
        }

        public async Task PayCurrentRow(double amount, double paidRemain, PurchasedProduct purchasedProduct,
            ProductSchedule productSchedule, int organizationId, string remark)
        {
            var singleRowPaidInterest = productSchedule.InterestDue;
            var singleRowPaidPrincipal = productSchedule.PricipalDue;

            if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Loan.Id)
            {
                if (paidRemain >= singleRowPaidInterest)
                {
                    await Pay(purchasedProduct, amount, VoucherType.Loan_Principal.Id, organizationId, remark);

                    if (Math.Round(amount + (Math.Round(paidRemain - singleRowPaidInterest, 2)), 2) ==
                        singleRowPaidPrincipal)
                    {
                        productSchedule.Status = true;

                        _scheduleRepo.UpdateAsync(productSchedule);
                        await _scheduleRepo.UnitOfWork.SaveChanges();
                    }
                }
                else
                {
                    var remainInterest = Math.Round(singleRowPaidInterest - paidRemain, 2);

                    if (amount <= remainInterest)
                    {
                        await Pay(purchasedProduct, amount, VoucherType.Loan_Interest.Id, organizationId, remark);
                    }
                    else
                    {
                        await Pay(purchasedProduct, remainInterest, VoucherType.Loan_Interest.Id, organizationId, remark);

                        amount = Math.Round(amount - remainInterest, 2);

                        if (amount > singleRowPaidPrincipal)
                            amount = singleRowPaidPrincipal;

                        await Pay(purchasedProduct, amount, VoucherType.Loan_Principal.Id, organizationId, remark);

                        if (amount >= singleRowPaidPrincipal)
                        {
                            productSchedule.Status = true;

                            _scheduleRepo.UpdateAsync(productSchedule);
                            await _scheduleRepo.UnitOfWork.SaveChanges();
                        }
                    }
                }
            }
            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                await Pay(purchasedProduct, amount, VoucherType.Deposit.Id, organizationId, remark);

                if (Math.Round(amount + paidRemain, 2) == singleRowPaidPrincipal)
                {
                    productSchedule.Status = true;

                    _scheduleRepo.UpdateAsync(productSchedule);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                await Pay(purchasedProduct, amount, VoucherType.Share_Principal.Id, organizationId, remark);

                if (Math.Round(amount + paidRemain, 2) == singleRowPaidPrincipal)
                {
                    productSchedule.Status = true;

                    _scheduleRepo.UpdateAsync(productSchedule);
                    await _scheduleRepo.UnitOfWork.SaveChanges();
                }
            }
        }

        public async Task<List<Voucher>> Add(int organizationId, int purchasedProductId, double amount, string remark)
        {
            var purchasedProduct = await _context.PurchasedProducts
                .Include(c => c.Account)
                .ThenInclude(e => e.AccountMaps)
                .Include(c => c.Product)
                .ThenInclude(c => c.AccountProductType)
                .Include(c => c.Product)
                .ThenInclude(c => c.LoanExtension)
                .Include(c => c.Product)
                .ThenInclude(c => c.SaveExtention)
                .Include(c => c.Product)
                .ThenInclude(c => c.ShareExtension)
                .Include(c => c.ProductSetups)
                .Include(c => c.ScheduleHeaders)
                .ThenInclude(e => e.ProductSchedules)
                .Include(c => c.Vouchers)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Id == purchasedProductId);

            if (purchasedProduct == null)
                throw new DomainException("PurchasedProductId is not valid!");

            if (purchasedProduct.ProductSetups.Count() == 0)
                throw new DomainException("Invalid PurchasedProduct!");

            if (purchasedProduct.ScheduleHeaders.Count() == 0)
                throw new DomainException("Invalid PurchasedProduct!");

            if (purchasedProduct.Product.LoanExtension != null)
            {
                var repaymentRange = await _productRangeRepo.GetByIdAsync(purchasedProduct.
                    Product.LoanExtension.RepaymentRange);

                if (repaymentRange == null)
                    throw new DomainException("Bought invalid product.");

                if (amount < repaymentRange.Min || amount > repaymentRange.Max)
                    throw new DomainException("Must be between " + repaymentRange.Min +
                    " - " + repaymentRange.Max);
            }

            if (purchasedProduct.Product.SaveExtention != null)
            {
                var minCompulsoryAmount = purchasedProduct.Product.SaveExtention.MinCompulsoryAmount;

                if (amount < minCompulsoryAmount)
                    throw new DomainException("Must be " + minCompulsoryAmount);
            }

            if (purchasedProduct.Product.ShareExtension != null)
            {
                var nominalPrice = purchasedProduct.Product.ShareExtension.NominalPrice;

                if (amount % nominalPrice != 0)
                    throw new DomainException("Must be paid " + nominalPrice + " multiples");
            }

            var repaymentCount = 0;

            if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Loan.Id)
                repaymentCount = purchasedProduct.ProductSetups.SingleOrDefault(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name).PaymentCount;

            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Saving.Id)
                repaymentCount = purchasedProduct.ProductSetups.Exists(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name && c.CreatedDate.Year == DateTime.Now.Year) ? 12 : 0;

            else if (purchasedProduct.Product.AccountProductTypeId == AccountProductType.Sharing.Id)
                repaymentCount = purchasedProduct.ProductSetups.SingleOrDefault(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name).PaymentCount;

            var paid = Math.Round(purchasedProduct.Vouchers.Where(e => e.VoucherTypeId != VoucherType
                .Daily_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Daily_Save_Bigining_Balance
                .Id && e.VoucherTypeId != VoucherType.Daily_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                .Monthly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Monthly_Save_Bigining_Balance.Id &&
                e.VoucherTypeId != VoucherType.Monthly_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                .Yearly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Yearly_Save_Bigining_Balance.Id &&
                e.VoucherTypeId != VoucherType.Yearly_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Disbursement.Id)
                .Sum(c => c.AmountTransacted), 2);

            var productSchedules = purchasedProduct.ScheduleHeaders.SingleOrDefault(c => c.LastObjectState ==
                ObjectStateEnumeration.Active.Name).ProductSchedules;

            int paidStopIndex = productSchedules.Where(c => c.Status == true).Count();

            if (paidStopIndex == repaymentCount)
                throw new DomainException("Paying has finished!");

            var due = Math.Round(productSchedules[paidStopIndex].PricipalDue + productSchedules[paidStopIndex].InterestDue,
                2);

            if (paid == 0.0 && (int)(amount / due) >= repaymentCount)
            {
                await FullPay(purchasedProduct, productSchedules, organizationId, remark);
                return ReturnableVouchers;
            }

            var lastPaid = 0.0;

            if (paid < due)
                lastPaid = Math.Round(paid % due, 2);
            else
                lastPaid = Math.Round(paid - Math.Round(paidStopIndex * due, 2), 2);

            var currentPayment = Math.Round(due - lastPaid, 2);
            var roundedAmount = Math.Round(amount, 2);
            var singleRowPaid = productSchedules[paidStopIndex];

            if (roundedAmount <= currentPayment)
            {
                await PayCurrentRow(roundedAmount, lastPaid, purchasedProduct, singleRowPaid, organizationId, remark);
            }
            else
            {
                await PayCurrentRow(currentPayment, lastPaid, purchasedProduct, singleRowPaid, organizationId, remark);

                roundedAmount = Math.Round(roundedAmount - currentPayment, 2);

                var payCount = (int)(roundedAmount / due);
                var remain = 0.0;

                if (roundedAmount < due)
                    remain = Math.Round(roundedAmount % due, 2);
                else
                    remain = Math.Round(roundedAmount - Math.Round(due * payCount, 2), 2);

                var end = 0;

                if (payCount == 0)
                    end = paidStopIndex + 1;
                else
                {
                    if (payCount > repaymentCount - (paidStopIndex + 1))
                        end = repaymentCount - (paidStopIndex + 1);
                    else
                        end = paidStopIndex + 1 + payCount;

                    await PayMultiple(paidStopIndex + 1, end, purchasedProduct, productSchedules, organizationId, remark);
                }

                if (remain != 0)
                {
                    if (end < repaymentCount)
                    {
                        singleRowPaid = productSchedules[end];
                        await PayCurrentRow(remain, 0.0, purchasedProduct, singleRowPaid, organizationId, remark);
                    }
                }
            }

            return ReturnableVouchers;
        }

        public async Task<Voucher> GetVoucherSummaryByIdForOrganization(int id)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .SingleOrDefaultAsync(c => c.Id == id && c.Member == null && c.OrganizationId == user.Staff.StaffOrganizations[0]
                        .OrganizationId);
            else
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .SingleOrDefaultAsync(c => c.Id == id && c.Member == null);
        }

        public async Task<Voucher> GetVoucherSummaryByIdForMember(int id)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .SingleOrDefaultAsync(c => c.Id == id && c.Member != null && c.OrganizationId == user.Staff.StaffOrganizations[0]
                        .OrganizationId);
            else
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .SingleOrDefaultAsync(c => c.Id == id && c.Member != null);
        }

        public async Task<List<Voucher>> Withdraw(int organizationId, int purchasedProductId, double amount, string remark)
        {
            var purchasedProduct = await _context.PurchasedProducts
                .Include(c => c.Account)
                .ThenInclude(c => c.AccountMaps)
                .Include(c => c.Product)
                .ThenInclude(c => c.AccountProductType)
                .Include(c => c.Product)
                .ThenInclude(c => c.SaveExtention)
                .Include(c => c.ProductSetups)
                .Include(c => c.ScheduleHeaders)
                .ThenInclude(c => c.ProductSchedules)
                .Include(c => c.Vouchers)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Id == purchasedProductId);

            if (purchasedProduct == null)
                throw new DomainException("PurchasedProductId is not valid!");

            if (purchasedProduct.ProductSetups.Count() == 0)
                throw new DomainException("Invalid PurchasedProduct!");

            if (purchasedProduct.ScheduleHeaders.Count() == 0)
                throw new DomainException("Invalid PurchasedProduct!");

            if (purchasedProduct.Product.AccountProductTypeId != AccountProductType.Saving.Id)
                throw new DomainException("Withdraw allowed for saving product only!");

            var paid = Math.Round(purchasedProduct.Vouchers.Where(e => e.VoucherTypeId != VoucherType
                .Daily_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Daily_Save_Bigining_Balance
                .Id && e.VoucherTypeId != VoucherType.Daily_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                .Monthly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Monthly_Save_Bigining_Balance.Id &&
                e.VoucherTypeId != VoucherType.Monthly_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType
                .Yearly_Loan_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Yearly_Save_Bigining_Balance.Id &&
                e.VoucherTypeId != VoucherType.Yearly_Share_Bigining_Balance.Id && e.VoucherTypeId != VoucherType.Disbursement.Id)
                .Sum(c => c.AmountTransacted), 2);

            if (paid == 0)
                throw new DomainException("You've saved nothing!");

            var scheduleCreatedDate = purchasedProduct.ScheduleHeaders.SingleOrDefault(c => c.LastObjectState ==
                ObjectStateEnumeration.Active.Name).CreatedDate;
            var paidMonth = (int)(DateTime.Now.Subtract(scheduleCreatedDate).TotalDays / 30.436875F);
            var existingAmount = purchasedProduct.ProductSetups.SingleOrDefault(c => c.LastObjectState ==
                ObjectStateEnumeration.Active.Name).Amount;

            if (paidMonth == 0)
                paidMonth = 1;

            var expectedPayment = existingAmount * paidMonth;

            if (paid <= expectedPayment)
                throw new DomainException("You can't withdraw!");

            if (amount > paid - expectedPayment)
                throw new DomainException("You can't withdraw.");

            await Pay(purchasedProduct, -1 * amount, VoucherType.Withdrawal.Id, organizationId, remark);

            return ReturnableVouchers;
        }

        public async Task<List<Voucher>> GetVoucherByOrganizationIdAndClosingPeriodId(int closingPeriodId, int organizationId)
        {
            var vouchers = new List<Voucher>();
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
            {
                if (closingPeriodId == ClosingPeriod.Daily.Id)
                    vouchers = await _context.Vouchers.Where(c => c.OrganizationId == organizationId &&
                        c.TimeStamp.Day == DateTime.Now.Day).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .ToListAsync();

                else if (closingPeriodId == ClosingPeriod.Monthly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.OrganizationId == organizationId &&
                        c.TimeStamp.Month == DateTime.Now.Month).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .ToListAsync();

                else if (closingPeriodId == ClosingPeriod.Yearly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.OrganizationId == organizationId &&
                        c.TimeStamp.Year == DateTime.Now.Year).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .ToListAsync();
            }

            else
            {
                if (closingPeriodId == ClosingPeriod.Daily.Id)
                    vouchers = await _context.Vouchers.Where(c => c.OrganizationId == organizationId &&
                        c.TimeStamp.Day == DateTime.Now.Day).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .ToListAsync();

                else if (closingPeriodId == ClosingPeriod.Monthly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.OrganizationId == organizationId &&
                        c.TimeStamp.Month == DateTime.Now.Month).Include(c => c.VoucherType)
                        .Include(c => c.Organization).ToListAsync();

                else if (closingPeriodId == ClosingPeriod.Yearly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.OrganizationId == organizationId &&
                        c.TimeStamp.Year == DateTime.Now.Year).Include(c => c.VoucherType)
                        .Include(c => c.Organization).ToListAsync();
            }

            return vouchers;
        }

        private async Task<string> GetCodeFromId(int memberId)
        {
            var member = await _context.Members.SingleOrDefaultAsync(c => c.Id == memberId);

            if (member == null)
                throw new DomainException("Member not found!");

            return member.Code;
        }

        public async Task<List<Voucher>> GetVoucherByMemberCodeAndClosingPeriodId(int closingPeriodId, int memberId)
        {
            var vouchers = new List<Voucher>();
            var memberCode = await GetCodeFromId(memberId);
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
            {
                if (closingPeriodId == ClosingPeriod.Daily.Id)
                    vouchers = await _context.Vouchers.Where(c => c.Member == memberCode &&
                        c.TimeStamp.Day == DateTime.Now.Day).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .ToListAsync();

                if (closingPeriodId == ClosingPeriod.Monthly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.Member == memberCode &&
                        c.TimeStamp.Month == DateTime.Now.Month).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .ToListAsync();

                if (closingPeriodId == ClosingPeriod.Yearly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.Member == memberCode &&
                        c.TimeStamp.Year == DateTime.Now.Year).Include(c => c.VoucherType)
                        .Include(c => c.Organization)
                        .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                        .ToListAsync();
            }

            else
            {
                if (closingPeriodId == ClosingPeriod.Daily.Id)
                    vouchers = await _context.Vouchers.Where(c => c.Member == memberCode &&
                        c.TimeStamp.Day == DateTime.Now.Day).Include(c => c.VoucherType)
                        .Include(c => c.Organization).ToListAsync();

                if (closingPeriodId == ClosingPeriod.Monthly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.Member == memberCode &&
                        c.TimeStamp.Month == DateTime.Now.Month).Include(c => c.VoucherType)
                        .Include(c => c.Organization).ToListAsync();

                if (closingPeriodId == ClosingPeriod.Yearly.Id)
                    vouchers = await _context.Vouchers.Where(c => c.Member == memberCode &&
                        c.TimeStamp.Year == DateTime.Now.Year).Include(c => c.VoucherType)
                        .Include(c => c.Organization).ToListAsync();
            }

            return vouchers;
        }

        private async Task<List<Voucher>> GetVouchers()
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if (user.Staff.StaffOrganizations.Any())
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .Where(c => c.OrganizationId == user.Staff.StaffOrganizations[0].OrganizationId)
                    .ToListAsync();
            else
                return await _context.Vouchers
                    .Include(c => c.VoucherType)
                    .Include(c => c.Organization)
                    .ToListAsync();
        }

        private async Task<List<double>> VouchersByVoucherType(int voucherTypeId)
        {
            var vouchers = await GetVouchers();

            var daily = Math.Round(vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day && c.VoucherTypeId ==
                voucherTypeId).Sum(c => c.AmountTransacted), 2);

            var currentDayOfTheWeek = DateTime.Now.DayOfWeek;
            int daysTillCurrentDay = currentDayOfTheWeek - DayOfWeek.Monday;

            var weekly = Math.Round(vouchers.Where(c => c.CreatedDate.Date >= DateTime.Now.AddDays(-daysTillCurrentDay).Date &&
                c.CreatedDate.Date <= DateTime.Now.AddDays(-daysTillCurrentDay + 5).Date && c.VoucherTypeId ==
                    voucherTypeId).Sum(c => c.AmountTransacted), 2);

            var monthly = Math.Round(vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month && c.VoucherTypeId ==
                voucherTypeId).Sum(c => c.AmountTransacted), 2);

            return new List<double> { daily, weekly, monthly };
        }

        public async Task<List<List<double>>> VouchersByVoucherTypeforHighers(string organizationName)
        {
            var vouchers = await GetVouchers();
            var doubles = new List<List<double>>();

            var daily = Math.Round(vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day && c.VoucherTypeId ==
                VoucherType.Deposit.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var currentDayOfTheWeek = DateTime.Now.DayOfWeek;
            int daysTillCurrentDay = currentDayOfTheWeek - DayOfWeek.Monday;

            var weekly = Math.Round(vouchers.Where(c => c.CreatedDate.Date >= DateTime.Now.AddDays(-daysTillCurrentDay).Date &&
                c.CreatedDate.Date <= DateTime.Now.AddDays(-daysTillCurrentDay + 5).Date && c.VoucherTypeId ==
                    VoucherType.Deposit.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var monthly = Math.Round(vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month && c.VoucherTypeId ==
                VoucherType.Deposit.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var deposits = new List<double> { daily, weekly, monthly };

            daily = Math.Round(vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day && c.VoucherTypeId ==
                VoucherType.Disbursement.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            weekly = Math.Round(vouchers.Where(c => c.CreatedDate.Date >= DateTime.Now.AddDays(-daysTillCurrentDay).Date &&
                c.CreatedDate.Date <= DateTime.Now.AddDays(-daysTillCurrentDay + 5).Date && c.VoucherTypeId ==
                    VoucherType.Disbursement.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            monthly = Math.Round(vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month && c.VoucherTypeId ==
                VoucherType.Disbursement.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var disbursements = new List<double> { daily, weekly, monthly };

            daily = Math.Round(vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day && (c.VoucherTypeId ==
                VoucherType.Loan_Interest.Id || c.VoucherTypeId == VoucherType.Loan_Penality.Id || c.VoucherTypeId ==
                VoucherType.Loan_Principal.Id) && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            weekly = Math.Round(vouchers.Where(c => c.CreatedDate.Date >= DateTime.Now.AddDays(-daysTillCurrentDay).Date &&
                c.CreatedDate.Date <= DateTime.Now.AddDays(-daysTillCurrentDay + 5).Date && (c.VoucherTypeId ==
                VoucherType.Loan_Interest.Id || c.VoucherTypeId == VoucherType.Loan_Penality.Id || c.VoucherTypeId ==
                VoucherType.Loan_Principal.Id) && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            monthly = Math.Round(vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month && (c.VoucherTypeId ==
                VoucherType.Loan_Interest.Id || c.VoucherTypeId == VoucherType.Loan_Penality.Id || c.VoucherTypeId ==
                VoucherType.Loan_Principal.Id) && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var repayments = new List<double> { daily, weekly, monthly };

            daily = Math.Round(vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day && c.VoucherTypeId ==
                VoucherType.Share_Principal.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            weekly = Math.Round(vouchers.Where(c => c.CreatedDate.Date >= DateTime.Now.AddDays(-daysTillCurrentDay).Date &&
                c.CreatedDate.Date <= DateTime.Now.AddDays(-daysTillCurrentDay + 5).Date && c.VoucherTypeId ==
                    VoucherType.Share_Principal.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            monthly = Math.Round(vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month && c.VoucherTypeId ==
                VoucherType.Share_Principal.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var sharePayments = new List<double> { daily, weekly, monthly };

            daily = Math.Round(vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day && c.VoucherTypeId ==
                VoucherType.Withdrawal.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            weekly = Math.Round(vouchers.Where(c => c.CreatedDate.Date >= DateTime.Now.AddDays(-daysTillCurrentDay).Date &&
                c.CreatedDate.Date <= DateTime.Now.AddDays(-daysTillCurrentDay + 5).Date && c.VoucherTypeId ==
                    VoucherType.Withdrawal.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            monthly = Math.Round(vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month && c.VoucherTypeId ==
                VoucherType.Withdrawal.Id && c.Organization.Name == organizationName).Sum(c => c.AmountTransacted), 2);

            var withdrawals = new List<double> { daily, weekly, monthly };

            doubles.Add(deposits);
            doubles.Add(disbursements);
            doubles.Add(repayments);
            doubles.Add(sharePayments);
            doubles.Add(withdrawals);

            return doubles;
        }

        public async Task<List<double>> Summary()
        {
            var summary = new List<double>();
            var loanPrincipal = await VouchersByVoucherType(VoucherType.Loan_Principal.Id);
            var loanInterest = await VouchersByVoucherType(VoucherType.Loan_Interest.Id);
            var loanPenality = await VouchersByVoucherType(VoucherType.Loan_Penality.Id);
            var summation = new List<double>();

            summation.Add(loanInterest[0] + loanPenality[0] + loanPrincipal[0]);
            summation.Add(loanInterest[1] + loanPenality[1] + loanPrincipal[1]);
            summation.Add(loanInterest[2] + loanPenality[2] + loanPrincipal[2]);

            summary.AddRange(await VouchersByVoucherType(VoucherType.Deposit.Id));
            summary.AddRange(await VouchersByVoucherType(VoucherType.Disbursement.Id));
            summary.AddRange(summation);
            summary.AddRange(await VouchersByVoucherType(VoucherType.Share_Principal.Id));
            summary.AddRange(await VouchersByVoucherType(VoucherType.Withdrawal.Id));

            return summary;
        }
    }
}