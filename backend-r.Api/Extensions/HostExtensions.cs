namespace backend_r.Api.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder,
            int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("Database migration...");
                    InvokeSeeder(seeder, context, services);
                    logger.LogInformation("Migration succeed.");
                }
                catch (SqlException ex)
                {
                    logger?.LogError(ex, "An error occured while migrating the db");
                    throw;
                }
            }
            return host;
        }
        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
            IServiceProvider service) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, service);
        }
    }
}