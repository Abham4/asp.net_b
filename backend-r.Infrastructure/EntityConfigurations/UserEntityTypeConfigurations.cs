namespace backend_r.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            
            builder.HasIndex(m => m.Email)
                .IsUnique();

            builder.HasIndex(m => m.PhoneNumber)
                .IsUnique();
            
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            
            builder.HasOne(c => c.Staff)
                .WithOne(c => c.User)
                .HasForeignKey<Staff>(c => c.UserId)
                .IsRequired(false);

            builder.HasOne(c => c.Member)
                .WithOne(c => c.User)
                .HasForeignKey<Member>(c => c.UserId)
                .IsRequired(false);
        }
    }
}