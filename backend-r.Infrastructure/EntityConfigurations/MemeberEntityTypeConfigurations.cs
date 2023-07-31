namespace backend_r.Infrastructure.EntityConfigurations
{
    public class MemeberEntityTypeConfigurations : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}