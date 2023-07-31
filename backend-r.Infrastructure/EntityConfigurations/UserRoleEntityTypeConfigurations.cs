namespace backend_r.Infrastructure.EntityConfigurations
{
    public class UserRoleEntityTypeConfigurations : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");
        }
    }
}