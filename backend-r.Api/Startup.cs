namespace backend_r.Api;
public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IStartupFilter, ClientRequestIpTrackerFilter>();

        services.AddInfrastructureServices(Configuration);

        services.AddApplicationServices();

        services.AddControllers(c =>
        {
            c.Filters.Add(typeof(HttpGlobalExceptionFilter));
        });

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<JoshuaContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(op =>
            {
                // op.ExpireTimeSpan = TimeSpan.FromMinutes(0.1);
                // op.Cookie.MaxAge = TimeSpan.FromMinutes(5);
                op.SlidingExpiration = true;
                op.AccessDeniedPath = "/Forbidden/";
                op.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = redirect =>
                    {
                        redirect.HttpContext.Response.StatusCode = 401;
                        redirect.HttpContext.Response.WriteAsync("UnAuthenticated!");

                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = response =>
                    {
                        response.HttpContext.Response.StatusCode = 403;
                        response.HttpContext.Response.WriteAsync("UnAuthorized!");

                        return Task.CompletedTask;
                    }
                };
                op.Cookie.Name = "Joshua";
                // for https
                // op.Cookie.SameSite = SameSiteMode.None;
            });

        services.AddCors(b =>
        {
            b.AddDefaultPolicy(
            builder =>
            {
                builder.SetIsOriginAllowed(IsAllowed)
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
            });
        });

        services.AddMvc()
            .AddNewtonsoftJson(op =>
            {
                op.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(d => d.FullName);
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Joshua", Version = "v1" });
        });

        // services.AddCoreAdmin();

        // Used to Configure the server access to all networked device but for the https case it lacks adding certificate

        // builder.WebHost.ConfigureKestrel((context, options) => {
        //     options.ListenAnyIP(5001, listenOptions => {
        //         listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        //         listenOptions.UseHttps();
        //     });
        // });
    }

    private static bool IsAllowed(string origin)
    {
        var allowedList = new string[] { "http://localhost:3000", "http://196.189.119.71:80", "http://192.168.7.5:80" };

        return allowedList.Any(c => c == origin);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // used for https
        // var cookiePolicyOptions = new CookiePolicyOptions
        // {
        //     Secure = CookieSecurePolicy.Always
        // };

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Joshua v1"));
        }

        app.UseCors();

        app.Use(async (context, next) =>
        {
            await next();

            var host = context.Request.Host;

            if (context.Response.StatusCode == 302)
                context.Response.Redirect($"http://{host}/api/User/NotExist");
        });

        // app.UseStaticFiles();

        if (!Directory.Exists("Files"))
            Directory.CreateDirectory("./Files");
        if (!Directory.Exists("Files/Attachments"))
            Directory.CreateDirectory("./Files/Attachments");
        if (!Directory.Exists("Files/Member_Pictures"))
            Directory.CreateDirectory("./Files/Member_Pictures");
        if (!Directory.Exists("Files/Staff_Pictures"))
            Directory.CreateDirectory("./Files/Staff_Pictures");
        if (!Directory.Exists("Files/Signature"))
            Directory.CreateDirectory("./Files/Signature");

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Files")),
            RequestPath = "/Files"
        });

        // used for https
        // app.UseCookiePolicy(cookiePolicyOptions);

        // app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(c =>
        {
            c.MapControllers();
        });

        // app.UseCoreAdminCustomTitle("Joshua Multimedia Admin Portal");

        // app.UseCoreAdminCustomUrl("superadmin");
    }
}