using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.News;

namespace Nop.Services.News
{
    /// <summary>
    /// News Category service interface
    /// </summary>
    public partial interface INewsCategoryService
    {
        /// <summary>
        /// Delete news category
        /// </summary>
        /// <param name="category">NewsCategory</param>
        void DeleteNewsCategory(NewsCategory newsCategory);

        /// <summary>
        /// Gets all news categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<NewsCategory> GetAllNewsCategories(string categoryName = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all news categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        IList<NewsCategory> GetAllNewsCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false);

        /// <summary>
        /// Gets a news category
        /// </summary>
        /// <param name="categoryId">NewsCategory identifier</param>
        /// <returns>NewsCategory</returns>
        NewsCategory GetNewsCategoryById(int categoryId);

        /// <summary>
        /// Inserts news category
        /// </summary>
        /// <param name="category">NewsCategory</param>
        void InsertNewsCategory(NewsCategory newsCategory);

        /// <summary>
        /// Updates the News category
        /// </summary>
        /// <param name="category">NewsCategory</param>
        void UpdateNewsCategory(NewsCategory newsCategory);

        /// <summary>
        /// Deletes a NewsItem category mapping
        /// </summary>
        /// <param name="productCategory">NewsItem category</param>
        void DeleteNewsItemCategory(NewsItemCategory newsItemCategory);

        /// <summary>
        /// Gets newsItem category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>s NewsItem category mapping collection</returns>
        IPagedList<NewsItemCategory> GetNewsItemCategoriesByCategoryId(int categoryId,
            int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets a newsItem category mapping collection
        /// </summary>
        /// <param name="productId">NewsItem identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>NewsItem category mapping collection</returns>
        IList<NewsItemCategory> GetNewsItemCategoriesByNewsId(int newsId, bool showHidden = false);

        /// <summary>
        /// Gets a newsItem category mapping 
        /// </summary>
        /// <param name="productCategoryId">NewsItem category mapping identifier</param>
        /// <returns>NewsItem category mapping</returns>
        NewsItemCategory GetNewsItemCategoryById(int newsItemCategoryId);

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void InsertNewsItemCategory(NewsItemCategory newsItemCategory);

        /// <summary>
        /// Updates the newsItem category mapping 
        /// </summary>
        /// <param name="productCategory">>NewsItem category mapping</param>
        void UpdateNewsItemCategory(NewsItemCategory newsItemCategory);
    }
}
