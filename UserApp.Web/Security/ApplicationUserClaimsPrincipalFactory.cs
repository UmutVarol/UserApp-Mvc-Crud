using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserApp.Entities;

namespace UserApp.Web.Security
{
    /// <summary>
    /// Giriş sırasında ApplicationUser'daki KullaniciId ve YonetilenDepartmanId
    /// alanlarını cookie'ye claim olarak gömer — Authorization Handler'lar her
    /// istekte veritabanına gitmesin diye (performans).
    /// </summary>
    public class ApplicationUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user.KullaniciId.HasValue)
            {
                identity.AddClaim(new Claim("KullaniciId", user.KullaniciId.Value.ToString()));
            }

            if (user.YonetilenDepartmanId.HasValue)
            {
                identity.AddClaim(new Claim("YonetilenDepartmanId", user.YonetilenDepartmanId.Value.ToString()));
            }

            return identity;
        }
    }
}