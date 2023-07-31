namespace backend_r.Infrastructure.Data
{
    public class JoshuaContextSeed
    {
        public static async Task SeedAsync(JoshuaContext context, ILogger<JoshuaContextSeed> logger)
        {
            if (!context.ClosingPeriods.Any())
                context.ClosingPeriods.AddRange(ClosingPeriod.List());

            if (!context.VoucherTypes.Any())
                context.VoucherTypes.AddRange(VoucherType.List());

            if (!context.RangeTypes.Any())
                context.RangeTypes.AddRange(RangeType.List());

            if (!context.AccountProductTypes.Any())
                context.AccountProductTypes.AddRange(AccountProductType.List());

            if (!context.Genders.Any())
                context.Genders.AddRange(Gender.List());

            if (!context.WorkTypes.Any())
                context.WorkTypes.AddRange(WorkType.List());

            if (!context.OrganizationTypes.Any())
                context.OrganizationTypes.AddRange(OrganizationType.List());

            logger.LogInformation("Seeding Resource Table.");

            if (!context.Resources.Any())
                context.Resources.AddRange(Resources());

            logger.LogInformation("Seeding Organization Table.");

            if (!context.Organizations.Any())
                context.Organizations.AddRange(Organization());

            logger.LogInformation("Seeding Role Table.");

            if (!context.Roles.Any())
                context.Roles.AddRange(Roles());

            logger.LogInformation("Seeding Role Claim Table.");

            if (!context.RoleClaims.Any())
                context.RoleClaims.AddRange(RoleClaims());

            logger.LogInformation("Seeding User Table.");

            if (!context.Users.Any())
                context.Users.AddRange(Users());

            logger.LogInformation("Seeding IdDefinition Table.");

            if (!context.IdDefinitions.Any())
                context.IdDefinitions.AddRange(IdDefinitions());

            logger.LogInformation("Seeding Object State Definition Table.");

            if (!context.ObjectStateDefns.Any())
                context.ObjectStateDefns.AddRange(ObjectStates());

            logger.LogInformation("Seeding Address Table.");

            if (!context.Addresses.Any())
                context.Addresses.AddRange(Address());

            await context.SaveChangesAsync();
        }

        public static List<Role> Roles()
        {
            Role role1 = new Role(1, "General Manager", "General Manager".ToUpper(),
                DateTime.Now, "Seeding");
            Role role2 = new Role(2, "Branch Manager", "Branch Manager".ToUpper(),
                DateTime.Now, "Seeding");
            Role role3 = new Role(3, "Operation Manager", "Operation Manager"
                .ToUpper(), DateTime.Now, "Seeding");
            Role role4 = new Role(4, "Admin", "Admin".ToUpper(), DateTime.Now, "Seeding");
            Role role5 = new Role(5, "Cashier", "Cashier".ToUpper(), DateTime.Now, "Seeding");
            Role role6 = new Role(6, "Promotion Officer", "Promotion Officer"
                .ToUpper(), DateTime.Now, "Seeding");

            return new List<Role> { role1, role2, role3, role4, role5, role6 };
        }

        private enum Entity
        {
            Organization,
            Member,
            PassBook,
            Product,
            PurchasedProduct,
            Role,
            Staff,
            User,
            Voucher
        }

        private static List<RoleClaim> RoleClaims()
        {
            var generalRoleClaims = new List<RoleClaim>();
            var adminRoleClaims = new List<RoleClaim>();
            var operationRoleClaims = new List<RoleClaim>();
            var promotionRoleClaims = new List<RoleClaim>();
            var branchManagerRoleClaims = new List<RoleClaim>();
            var cashierRoleClaims = new List<RoleClaim>();
            var all = new List<RoleClaim>();

            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Member.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".Add"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".Edit"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".MemberGrowthsSummaryHigher"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".MemberCarrersSummaryHigher"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".ClosedTransaction"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Product.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Role.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Staff.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.User.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".View"));
            generalRoleClaims.Add(new RoleClaim(Roles()[0].Id, "AuthorizedTo." + Entity.Voucher.ToString() +
                ".SummaryHighers"));

            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Member.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Unregistered"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".Edit"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".MemberGrowthsSummaryHigher"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".MemberCarrersSummaryHigher"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Product.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Role.ToString() + ".Add"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Role.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Role.ToString() + ".Edit"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Staff.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Staff.ToString() + ".Unregistered"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.User.ToString() + ".Add"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.User.ToString() + ".Edit"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.User.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".View"));
            adminRoleClaims.Add(new RoleClaim(Roles()[3].Id, "AuthorizedTo." + Entity.Voucher.ToString() +
                ".SummaryHighers"));

            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Member.ToString() + ".ByState"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Activate"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".Terminate"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".Add"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".Edit"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".View"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".MemberGrowthsSummaryHigher"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Organization.ToString() +
                ".MemberCarrersSummaryHigher"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.PassBook.ToString() + ".View"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Product.ToString() + ".Add"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Product.ToString() + ".Edit"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Product.ToString() + ".View"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Staff.ToString() + ".Add"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Staff.ToString() + ".Edit"));
            operationRoleClaims.Add(new RoleClaim(Roles()[2].Id, "AuthorizedTo." + Entity.Staff.ToString() + ".View"));

            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".View"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Add"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Edit"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() + ".View"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".GrowthSummary"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".CarrerSummary"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Close"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".ClosedTransaction"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".ShowTransaction"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.PassBook.ToString() + ".View"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Product.ToString() + ".View"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".Add"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".View"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".View"));
            promotionRoleClaims.Add(new RoleClaim(Roles()[5].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".Summary"));

            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Organization.ToString() + ".View"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Add"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() + ".Edit"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() + ".View"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".GrowthSummary"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".CarrerSummary"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".Close"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".ClosedTransaction"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".ShowTransaction"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".ByState"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".Activate"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".Terminate"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.PassBook.ToString() +
                ".View"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".Add"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".View"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Voucher.ToString() +
                ".View"));
            branchManagerRoleClaims.Add(new RoleClaim(Roles()[1].Id, "AuthorizedTo." + Entity.Voucher.ToString() +
                ".Summary"));

            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.Member.ToString() + ".View"));
            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".GrowthSummary"));
            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.Member.ToString() +
                ".CarrerSummary"));
            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.PurchasedProduct.ToString() +
                ".View"));
            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".Add"));
            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".View"));
            cashierRoleClaims.Add(new RoleClaim(Roles()[4].Id, "AuthorizedTo." + Entity.Voucher.ToString() + ".Summary"));

            all.AddRange(generalRoleClaims);
            all.AddRange(adminRoleClaims);
            all.AddRange(operationRoleClaims);
            all.AddRange(branchManagerRoleClaims);
            all.AddRange(promotionRoleClaims);
            all.AddRange(cashierRoleClaims);

            return all;
        } 

        public static List<User> Users()
        {
            User admin = new User(1, "admin@eagle.com", "admin@eagle.com".ToUpper(),
                "+251", "Admin", "Admin".ToUpper(), DateTime.Now, new Domain.Entities.Staff(1, "Admin", "Admin", "Seeding"),
                    "Seeding", Guid.NewGuid().ToString());

            User operation = new User(2, "operation@eagle.com", "operation@eagle.com".ToUpper(), "+2519", "Operation",
                "Operation".ToUpper(), DateTime.Now, new Domain.Entities.Staff(2, "Operation", "Manager", "Seeding"),
                    "Seeding", Guid.NewGuid().ToString());

            User promotion = new User(3, "promotion@eagle.com", "promotion@eagle.com".ToUpper(), "+25199", "Promotion",
                "Promotion".ToUpper(), DateTime.Now, new Domain.Entities.Staff(3, "Promotion", "Manager", Organization().Id,
                    "Seeding"), "Seeding", Guid.NewGuid().ToString());

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(admin, "EaglE@111");
            
            admin.PasswordHash = hashedPassword;
            admin.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    RoleId = Roles()[3].Id,
                    CreatedDate = DateTime.Now
                }
            };

            operation.PasswordHash = hashedPassword;
            operation.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    RoleId = Roles()[2].Id,
                    CreatedDate = DateTime.Now
                }
            };

            promotion.PasswordHash = hashedPassword;
            promotion.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    RoleId = Roles()[5].Id,
                    CreatedDate = DateTime.Now
                }
            };

            return new List<User> { admin, promotion, operation };
        }

        public static List<Resource> Resources()
        {
            Resource member = new Resource(1, "Member", "member", DateTime.Now, "Seeding");
            Resource staff = new Resource(2, "Staff", "staff", DateTime.Now, "Seeding");
            Resource organization = new Resource(3, "Organization", "organization", DateTime.Now, "Seeding");
            Resource loan = new Resource(4, "Loan", "loan", DateTime.Now, "Seeding");
            Resource save = new Resource(5, "Share", "share", DateTime.Now, "Seeding");
            Resource share = new Resource(6, "Save", "save", DateTime.Now, "Seeding");

            // Voucher Types as a resource
            Resource loanPrincipal = new Resource(7, VoucherType.Loan_Principal.Name, VoucherType
                .Loan_Principal.Name.ToLower(), DateTime.Now, "Seeding");
            Resource savePrincipal = new Resource(8, VoucherType.Save_Principal.Name, VoucherType
                .Save_Principal.Name.ToLower(), DateTime.Now, "Seeding");
            Resource sharePrincipal = new Resource(9, VoucherType.Share_Principal.Name, VoucherType
                .Share_Principal.Name.ToLower(), DateTime.Now, "Seeding");
            Resource loanInterest = new Resource(10, VoucherType.Loan_Interest.Name, VoucherType
                .Loan_Interest.Name.ToLower(), DateTime.Now, "Seeding");
            Resource saveInterest = new Resource(11, VoucherType.Save_Interest.Name, VoucherType
                .Save_Interest.Name.ToLower(), DateTime.Now, "Seeding");
            Resource shareInterest = new Resource(12, VoucherType.Share_Interest.Name, VoucherType
                .Share_Interest.Name.ToLower(), DateTime.Now, "Seeding");
            Resource loanPenality = new Resource(13, VoucherType.Loan_Penality.Name, VoucherType
                .Loan_Penality.Name.ToLower(), DateTime.Now, "Seeding");
            Resource savePenality = new Resource(14, VoucherType.Save_Penality.Name, VoucherType
                .Save_Penality.Name.ToLower(), DateTime.Now, "Seeding");
            Resource sharePenality = new Resource(15, VoucherType.Share_Penality.Name, VoucherType
                .Share_Penality.Name.ToLower(), DateTime.Now, "Seeding");
            Resource deposit = new Resource(16, VoucherType.Deposit.Name, VoucherType.Deposit.Name.ToLower(),
                DateTime.Now, "Seeding");
            Resource withdrawal = new Resource(17, VoucherType.Withdrawal.Name, VoucherType.Withdrawal.Name.ToLower(),
                DateTime.Now, "Seeding");
            Resource dailyLoan = new Resource(18, VoucherType.Daily_Loan_Bigining_Balance.Name, VoucherType
                .Daily_Loan_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource dailySave = new Resource(19, VoucherType.Daily_Save_Bigining_Balance.Name, VoucherType
                .Daily_Save_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource dailyShare = new Resource(20, VoucherType.Daily_Share_Bigining_Balance.Name, VoucherType
                .Daily_Share_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource monthlyLoan = new Resource(21, VoucherType.Monthly_Loan_Bigining_Balance.Name, VoucherType
                .Monthly_Loan_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource monthlySave = new Resource(22, VoucherType.Monthly_Save_Bigining_Balance.Name, VoucherType
                .Monthly_Save_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource monthlyShare = new Resource(23, VoucherType.Monthly_Share_Bigining_Balance.Name, VoucherType
                .Monthly_Share_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource yearlyLoan = new Resource(24, VoucherType.Yearly_Loan_Bigining_Balance.Name, VoucherType
                .Yearly_Loan_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource yearlySave = new Resource(25, VoucherType.Yearly_Save_Bigining_Balance.Name, VoucherType
                .Yearly_Save_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource yearlyShare = new Resource(26, VoucherType.Yearly_Share_Bigining_Balance.Name, VoucherType
                .Yearly_Share_Bigining_Balance.Name.ToLower(), DateTime.Now, "Seeding");
            Resource disbursement = new Resource(27, VoucherType.Disbursement.Name, VoucherType.Disbursement.Name.ToLower(),
                DateTime.Now, "Seeding");

            return new List<Resource> { member, staff, organization, loan, save, share, loanPrincipal, savePrincipal,
                sharePrincipal, loanInterest, saveInterest, shareInterest, loanPenality, savePenality, sharePenality,
                deposit, withdrawal, dailyLoan, dailySave, dailyShare, monthlyLoan, monthlySave, monthlyShare, yearlyLoan,
                yearlySave, yearlyShare, disbursement };
        }

        public static Organization Organization()
        {
            return new Organization(1, "Head Office", "Head Office", OrganizationType.Root.Id, 0, false, DateTime.Now, "HOF",
                "Seeding");
        }

        public static List<IdDefinition> IdDefinitions()
        {
            var memberIdDefn = new IdDefinition(1, Resources()[0].Id, Organization().Id, "Mem", "*", "/", "ber", 1,
                DateTime.Now, 6, "Sedding");
            var staffIdDefn = new IdDefinition(2, Resources()[1].Id, Organization().Id, "Sta", "*", "/", "ff", 1,
                DateTime.Now, 6, "Sedding");
            var loanPrincipal = new IdDefinition(3, Resources()[6].Id, Organization().Id, "LOPR", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Sedding");
            var savePrincipal = new IdDefinition(4, Resources()[7].Id, Organization().Id, "SAPR", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Sedding");
            var sharePrincipal = new IdDefinition(5, Resources()[8].Id, Organization().Id, "SHPR", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Sedding");
            var loaninterest = new IdDefinition(6, Resources()[9].Id, Organization().Id, "LOIN", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Sedding");
            var saveinterest = new IdDefinition(7, Resources()[10].Id, Organization().Id, "SAIN", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Sedding");
            var shareinterest = new IdDefinition(8, Resources()[11].Id, Organization().Id, "SHIN", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Sedding");
            var loanPenality = new IdDefinition(9, Resources()[12].Id, Organization().Id, "LOPE", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var savePenality = new IdDefinition(10, Resources()[13].Id, Organization().Id, "SAPE", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var sharePenality = new IdDefinition(11, Resources()[14].Id, Organization().Id, "SHPE", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var depositVoucher = new IdDefinition(12, Resources()[15].Id, Organization().Id, "SADP", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var withdrawVoucher = new IdDefinition(13, Resources()[16].Id, Organization().Id, "SAWD", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var dailyLoan = new IdDefinition(14, Resources()[17].Id, Organization().Id, "LODB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var monthlyLoan = new IdDefinition(15, Resources()[18].Id, Organization().Id, "LOMB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var yearlyLoan = new IdDefinition(16, Resources()[19].Id, Organization().Id, "LOYB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var dailySave = new IdDefinition(17, Resources()[20].Id, Organization().Id, "SADB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var monthlySave = new IdDefinition(18, Resources()[21].Id, Organization().Id, "SAMB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var yearlySave = new IdDefinition(19, Resources()[22].Id, Organization().Id, "SAYB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var dailyShare = new IdDefinition(20, Resources()[23].Id, Organization().Id, "SHDB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var monthlyShare = new IdDefinition(21, Resources()[24].Id, Organization().Id, "SHMB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var yearlyShare = new IdDefinition(22, Resources()[25].Id, Organization().Id, "SHYB", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");
            var disbursement = new IdDefinition(23, Resources()[26].Id, Organization().Id, "LODS", "-", "-",
                Organization().ShortName, 1, DateTime.Now, 7, "Seeding");

            return new List<IdDefinition> { memberIdDefn, staffIdDefn, loanPrincipal, loaninterest, loanPenality,
                savePrincipal, saveinterest, savePenality, sharePrincipal, shareinterest, sharePenality, depositVoucher,
                withdrawVoucher, dailyLoan, dailySave, dailyShare, monthlyLoan, monthlySave, monthlyShare, yearlyLoan,
                yearlySave, yearlyShare, disbursement };
        }

        public static Address Address()
        {
            return new Address(1, "Organization", Organization().Id, "resedence", "Default", "Default",
                DateTime.Now, "Seeding");
        }

        public static List<ObjectStateDefn> ObjectStates()
        {
            var organizationObjectStateDefn1 = new ObjectStateDefn(1, Resources()[2].Id, ObjectStateEnumeration.Created
                .Name, "Seeding");
            var organizationObjectStateDefn2 = new ObjectStateDefn(2, Resources()[2].Id, ObjectStateEnumeration.Active
                .Name, "Seeding");
            var organizationObjectStateDefn3 = new ObjectStateDefn(3, Resources()[2].Id, ObjectStateEnumeration.Closed
                .Name, "Seeding");

            var memberObjectStateDefn1 = new ObjectStateDefn(4, Resources()[0].Id, ObjectStateEnumeration.Created.Name, "Seeding");
            var memberObjectStateDefn2 = new ObjectStateDefn(5, Resources()[0].Id, ObjectStateEnumeration.Active.Name, "Seeding");
            var memberObjectStateDefn3 = new ObjectStateDefn(6, Resources()[0].Id, ObjectStateEnumeration.Terminated.Name, "Seeding");

            var staffObjectStateDefn1 = new ObjectStateDefn(7, Resources()[1].Id, ObjectStateEnumeration.Created.Name, "Seeding");
            var staffObjectStateDefn2 = new ObjectStateDefn(8, Resources()[1].Id, ObjectStateEnumeration.Active.Name, "Seeding");
            var staffObjectStateDefn3 = new ObjectStateDefn(9, Resources()[1].Id, ObjectStateEnumeration.Terminated.Name, "Seeding");

            var loanObjectStateDefn1 = new ObjectStateDefn(10, Resources()[3].Id, ObjectStateEnumeration.Active.Name, "Seeding");
            var loanObjectStateDefn2 = new ObjectStateDefn(11, Resources()[3].Id, ObjectStateEnumeration.Archived.Name, "Seeding");

            var shareObjectStateDefn1 = new ObjectStateDefn(12, Resources()[4].Id, ObjectStateEnumeration.Active.Name, "Seeding");
            var shareObjectStateDefn2 = new ObjectStateDefn(13, Resources()[4].Id, ObjectStateEnumeration.Archived.Name, "Seeding");

            var saveObjectStateDefn1 = new ObjectStateDefn(14, Resources()[5].Id, ObjectStateEnumeration.Active.Name, "Seeding");

            return new List<ObjectStateDefn> {organizationObjectStateDefn1,
                organizationObjectStateDefn2, organizationObjectStateDefn3,
                memberObjectStateDefn1, memberObjectStateDefn2, memberObjectStateDefn3,
                staffObjectStateDefn1, staffObjectStateDefn2, staffObjectStateDefn3,
                loanObjectStateDefn1, loanObjectStateDefn2, shareObjectStateDefn1, shareObjectStateDefn2,
                saveObjectStateDefn1 };
        }
    }
}