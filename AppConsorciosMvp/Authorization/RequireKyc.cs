using Microsoft.AspNetCore.Authorization;

namespace AppConsorciosMvp.Authorization
{
    public class RequireKycRequirement : IAuthorizationRequirement
    {
        public int Level { get; }
        public RequireKycRequirement(int level) => Level = level;
    }

    public class RequireKycHandler : AuthorizationHandler<RequireKycRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireKycRequirement requirement)
        {
            var claim = context.User.FindFirst(c => c.Type == "KycLevel")?.Value;
            if (int.TryParse(claim, out var level) && level >= requirement.Level)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
