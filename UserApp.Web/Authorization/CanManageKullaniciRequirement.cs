using Microsoft.AspNetCore.Authorization;

namespace UserApp.Web.Authorization
{
    /// <summary>
    /// "Bu kullanıcı, şu Kullanici kaydını düzenleyebilir/silebilir mi?"
    /// sorusunu temsil eden yetki gereksinimi. Edit ve Delete action'larında kullanılır.
    /// </summary>
    public class CanManageKullaniciRequirement : IAuthorizationRequirement
    {
    }
}