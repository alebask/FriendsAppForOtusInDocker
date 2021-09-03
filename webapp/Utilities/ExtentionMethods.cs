using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FriendsAppNoORM.Utilities
{
    public static class ExtentionMethods{

        public static long GetMyProfileId(this ClaimsPrincipal principal){

            Claim profileIdClaim = principal.Claims.SingleOrDefault(c => c.Type == "ProfileId");

            if(profileIdClaim != null){
                return long.Parse(profileIdClaim.Value);
            }
            return -1;
        }

        public static bool HasProfile (this ClaimsPrincipal principal){
            return principal.HasClaim(c => c.Type == "ProfileId");
        }
        public static Guid GetAccountId(this ClaimsPrincipal principal)
        {

            Claim accountIdClaim = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (accountIdClaim != null)
            {
                return new Guid(accountIdClaim.Value);
            }
            return Guid.Empty;
        }

        public static string GetAccountName(this ClaimsPrincipal principal)
        {

            Claim nameClaim = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name);

            if (nameClaim != null)
            {
                return nameClaim.Value;
            }
            return null;
        }
        public static string GetAccountEmail(this ClaimsPrincipal principal)
        {

            Claim emailClaim = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim != null)
            {
                return emailClaim.Value;
            }
            return null;
        }

        public static async Task SignInAsync(this HttpContext httpContext, Guid accountId, string accountName, long? profileId, bool isPersistent)
        {
            List<Claim> claims = new List<Claim>(){};
            
            if(accountId != Guid.Empty)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, accountId.ToString()));

            if (!String.IsNullOrEmpty(accountName))
                claims.Add(new Claim(ClaimTypes.Name, accountName));

            if (profileId.HasValue)
                claims.Add(new Claim("ProfileId", profileId.Value.ToString()));


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties() { IsPersistent= isPersistent });

        }

    }
}