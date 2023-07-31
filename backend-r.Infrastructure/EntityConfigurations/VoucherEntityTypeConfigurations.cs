namespace backend_r.Infrastructure.EntityConfigurations
{
    public class VoucherEntityTypeConfigurations : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.Property(c => c.Code)
                .IsRequired();

            builder.HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}