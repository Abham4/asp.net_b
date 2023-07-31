namespace backend_r.Infrastructure.EntityConfigurations
{
    public class PassBookEntityTypeConfigurations : IEntityTypeConfiguration<PassBook>
    {
        public void Configure(EntityTypeBuilder<PassBook> builder)
        {
            builder.HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}