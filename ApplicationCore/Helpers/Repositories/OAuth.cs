﻿using ApplicationCore.Consts;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class OAuthRepositoryHelpers
{
	public static async Task<OAuth?> FindByProviderAsync(this IDefaultRepository<OAuth> oAuthRepository, User user, OAuthProvider provider)
		=> await oAuthRepository.FirstOrDefaultAsync(new OAuthSpecification(user, provider));


}