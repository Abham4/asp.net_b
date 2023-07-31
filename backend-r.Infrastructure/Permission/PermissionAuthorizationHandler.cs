namespace backend_r.Infrastructure.Permission
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {            
            if (context.User == null)
                return Task.CompletedTask;
                
            var permission = context.User.Claims.Where(claim => claim.Type == "AuthorizedTo" && claim.Value ==
                requirement.Permission && claim.Issuer == "LOCAL AUTHORITY");

            if (permission.Any())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}