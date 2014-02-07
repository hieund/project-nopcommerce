using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.News
{
    /// <summary>
    /// NewsCategory service
    /// </summary>
    public partial class NewsCategoryService : INewsCategoryService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// </remarks>
        private const string NEWSCATEGORIES_BY_PARENT_CATEGORY_ID_KEY = "Nop.newscategory.byparent-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : category ID
        /// {2} : page index
        /// {3} : page size
        /// </remarks>
        private const string NEWSITEMCATEGORIES_ALLBYCATEGORYID_KEY = "Nop.newsitemcategory.allbycategoryid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : newsItem ID
        /// </remarks>
        private const string NEWSITEMCATEGORIES_ALLBYNEWSID_KEY = "Nop.newsitemcategory.allbynewsid-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string NEWSCATEGORIES_PATTERN_KEY = "Nop.newscategory.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string NEWSITEMCATEGORIES_PATTERN_KEY = "Nop.newsitemcategory.";

        #endregion

        #region Fields

        private readonly IRepository<NewsCategory> _newsCategoryRepository;
        private readonly IRepository<NewsItemCategory> _newsItemCategoryRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryRepository">Category repository</param>
        /// <param name="newsItemCategoryRepository">newsItemCategory repository</param>
        /// <param name="newsItemRepository">newsItem repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        public NewsCategoryService(ICacheManager cacheManager,
            IRepository<NewsCategory> newsCategoryRepository,
            IRepository<NewsItemCategory> newsItemCategoryRepository,
            IRepository<NewsItem> newsItemRepository,
            IRepository<AclRecord> aclRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._newsCategoryRepository = newsCategoryRepository;
            this._newsItemCategoryRepository = newsItemCategoryRepository;
            this._newsItemRepository = newsItemRepository;
            this._aclRepository = aclRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete newscategory
        /// </summary>
        /// <param name="newsCategory">NewsCategory</param>
        public virtual void DeleteNewsCategory(NewsCategory newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException("newsCategory");

            newsCategory.Deleted = true;
            UpdateNewsCategory(newsCategory);

            //set a ParentCategory property of the children to 0
            var subcategories = GetAllNewsCategoriesByParentCategoryId(newsCategory.Id);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                UpdateNewsCategory(subcategory);
            }
        }

        /// <summary>
        /// Gets all news categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<NewsCategory> GetAllNewsCategories(string categoryName = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _newsCategoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!String.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder);

            if (!showHidden)
            {
                //only distinct categories (group by ID)
                query = from c in query
                        group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder);
            }

            var unsortedCategories = query.ToList();

            //sort categories
            //var sortedCategories = unsortedCategories.SortCategoriesForTree();

            //paging
            return new PagedList<NewsCategory>(unsortedCategories, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all news categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>NewsCategory collection</returns>
        public IList<NewsCategory> GetAllNewsCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false)
        {
            string key = string.Format(NEWSCATEGORIES_BY_PARENT_CATEGORY_ID_KEY, parentCategoryId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _newsCategoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentCategoryId == parentCategoryId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder);

                if (!showHidden)
                {
                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                                into cGroup
                                orderby cGroup.Key
                                select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.DisplayOrder);
                }

                var categories = query.ToList();
                return categories;
            });

        }

        /// <summary>
        /// Gets a news category
        /// </summary>
        /// <param name="categoryId">NewsCategory identifier</param>
        /// <returns>NewsCategory</returns>
        public virtual NewsCategory GetNewsCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return _newsCategoryRepository.GetById(categoryId);
        }

        /// <summary>
        /// Inserts newsCategory
        /// </summary>
        /// <param name="category">NewsCategory</param>
        public virtual void InsertNewsCategory(NewsCategory newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException("newsCategory");

            _newsCategoryRepository.Insert(newsCategory);

            //cache
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(NEWSITEMCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(newsCategory);
        }

        /// <summary>
        /// Updates the newsCategory
        /// </summary>
        /// <param name="category">NewsCategory</param>
        public virtual void UpdateNewsCategory(NewsCategory newsCategory)
        {
            if (newsCategory == null)
                throw new ArgumentNullException("newsCategory");

            //validate category hierarchy
            var parentCategory = GetNewsCategoryById(newsCategory.ParentCategoryId);
            while (parentCategory != null)
            {
                if (newsCategory.Id == parentCategory.Id)
                {
                    newsCategory.ParentCategoryId = 0;
                    break;
                }
                parentCategory = GetNewsCategoryById(parentCategory.ParentCategoryId);
            }

            _newsCategoryRepository.Update(newsCategory);

            //cache
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(NEWSITEMCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(newsCategory);
        }

        /// <summary>
        /// Deletes a newsItem category mapping
        /// </summary>
        /// <param name="newsItemCategory">NewsItem category</param>
        public virtual void DeleteNewsItemCategory(NewsItemCategory newsItemCategory)
        {
            if (newsItemCategory == null)
                throw new ArgumentNullException("newsItemCategory");

            _newsItemCategoryRepository.Delete(newsItemCategory);

            //cache
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(NEWSITEMCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(newsItemCategory);
        }

        /// <summary>
        /// Gets newsItem category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>newsItem a category mapping collection</returns>
        public virtual IPagedList<NewsItemCategory> GetNewsItemCategoriesByCategoryId(int categoryId, int pageIndex, int pageSize, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<NewsItemCategory>(new List<NewsItemCategory>(), pageIndex, pageSize);

            string key = string.Format(NEWSITEMCATEGORIES_ALLBYCATEGORYID_KEY, showHidden, categoryId, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _newsItemCategoryRepository.Table
                            join p in _newsItemRepository.Table on pc.NewsId equals p.Id
                            where pc.CategoryId == categoryId &&
                                  (showHidden || p.Published)
                            orderby pc.DisplayOrder
                            select pc;

                if (!showHidden)
                {
                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                                into cGroup
                                orderby cGroup.Key
                                select cGroup.FirstOrDefault();
                    query = query.OrderBy(pc => pc.DisplayOrder);
                }

                var newsItemCategories = new PagedList<NewsItemCategory>(query, pageIndex, pageSize);
                return newsItemCategories;
            });
        }

        /// <summary>
        /// Gets a newsItem category mapping collection
        /// </summary>
        /// <param name="newsItemId">newsItem identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>newsItem category mapping collection</returns>
        public virtual IList<NewsItemCategory> GetNewsItemCategoriesByNewsId(int newsId, bool showHidden = false)
        {
            if (newsId == 0)
                return new List<NewsItemCategory>();

            string key = string.Format(NEWSITEMCATEGORIES_ALLBYNEWSID_KEY, showHidden, newsId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _newsItemCategoryRepository.Table
                            join c in _newsCategoryRepository.Table on pc.CategoryId equals c.Id
                            where pc.NewsId == newsId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.DisplayOrder
                            select pc;

                if (!showHidden)
                {
                    //only distinct categories (group by ID)
                    query = from pc in query
                            group pc by pc.Id
                                into pcGroup
                                orderby pcGroup.Key
                                select pcGroup.FirstOrDefault();
                    query = query.OrderBy(pc => pc.DisplayOrder);
                }

                var newsItemCategories = query.ToList();
                return newsItemCategories;
            });
        }

        /// <summary>
        /// Gets a newsItem category mapping 
        /// </summary>
        /// <param name="newsItemCategoryId">newsItem category mapping identifier</param>
        /// <returns>newsItem category mapping</returns>
        public virtual NewsItemCategory GetNewsItemCategoryById(int newsItemCategoryId)
        {
            if (newsItemCategoryId == 0)
                return null;

            return _newsItemCategoryRepository.GetById(newsItemCategoryId);
        }

        /// <summary>
        /// Inserts a newsItem category mapping
        /// </summary>
        /// <param name="newsItemCategory">>newsItem category mapping</param>
        public virtual void InsertNewsItemCategory(NewsItemCategory newsItemCategory)
        {
            if (newsItemCategory == null)
                throw new ArgumentNullException("newsItemCategory");

            _newsItemCategoryRepository.Insert(newsItemCategory);

            //cache
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(NEWSITEMCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(newsItemCategory);
        }

        /// <summary>
        /// Updates the newsItem category mapping 
        /// </summary>
        /// <param name="newsItemCategory">>newsItem category mapping</param>
        public virtual void UpdateNewsItemCategory(NewsItemCategory newsItemCategory)
        {
            if (newsItemCategory == null)
                throw new ArgumentNullException("newsItemCategory");

            _newsItemCategoryRepository.Update(newsItemCategory);

            //cache
            _cacheManager.RemoveByPattern(NEWSCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(NEWSITEMCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(newsItemCategory);
        }

        #endregion
    }
}
