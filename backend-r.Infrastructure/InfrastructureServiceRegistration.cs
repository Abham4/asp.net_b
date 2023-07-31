namespace backend_r.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("JoshuaDb");

            services.AddDbContext<JoshuaContext>(op => {
                op.UseMySql(connection, ServerVersion.AutoDetect(connection));
            });

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISpouseRepository, SpouseRepository>();
            services.AddScoped<IIdDefinitionRepository, IdDefinitionRepository>();
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IObjectStateRepository, ObjectStateRepository>();
            services.AddScoped<IObjectStateDefnRepository, ObjectStateDefnRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEducationRepository, EducationRepository>();
            services.AddScoped<IAccountMapRepository, AccountMapRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductRangeRepository, ProductRangeRepository>();
            services.AddScoped<IPurchasedProductRepository, PurchasedProductRepository>();
            services.AddScoped<IProductScheduleRepository, ProductScheduleRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IScheduleHeaderRepository, ScheduleHeaderRepository>();
            services.AddScoped<IVoucherReferenceRepository, VoucherRefenceRepository>();
            services.AddScoped<IPassBookRepository, PassBookRepository>();
            services.AddScoped<IGuardianRepository, GuardianRepository>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            
            return services;
        }
    }
}