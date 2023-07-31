namespace backend_r.Infrastructure.EntityConfigurations
{
    public class ProductEntityTypeConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(c => c.LoanExtension)
                .WithOne(c => c.Product)
                .HasForeignKey<LoanExtension>(c => c.Id);
            
            builder.HasOne(c => c.SaveExtention)
                .WithOne(c => c.Product)
                .HasForeignKey<SaveExtension>(c => c.Id);
            
            builder.HasOne(c => c.ShareExtension)
                .WithOne(c => c.Product)
                .HasForeignKey<ShareExtension>(c => c.Id);
        }
    }
}