namespace backend_r.Infrastructure.Repository
{
    public class VoucherRefenceRepository : BaseRepository<VoucherReference>, IVoucherReferenceRepository
    {
        private readonly JoshuaContext _joshuaContext;
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IAccountMapRepository _accountMapRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IResourceRepository _resourceRepo;
        private readonly IIdDefinitionRepository _idDefinitionRepo;
        private readonly IUserRepository _userRepo;

        public VoucherRefenceRepository(JoshuaContext context, IOrganizationRepository organizationRepository,
            IAccountMapRepository accountMapRepository, IVoucherRepository voucherRepository,
            IMemberRepository memberRepository, IResourceRepository resourceRepository,
            IIdDefinitionRepository idDefinitionRepository, IUserRepository userRepository) : base(context)
        {
            _joshuaContext = context;
            _accountMapRepo = accountMapRepository;
            _organizationRepo = organizationRepository;
            _voucherRepo = voucherRepository;
            _memberRepo = memberRepository;
            _resourceRepo = resourceRepository;
            _idDefinitionRepo = idDefinitionRepository;
            _userRepo = userRepository;
        }

        public async Task<List<VoucherReference>> GetVoucherReferencesBySummaryVoucher(int summaryVoucherId)
        {
            return await _joshuaContext.VoucherReferences.Where(c => c.SummaryVoucher == summaryVoucherId).ToListAsync();
        }

        private int countDigit(int number)
        {
            if (number / 10 == 0)
                return 1;

            return 1 + countDigit(number / 10);
        }

        private async Task<string> codeGiver(string voucherTypeName, int organizationId)
        {
            var resource = await _resourceRepo.GetResourceByType(voucherTypeName);

            if (resource == null)
                throw new DomainException("Invalid Resource!");

            var idDefinition = await _idDefinitionRepo.GetByResourseTypeAndOrganizationId(organizationId, resource.Type);

            if (idDefinition == null)
                throw new DomainException("Invalid IdDefinition!");

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

            return code;
        }

        public async Task MemberClose(int closingPeriodId, int memberId)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var member = await _memberRepo.GetByIdAsync(memberId);

            if (member == null)
                throw new DomainException("Invalid Member!");

            var memberOrganization = await _joshuaContext.MemberOrganizations.SingleOrDefaultAsync(c => c.MemberId ==
                memberId);

            if (memberOrganization == null)
                throw new DomainException("Invalid Member!");

            if (_joshuaContext.Vouchers.Where(c => c.Member == member.Code).Count() < 1)
                throw new DomainException(string.Format("Sorry, {0} {1} have no transaction to close.",
                    member.FirstName, member.LastName));

            var accountMaps = await _accountMapRepo.GetAccountMapByReferenceAndOwner(memberId, "Member");
            var loans = 0.0;
            var loanIds = new List<int>();
            var saves = 0.0;
            var saveIds = new List<int>();
            var shares = 0.0;
            var shareIds = new List<int>();

            if (closingPeriodId == ClosingPeriod.Daily.Id)
            {
                if (_joshuaContext.Vouchers.Any(d => d.CreatedDate.Day == DateTime.Now.Day &&
                    d.VoucherTypeId == VoucherType.Daily_Loan_Bigining_Balance.Id && d.Member == member.Code) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Day == DateTime.Now.Day &&
                    d.VoucherTypeId == VoucherType.Daily_Save_Bigining_Balance.Id && d.Member == member.Code) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Day == DateTime.Now.Day &&
                    d.VoucherTypeId == VoucherType.Daily_Share_Bigining_Balance.Id && d.Member == member.Code))
                    throw new DomainException("Already Done!");

                accountMaps.ForEach(accountMap =>
                {
                    accountMap.Account.PurchasedProducts.ForEach(purchasedProduct =>
                    {
                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Loan.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day).ToList()
                                .ForEach(voucher =>
                                {
                                    loans = Math.Round(loans + voucher.AmountTransacted, 2);
                                    loanIds.Add(voucher.Id);
                                });
                            else
                                loanIds.Add(0);
                        }

                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day).ToList()
                                .ForEach(voucher =>
                                {
                                    saves = Math.Round(saves + voucher.AmountTransacted, 2);
                                    saveIds.Add(voucher.Id);
                                });
                            else
                                saveIds.Add(0);
                        }

                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day).ToList()
                                .ForEach(voucher =>
                                {
                                    shares = Math.Round(shares + voucher.AmountTransacted, 2);
                                    shareIds.Add(voucher.Id);
                                });
                            else
                                shareIds.Add(0);
                        }
                    });
                });

                var loanVoucherCode = await codeGiver(VoucherType.Daily_Loan_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var loanVoucher = new Voucher(loanVoucherCode, VoucherType.Daily_Loan_Bigining_Balance.Id, DateTime.Now,
                    loans, memberOrganization.OrganizationId, member.Code, user.Email);

                var saveVoucherCode = await codeGiver(VoucherType.Daily_Save_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var saveVoucher = new Voucher(saveVoucherCode, VoucherType.Daily_Save_Bigining_Balance.Id, DateTime.Now,
                    saves, memberOrganization.OrganizationId, member.Code, user.Email);

                var shareVoucherCode = await codeGiver(VoucherType.Daily_Share_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var shareVoucher = new Voucher(shareVoucherCode, VoucherType.Daily_Share_Bigining_Balance.Id, DateTime.Now,
                    shares, memberOrganization.OrganizationId, member.Code, user.Email);

                await _voucherRepo.AddAsync(loanVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(saveVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(shareVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                foreach (var loanId in loanIds)
                {
                    var loanVoucherReference = new VoucherReference(loanVoucher.Id, loanId, user.Email);

                    await AddAsync(loanVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var saveId in saveIds)
                {
                    var saveVoucherReference = new VoucherReference(saveVoucher.Id, saveId, user.Email);

                    await AddAsync(saveVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var shareId in shareIds)
                {
                    var shareVoucherReference = new VoucherReference(shareVoucher.Id, shareId, user.Email);

                    await AddAsync(shareVoucherReference);
                    await UnitOfWork.SaveChanges();
                }
            }

            else if (closingPeriodId == ClosingPeriod.Monthly.Id)
            {
                if (_joshuaContext.Vouchers.Any(d => d.CreatedDate.Month == DateTime.Now.Month &&
                    d.VoucherTypeId == VoucherType.Monthly_Loan_Bigining_Balance.Id && d.Member == member.Code) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Month == DateTime.Now.Month &&
                    d.VoucherTypeId == VoucherType.Monthly_Save_Bigining_Balance.Id && d.Member == member.Code) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Month == DateTime.Now.Month &&
                    d.VoucherTypeId == VoucherType.Monthly_Share_Bigining_Balance.Id && d.Member == member.Code))
                    throw new DomainException("Already Done!");

                accountMaps.ForEach(accountMap =>
                {
                    accountMap.Account.PurchasedProducts.ForEach(purchasedProduct =>
                    {
                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Loan.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month).ToList()
                                .ForEach(voucher =>
                                {
                                    loans = Math.Round(loans + voucher.AmountTransacted, 2);
                                    loanIds.Add(voucher.Id);
                                });
                            else
                                loanIds.Add(0);
                        }

                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month).ToList()
                                .ForEach(voucher =>
                                {
                                    saves = Math.Round(saves + voucher.AmountTransacted, 2);
                                    saveIds.Add(voucher.Id);
                                });
                            else
                                saveIds.Add(0);
                        }

                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month).ToList()
                                .ForEach(voucher =>
                                {
                                    shares = Math.Round(shares + voucher.AmountTransacted, 2);
                                    shareIds.Add(voucher.Id);
                                });
                            else
                                shareIds.Add(0);
                        }
                    });
                });

                var loanVoucherCode = await codeGiver(VoucherType.Monthly_Loan_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var loanVoucher = new Voucher(loanVoucherCode, VoucherType.Monthly_Loan_Bigining_Balance.Id, DateTime.Now,
                    loans, memberOrganization.OrganizationId, member.Code, user.Email);

                var saveVoucherCode = await codeGiver(VoucherType.Monthly_Save_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var saveVoucher = new Voucher(saveVoucherCode, VoucherType.Monthly_Save_Bigining_Balance.Id, DateTime.Now,
                    saves, memberOrganization.OrganizationId, member.Code, user.Email);

                var shareVoucherCode = await codeGiver(VoucherType.Monthly_Share_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var shareVoucher = new Voucher(shareVoucherCode, VoucherType.Monthly_Share_Bigining_Balance.Id, DateTime.Now,
                    shares, memberOrganization.OrganizationId, member.Code, user.Email);

                await _voucherRepo.AddAsync(loanVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(saveVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(shareVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                foreach (var loanId in loanIds)
                {
                    var loanVoucherReference = new VoucherReference(loanVoucher.Id, loanId, user.Email);

                    await AddAsync(loanVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var saveId in saveIds)
                {
                    var saveVoucherReference = new VoucherReference(saveVoucher.Id, saveId, user.Email);

                    await AddAsync(saveVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var shareId in shareIds)
                {
                    var shareVoucherReference = new VoucherReference(shareVoucher.Id, shareId, user.Email);

                    await AddAsync(shareVoucherReference);
                    await UnitOfWork.SaveChanges();
                }
            }

            else if (closingPeriodId == ClosingPeriod.Yearly.Id)
            {
                if (_joshuaContext.Vouchers.Any(d => d.CreatedDate.Year == DateTime.Now.Year &&
                    d.VoucherTypeId == VoucherType.Yearly_Loan_Bigining_Balance.Id && d.Member == member.Code) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Year == DateTime.Now.Year &&
                    d.VoucherTypeId == VoucherType.Yearly_Save_Bigining_Balance.Id && d.Member == member.Code) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Year == DateTime.Now.Year &&
                    d.VoucherTypeId == VoucherType.Yearly_Share_Bigining_Balance.Id && d.Member == member.Code))
                    throw new DomainException("Already Done!");

                accountMaps.ForEach(accountMap =>
                {
                    accountMap.Account.PurchasedProducts.ForEach(purchasedProduct =>
                    {
                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Loan.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Year == DateTime.Now.Year).ToList()
                                .ForEach(voucher =>
                                {
                                    loans = Math.Round(loans + voucher.AmountTransacted, 2);
                                    loanIds.Add(voucher.Id);
                                });
                            else
                                loanIds.Add(0);
                        }

                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Year == DateTime.Now.Year).ToList()
                                .ForEach(voucher =>
                                {
                                    saves = Math.Round(saves + voucher.AmountTransacted, 2);
                                    saveIds.Add(voucher.Id);
                                });
                            else
                                saveIds.Add(0);
                        }

                        if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                        {
                            if (purchasedProduct.Vouchers.Count() > 0)
                                purchasedProduct.Vouchers.Where(c => c.CreatedDate.Year == DateTime.Now.Year).ToList()
                                .ForEach(voucher =>
                                {
                                    shares = Math.Round(shares + voucher.AmountTransacted, 2);
                                    shareIds.Add(voucher.Id);
                                });
                            else
                                shareIds.Add(0);
                        }
                    });
                });

                var loanVoucherCode = await codeGiver(VoucherType.Yearly_Loan_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var loanVoucher = new Voucher(loanVoucherCode, VoucherType.Yearly_Loan_Bigining_Balance.Id, DateTime.Now,
                    loans, memberOrganization.OrganizationId, member.Code, user.Email);

                var saveVoucherCode = await codeGiver(VoucherType.Yearly_Save_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var saveVoucher = new Voucher(saveVoucherCode, VoucherType.Yearly_Save_Bigining_Balance.Id, DateTime.Now,
                    saves, memberOrganization.OrganizationId, member.Code, user.Email);

                var shareVoucherCode = await codeGiver(VoucherType.Yearly_Share_Bigining_Balance.Name, memberOrganization.OrganizationId);
                var shareVoucher = new Voucher(shareVoucherCode, VoucherType.Yearly_Share_Bigining_Balance.Id, DateTime.Now,
                    shares, memberOrganization.OrganizationId, member.Code, user.Email);

                await _voucherRepo.AddAsync(loanVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(saveVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(shareVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                foreach (var loanId in loanIds)
                {
                    var loanVoucherReference = new VoucherReference(loanVoucher.Id, loanId, user.Email);

                    await AddAsync(loanVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var saveId in saveIds)
                {
                    var saveVoucherReference = new VoucherReference(saveVoucher.Id, saveId, user.Email);

                    await AddAsync(saveVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var shareId in shareIds)
                {
                    var shareVoucherReference = new VoucherReference(shareVoucher.Id, shareId, user.Email);

                    await AddAsync(shareVoucherReference);
                    await UnitOfWork.SaveChanges();
                }
            }
        }

        public async Task OrganizationalClose(int closingPeriodId, int organizationId)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var organization = await _organizationRepo.GetByIdAsync(organizationId);

            if (organization == null)
                throw new DomainException("Invalid Organization!");

            var memberOrganizations = await _joshuaContext.MemberOrganizations.Where(c => c.OrganizationId ==
                organizationId).ToListAsync();

            if (memberOrganizations.Count() < 1)
                throw new DomainException("Organization've no member!");

            var loans = 0.0;
            var loanIds = new List<int>();
            var saves = 0.0;
            var saveIds = new List<int>();
            var shares = 0.0;
            var shareIds = new List<int>();

            if (closingPeriodId == ClosingPeriod.Daily.Id)
            {
                if (_joshuaContext.Vouchers.Any(d => d.CreatedDate.Day == DateTime.Now.Day &&
                    d.VoucherTypeId == VoucherType.Daily_Loan_Bigining_Balance.Id) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Day == DateTime.Now.Day &&
                    d.VoucherTypeId == VoucherType.Daily_Save_Bigining_Balance.Id) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Day == DateTime.Now.Day &&
                    d.VoucherTypeId == VoucherType.Daily_Share_Bigining_Balance.Id))
                    throw new DomainException("Already Done!");

                var accountMaps = new List<List<AccountMap>>();

                foreach (var memberOrganization in memberOrganizations)
                {
                    accountMaps.Add(await _accountMapRepo.GetAccountMapByReferenceAndOwner(memberOrganization.MemberId,
                        "Member"));
                }

                accountMaps.ForEach(accountMaps =>
                {
                    accountMaps.ForEach(accountMap =>
                    {
                        accountMap.Account.PurchasedProducts.ForEach(purchaseProduct =>
                        {
                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Loan.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day).ToList()
                                    .ForEach(voucher =>
                                    {
                                        loans = Math.Round(loans + voucher.AmountTransacted, 2);
                                        loanIds.Add(voucher.Id);
                                    });
                                else
                                    loanIds.Add(0);
                            }

                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day).ToList()
                                    .ForEach(voucher =>
                                    {
                                        saves = Math.Round(saves + voucher.AmountTransacted, 2);
                                        saveIds.Add(voucher.Id);
                                    });
                                else
                                    saveIds.Add(0);
                            }

                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Day == DateTime.Now.Day).ToList()
                                    .ForEach(voucher =>
                                    {
                                        shares = Math.Round(shares + voucher.AmountTransacted, 2);
                                        shareIds.Add(voucher.Id);
                                    });
                                else
                                    shareIds.Add(0);
                            }
                        });
                    });
                });

                var loanVoucherCode = await codeGiver(VoucherType.Daily_Loan_Bigining_Balance.Name, organizationId);
                var loanVoucher = new Voucher(loanVoucherCode, VoucherType.Daily_Loan_Bigining_Balance.Id, DateTime.Now,
                    loans, organizationId, user.Email);

                var saveVoucherCode = await codeGiver(VoucherType.Daily_Save_Bigining_Balance.Name, organizationId);
                var saveVoucher = new Voucher(saveVoucherCode, VoucherType.Daily_Save_Bigining_Balance.Id, DateTime.Now,
                    saves, organizationId, user.Email);

                var shareVoucherCode = await codeGiver(VoucherType.Daily_Share_Bigining_Balance.Name, organizationId);
                var shareVoucher = new Voucher(shareVoucherCode, VoucherType.Daily_Share_Bigining_Balance.Id, DateTime.Now,
                    shares, organizationId, user.Email);

                await _voucherRepo.AddAsync(loanVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(saveVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(shareVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                foreach (var loanId in loanIds)
                {
                    var loanVoucherReference = new VoucherReference(loanVoucher.Id, loanId, user.Email);

                    await AddAsync(loanVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var saveId in saveIds)
                {
                    var saveVoucherReference = new VoucherReference(saveVoucher.Id, saveId, user.Email);

                    await AddAsync(saveVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var shareId in shareIds)
                {
                    var shareVoucherReference = new VoucherReference(shareVoucher.Id, shareId, user.Email);

                    await AddAsync(shareVoucherReference);
                    await UnitOfWork.SaveChanges();
                }
            }

            else if (closingPeriodId == ClosingPeriod.Monthly.Id)
            {
                if (_joshuaContext.Vouchers.Any(d => d.CreatedDate.Month == DateTime.Now.Month &&
                    d.VoucherTypeId == VoucherType.Monthly_Loan_Bigining_Balance.Id) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Month == DateTime.Now.Month &&
                    d.VoucherTypeId == VoucherType.Monthly_Save_Bigining_Balance.Id) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Month == DateTime.Now.Month &&
                    d.VoucherTypeId == VoucherType.Monthly_Share_Bigining_Balance.Id))
                    throw new DomainException("Already Done!");

                var accountMaps = new List<List<AccountMap>>();

                foreach (var memberOrganization in memberOrganizations)
                {
                    accountMaps.Add(await _accountMapRepo.GetAccountMapByReferenceAndOwner(memberOrganization.MemberId,
                        "Member"));
                }

                accountMaps.ForEach(accountMaps =>
                {
                    accountMaps.ForEach(accountMap =>
                    {
                        accountMap.Account.PurchasedProducts.ForEach(purchaseProduct =>
                        {
                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Loan.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month).ToList()
                                    .ForEach(voucher =>
                                    {
                                        loans = Math.Round(loans + voucher.AmountTransacted, 2);
                                        loanIds.Add(voucher.Id);
                                    });
                                else
                                    loanIds.Add(0);
                            }

                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month).ToList()
                                    .ForEach(voucher =>
                                    {
                                        saves = Math.Round(saves + voucher.AmountTransacted, 2);
                                        saveIds.Add(voucher.Id);
                                    });
                                else
                                    saveIds.Add(0);
                            }

                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Month == DateTime.Now.Month).ToList()
                                    .ForEach(voucher =>
                                    {
                                        shares = Math.Round(shares + voucher.AmountTransacted, 2);
                                        shareIds.Add(voucher.Id);
                                    });
                                else
                                    shareIds.Add(0);
                            }
                        });
                    });
                });

                var loanVoucherCode = await codeGiver(VoucherType.Monthly_Loan_Bigining_Balance.Name, organizationId);
                var loanVoucher = new Voucher(loanVoucherCode, VoucherType.Monthly_Loan_Bigining_Balance.Id, DateTime.Now,
                    loans, organizationId, user.Email);

                var saveVoucherCode = await codeGiver(VoucherType.Monthly_Save_Bigining_Balance.Name, organizationId);
                var saveVoucher = new Voucher(saveVoucherCode, VoucherType.Monthly_Save_Bigining_Balance.Id, DateTime.Now,
                    saves, organizationId, user.Email);

                var shareVoucherCode = await codeGiver(VoucherType.Monthly_Share_Bigining_Balance.Name, organizationId);
                var shareVoucher = new Voucher(shareVoucherCode, VoucherType.Monthly_Share_Bigining_Balance.Id, DateTime.Now,
                    shares, organizationId, user.Email);

                await _voucherRepo.AddAsync(loanVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(saveVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(shareVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                foreach (var loanId in loanIds)
                {
                    var loanVoucherReference = new VoucherReference(loanVoucher.Id, loanId, user.Email);

                    await AddAsync(loanVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var saveId in saveIds)
                {
                    var saveVoucherReference = new VoucherReference(saveVoucher.Id, saveId, user.Email);

                    await AddAsync(saveVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var shareId in shareIds)
                {
                    var shareVoucherReference = new VoucherReference(shareVoucher.Id, shareId, user.Email);

                    await AddAsync(shareVoucherReference);
                    await UnitOfWork.SaveChanges();
                }
            }

            else if (closingPeriodId == ClosingPeriod.Yearly.Id)
            {
                if (_joshuaContext.Vouchers.Any(d => d.CreatedDate.Year == DateTime.Now.Year &&
                    d.VoucherTypeId == VoucherType.Yearly_Loan_Bigining_Balance.Id) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Year == DateTime.Now.Year &&
                    d.VoucherTypeId == VoucherType.Yearly_Save_Bigining_Balance.Id) &&
                    _joshuaContext.Vouchers.Any(d => d.CreatedDate.Year == DateTime.Now.Year &&
                    d.VoucherTypeId == VoucherType.Yearly_Share_Bigining_Balance.Id))
                    throw new DomainException("Already Done!");

                var accountMaps = new List<List<AccountMap>>();

                foreach (var memberOrganization in memberOrganizations)
                {
                    accountMaps.Add(await _accountMapRepo.GetAccountMapByReferenceAndOwner(memberOrganization.MemberId,
                        "Member"));
                }

                accountMaps.ForEach(accountMaps =>
                {
                    accountMaps.ForEach(accountMap =>
                    {
                        accountMap.Account.PurchasedProducts.ForEach(purchaseProduct =>
                        {
                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Loan.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Year == DateTime.Now.Year).ToList()
                                    .ForEach(voucher =>
                                    {
                                        loans = Math.Round(loans + voucher.AmountTransacted, 2);
                                        loanIds.Add(voucher.Id);
                                    });
                                else
                                    loanIds.Add(0);
                            }

                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Year == DateTime.Now.Year).ToList()
                                    .ForEach(voucher =>
                                    {
                                        saves = Math.Round(saves + voucher.AmountTransacted, 2);
                                        saveIds.Add(voucher.Id);
                                    });
                                else
                                    saveIds.Add(0);
                            }

                            if (purchaseProduct.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                            {
                                if (purchaseProduct.Vouchers.Count() > 0)
                                    purchaseProduct.Vouchers.Where(c => c.CreatedDate.Year == DateTime.Now.Year).ToList()
                                    .ForEach(voucher =>
                                    {
                                        shares = Math.Round(shares + voucher.AmountTransacted, 2);
                                        shareIds.Add(voucher.Id);
                                    });
                                else
                                    shareIds.Add(0);
                            }
                        });
                    });
                });

                var loanVoucherCode = await codeGiver(VoucherType.Yearly_Loan_Bigining_Balance.Name, organizationId);
                var loanVoucher = new Voucher(loanVoucherCode, VoucherType.Yearly_Loan_Bigining_Balance.Id, DateTime.Now,
                    loans, organizationId, user.Email);

                var saveVoucherCode = await codeGiver(VoucherType.Yearly_Loan_Bigining_Balance.Name, organizationId);
                var saveVoucher = new Voucher(saveVoucherCode, VoucherType.Yearly_Save_Bigining_Balance.Id, DateTime.Now,
                    saves, organizationId, user.Email);

                var shareVoucherCode = await codeGiver(VoucherType.Yearly_Loan_Bigining_Balance.Name, organizationId);
                var shareVoucher = new Voucher(shareVoucherCode, VoucherType.Yearly_Share_Bigining_Balance.Id, DateTime.Now,
                    shares, organizationId, user.Email);

                await _voucherRepo.AddAsync(loanVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(saveVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                await _voucherRepo.AddAsync(shareVoucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                foreach (var loanId in loanIds)
                {
                    var loanVoucherReference = new VoucherReference(loanVoucher.Id, loanId, user.Email);

                    await AddAsync(loanVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var saveId in saveIds)
                {
                    var saveVoucherReference = new VoucherReference(saveVoucher.Id, saveId, user.Email);

                    await AddAsync(saveVoucherReference);
                    await UnitOfWork.SaveChanges();
                }

                foreach (var shareId in shareIds)
                {
                    var shareVoucherReference = new VoucherReference(shareVoucher.Id, shareId, user.Email);

                    await AddAsync(shareVoucherReference);
                    await UnitOfWork.SaveChanges();
                }
            }
        }
    }
}