namespace backend_r.Infrastructure.EntityConfigurations
{
    public class ResourceEntityTypeConfigurations : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.Property(c => c.Type)
                .IsRequired();

            builder.HasIndex(c => c.Type)
                .IsUnique();
        }
    }
}