using Microsoft.AspNetCore.Authorization;

namespace UserApp.Web.Authorization
{
    /// "Bu kullanıcı, şu Kullanici kaydının detayını görüntüleyebilir mi?"
    public class CanViewKullaniciDetailRequirement : IAuthorizationRequirement
    {
    }
}