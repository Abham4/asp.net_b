namespace backend_r.Infrastructure.EntityConfigurations
{
    public class RoleEntityTypeConfigurations : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            
            builder.Property(c => c.Name)
                .IsRequired();

            builder.HasIndex(c => c.Name)
                .IsUnique();
            
            builder.HasMany(m => m.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            builder.HasMany(c => c.RoleClaims)
                .WithOne(c => c.Role)
                .HasForeignKey(c => c.RoleId)
                .IsRequired();
        }
    }
}