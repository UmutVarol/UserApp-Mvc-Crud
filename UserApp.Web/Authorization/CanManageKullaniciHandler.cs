using Microsoft.AspNetCore.Authorization;
using UserApp.Entities.Dtos;

namespace UserApp.Web.Authorization
{
    /// Admin: her zaman izinli. DepartmanYoneticisi: sadece kendi departmanı.
    /// User: bu policy üzerinden asla izinli değil (kendi profili ayrı akıştan geçer).
    public class CanManageKullaniciHandler
        : AuthorizationHandler<CanManageKullaniciRequirement, KullaniciAccessInfo>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanManageKullaniciRequirement requirement,
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
                }
            }

            return Task.CompletedTask;
        }
    }
}