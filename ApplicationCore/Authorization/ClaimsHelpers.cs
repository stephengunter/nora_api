using ApplicationCore.Helpers;
using System.Security.Claims;
using System.Security.Principal;
using ApplicationCore.Models;
using ApplicationCore.Consts;

namespace ApplicationCore.Authorization;
public static class ClaimsHelpers
{
	public static IEnumerable<Claim> CreateClaims(this User user, IList<string> roles, OAuth? oAuth = null)
	{
		string id = user.Id;
		string userName = user.UserName!;
		var identity = GenerateClaimsIdentity(id, userName);
		var claims = new List<Claim>
		{
			identity.FindFirst(JwtClaimIdentifiers.Rol)!,
			identity.FindFirst(JwtClaimIdentifiers.Id)!,
			new Claim(JwtClaimIdentifiers.Sub, userName),
			new Claim(JwtClaimIdentifiers.Roles, roles.JoinToString())
		};

		if (oAuth != null)
		{
			claims.Add(new Claim(JwtClaimIdentifiers.Provider, oAuth.Provider.ToString()));
			claims.Add(new Claim(JwtClaimIdentifiers.Picture, oAuth.PictureUrl.GetString()));
			claims.Add(new Claim(JwtClaimIdentifiers.Name, oAuth.GivenName.GetString()));
		}
		else 
		{
         claims.Add(new Claim(JwtClaimIdentifiers.Name, user.Name));
      }
		
		return claims;
	}

	private static ClaimsIdentity GenerateClaimsIdentity(string id, string userName)
	{
		return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
		{
			new Claim(JwtClaimIdentifiers.Id, id),
			new Claim(JwtClaimIdentifiers.Rol, JwtClaims.ApiAccess)
		});
	}
	public static OAuthProvider Provider(this IEnumerable<Claim> claims)
	{
		string providerName = claims.Find(JwtClaimIdentifiers.Provider)?.Value ?? string.Empty;
		
		OAuthProvider provider;
		if(!Enum.TryParse(providerName, true, out provider)) return OAuthProvider.Unknown;
		return provider;
	}
	
	public static string UserId(this IEnumerable<Claim> claims)
		=> claims.Find(JwtClaimIdentifiers.Id)?.Value ?? string.Empty;
	
	public static IEnumerable<string> Roles(this IEnumerable<Claim> claims)
		=> claims.Find(JwtClaimIdentifiers.Roles)?.Value.SplitToList() 
			?? new List<string>();		

	public static string UserName(this IEnumerable<Claim> claims)
		=> claims.Find(JwtClaimIdentifiers.Sub)?.Value ?? string.Empty;

	public static bool IsDev(this IEnumerable<Claim> claims)
	{
		if(Roles(claims).IsNullOrEmpty()) return false;

		return Roles(claims).First(r => r.EqualTo(AppRoles.Dev.ToString())) != null;
	}
	public static bool IsBoss(this IEnumerable<Claim> claims)
	{
		if(Roles(claims).IsNullOrEmpty()) return false;

		return Roles(claims).First(r => r.EqualTo(AppRoles.Boss.ToString())) != null;
	}

	static Claim? Find(this IEnumerable<Claim> claims, string val) 
		=> claims.FirstOrDefault(c => c.Type.EqualTo(val));

}
