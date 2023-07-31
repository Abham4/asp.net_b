namespace backend_r.Infrastructure.EntityConfigurations
{
    public class RoleClaimEntityTypeConfigurations : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("RoleClaims");
        }
    }
}