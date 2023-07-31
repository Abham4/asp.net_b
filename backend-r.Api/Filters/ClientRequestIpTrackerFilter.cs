namespace backend_r.Api.Filters
{
    public class ClientRequestIpTrackerFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<ClientRequestIpTracker>();
                next(builder);
            };
        }
    }
}