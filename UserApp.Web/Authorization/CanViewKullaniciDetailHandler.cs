using Microsoft.AspNetCore.Authorization;
using UserApp.Entities.Dtos;

namespace UserApp.Web.Authorization
{
    public class CanViewKullaniciDetailHandler
        : AuthorizationHandler<CanViewKullaniciDetailRequirement, KullaniciAccessInfo>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanViewKullaniciDetailRequirement requirement,
            KullaniciAccessInfo resource)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.IsInRole("DepartmanYoneticisi"))
            {
                var claimValue = context.User.FindFirst("YonetilenDepartmanId")?.Value;
                if (int.TryParse(claimValue, out var yonetilenDepartmanId)
                    && yonetilenDepartmanId == resource.DepartmanId)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            // User rolü: sadece AKTİF kayıtları görebilir (rehber mantığı).
            if (context.User.IsInRole("User") && resource.IsActive)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
