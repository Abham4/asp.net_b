namespace backend_r.Infrastructure.Data
{
    public class JoshuaContextDesignFactory : IDesignTimeDbContextFactory<JoshuaContext>
    {
        public JoshuaContext CreateDbContext(string[] args)
        {
            var connection = "Server=localhost;Database=JoshuaMultimedia;User=root;Password=p@ssw0rd;Port=3306;";

            var builderOptions = new DbContextOptionsBuilder<JoshuaContext>()
                .UseMySql(connection, ServerVersion.AutoDetect(connection));

            return new JoshuaContext(builderOptions.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
                CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where
                TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(default(object));
            }
        }
    }
}