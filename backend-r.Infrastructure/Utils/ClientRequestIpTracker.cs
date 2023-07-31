namespace backend_r.Infrastructure.Utils
{
    public class ClientRequestIpTracker
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClientRequestIpTracker> _logger;

        public ClientRequestIpTracker(RequestDelegate next, ILogger<ClientRequestIpTracker> logger, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress!.ToString();

            if(ip == "::1")
            {
                try
                {
                    _logger.LogWarning(string.Format("Server-Ip = {0}", Dns.GetHostEntry(Dns.GetHostName())
                        .AddressList.First(c => c.AddressFamily == AddressFamily.InterNetwork).ToString()));
                }
                catch (Exception exp)
                {
                    _logger.LogCritical(exp.ToString());
                }
            }

            else
            {
                var proxyCheck = new ProxyCheck{ IncludeASN = true, IncludeVPN = true };

                try
                {
                    var checkResult = await proxyCheck.QueryAsync(ip);

                    foreach (var item in checkResult.Results)
                    {
                        _logger.LogWarning(string.Format("Ip = {0}, IsVpn = {1}, Provider = {2}, Country = {3}, City = {4}, ASN = {5}, Latitude = {6}, Longitude = {7}, Proxy-Type = {8}", 
                            item.Key.ToString(), item.Value.IsProxy.ToString(), item.Value.Provider, item.Value.Country,
                                item.Value.City, item.Value.ASN, item.Value.Latitude.ToString(),
                                    item.Value.Longitude.ToString(), item.Value.ProxyType));
                    }
                }

                catch (Exception exp)
                {
                    _logger.LogCritical(exp.ToString());
                    _logger.LogWarning(string.Format("Non-Public Ip-Address {0}", ip));
                }
            }

            await _next(context);
        }
    }
}