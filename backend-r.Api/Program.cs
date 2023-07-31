try
{
    var host = CreateHostBuilder(args).Build();
    Log.Information("Applying Migration ....");
    host.MigrateDatabase<JoshuaContext>((context, services) =>
    {
        var logger = services.GetService<ILogger<JoshuaContextSeed>>();
        JoshuaContextSeed.SeedAsync(context, logger).Wait();
    });
    Log.Information("Starting Host ...");
    host.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected Error");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });