namespace backend_r.Infrastructure.EntityConfigurations
{
    public class OrganizationEntityTypeConfigurations : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired();
                
            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.Property(c => c.ShortName)
                .IsRequired();

            builder.HasIndex(c => c.ShortName)
                .IsUnique();
        }
    }
}