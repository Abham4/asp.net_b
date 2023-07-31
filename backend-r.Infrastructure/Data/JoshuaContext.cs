namespace backend_r.Infrastructure.Data
{
    public class JoshuaContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, 
        UserRole, IdentityUserLogin<int>, RoleClaim, IdentityUserToken<int>>
    {
        private readonly IMediator _mediator;
        public DbSet<RangeType> RangeTypes { get; set; }
        public DbSet<AccountProductType> AccountProductTypes { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountMap> AccountMaps { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AssignedStaff> AssignedStaffs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Spouse> Spouses { get; set; }
        public DbSet<Identity> Identities { get; set; }
        public DbSet<MemberOrganization> MemberOrganizations { get; set; }
        public DbSet<MemberCategory> MemberCategories { get; set; }
        public DbSet<MemberPassBook> MemberPassBooks { get; set; }
        public DbSet<Member> Members { get; set; }   
        public DbSet<PassBook> PassBooks { get; set; }
        public DbSet<StaffOrganization> StaffOrganizations { get; set;}
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<IdDefinition> IdDefinitions { get; set; }
        public DbSet<ObjectStateDefn> ObjectStateDefns { get; set; }
        public DbSet<ObjectState> ObjectStates { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<TransitionRule> TransitionRules { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Occupation> Occupations { get; set;}
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Collateral> Collaterals { get; set; }
        public DbSet<LoanExtension> LoanExtensions { get; set; }
        public DbSet<SaveExtension> SaveExtensions { get; set; }
        public DbSet<ShareExtension> ShareExtensions { get; set; }
        public DbSet<ProductSchedule> ProductSchedules { get; set; }
        public DbSet<ProductSetup> ProductSetups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchasedProduct> PurchasedProducts { get; set; }
        public DbSet<ProductRange> ProductRanges { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherType> VoucherTypes { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<VoucherReference> VoucherReferences { get; set; }
        public DbSet<ClosingPeriod> ClosingPeriods { get; set; }
        public DbSet<OrganizationType> OrganizationTypes { get; set; }

        public JoshuaContext(DbContextOptions<JoshuaContext> options) : base(options) {}

        public JoshuaContext(DbContextOptions<JoshuaContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserClaim<int>>()
                .ToTable("UserClaims");
            
            builder.Entity<IdentityUserLogin<int>>()
                .ToTable("UserLogins");
            
            builder.Entity<IdentityUserToken<int>>()
                .ToTable("UserTokens");
            
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;
                    
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<Role>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<User>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<UserRole>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}