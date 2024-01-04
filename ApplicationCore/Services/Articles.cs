﻿using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ApplicationCore.Consts;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IArticlesService
{
   Task<IEnumerable<Article>> FetchAsync(Category category);
   Task<IEnumerable<Article>> FetchAllAsync();
   Task<Article?> GetByIdAsync(int id);

   Task<Article> CreateAsync(Article Article);
	Task UpdateAsync(Article Article);
}

public class ArticlesService : IArticlesService
{
	private readonly IDefaultRepository<Article> _articlesRepository;

	public ArticlesService(IDefaultRepository<Article> articlesRepository)
	{
      _articlesRepository = articlesRepository;
	}
   public async Task<IEnumerable<Article>> FetchAsync(Category category)
      => await _articlesRepository.ListAsync(new ArticleSpecification(category));
   public async Task<IEnumerable<Article>> FetchAllAsync()
      => await _articlesRepository.ListAsync(new ArticleSpecification());

   public async Task<Article?> GetByIdAsync(int id)
      => await _articlesRepository.GetByIdAsync(id);

   public async Task<Article> CreateAsync(Article Article)
		=> await _articlesRepository.AddAsync(Article);

		public async Task UpdateAsync(Article Article)
		=> await _articlesRepository.UpdateAsync(Article);

}
