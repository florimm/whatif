using System.Security.Claims;
using System.Security.Principal;

namespace WhatIf.Api.Services
{
    public interface ICurrentUserService {
        string GetUserId();
        string GetEmail();
    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal principal;
        public CurrentUserService(IPrincipal principal)
        {
            this.principal = (ClaimsPrincipal)principal;
        }

        public string GetUserId()
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string GetEmail()
        {
            return principal.FindFirstValue(ClaimTypes.Email);
        }
    }
}