using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.News;

namespace Nop.Services.News
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class NewsCategoryExtensions
    {
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public static IList<NewsCategory> SortCategoriesForTree(this IList<NewsCategory> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<NewsCategory>();

            foreach (var cat in source.ToList().FindAll(c => c.ParentCategoryId == parentId))
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTree(source, cat.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                //find categories without parent in provided category source and insert them into result
                foreach (var cat in source)
                    if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                        result.Add(cat);
            }
            return result;
        }

        /// <summary>
        /// Returns a ProductCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>A ProductCategory that has the specified values; otherwise null</returns>
        public static NewsItemCategory FindNewsItemCategory(this IList<NewsItemCategory> source,
            int newsId, int categoryId)
        {
            foreach (var newsItemCategory in source)
                if (newsItemCategory.NewsId == newsId && newsItemCategory.CategoryId == categoryId)
                    return newsItemCategory;

            return null;
        }

        public static string GetCategoryNameWithPrefix(this NewsCategory category, INewsCategoryService categoryService)
        {
            string result = string.Empty;

            while (category != null)
            {
                if (String.IsNullOrEmpty(result))
                    result = category.Name;
                else
                    result = "--" + result;
                category = categoryService.GetNewsCategoryById(category.ParentCategoryId);
            }
            return result;
        }

        public static string GetCategoryBreadCrumb(this NewsCategory category, INewsCategoryService categoryService)
        {
            string result = string.Empty;

            while (category != null && !category.Deleted)
            {
                if (String.IsNullOrEmpty(result))
                    result = category.Name;
                else
                    result = category.Name + " >> " + result;

                category = categoryService.GetNewsCategoryById(category.ParentCategoryId);

            }
            return result;
        }

    }
}
