using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.News
{
    /// <summary>
    /// News service
    /// </summary>
    public partial class NewsService : INewsService
    {
        #region Fields

        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<NewsItemCategory> _newsItemCategoryRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<RelatedNews> _relatedNewsRepository;
        private readonly IRepository<NewsPicture> _newsPictureRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<NewsViewTracking> _newsViewTrackingRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public NewsService(IRepository<NewsItem> newsItemRepository,
            IRepository<NewsItemCategory> newsItemCategoryRepository, 
            IRepository<NewsComment> newsCommentRepository,
            IRepository<RelatedNews> relatedNewsRepository,
            IRepository<NewsPicture> newsPictureRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<NewsViewTracking> newsViewTrackingRepository,
            IEventPublisher eventPublisher)
        {
            this._newsItemCategoryRepository = newsItemCategoryRepository;
            this._newsItemRepository = newsItemRepository;
            this._newsCommentRepository = newsCommentRepository;
            this._relatedNewsRepository = relatedNewsRepository;
            this._newsPictureRepository = newsPictureRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._newsViewTrackingRepository = newsViewTrackingRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region News
        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsItem">News item</param>
        public virtual void DeleteNews(NewsItem newsItem)
        {
            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

            _newsItemRepository.Delete(newsItem);
            
            //event notification
            _eventPublisher.EntityDeleted(newsItem);
        }

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News</returns>
        public virtual NewsItem GetNewsById(int newsId)
        {
            if (newsId == 0)
                return null;

            return _newsItemRepository.GetById(newsId);
        }

        /// <summary>
        /// Get news by identifiers
        /// </summary>
        /// <param name="productIds">News identifiers</param>
        /// <returns>NewsItems</returns>
        public virtual IList<NewsItem> GetNewsByIds(int[] newsIds)
        {
            if (newsIds == null || newsIds.Length == 0)
                return new List<NewsItem>();

            var query = from p in _newsItemRepository.Table
                        where newsIds.Contains(p.Id)
                        select p;
            var newsItems = query.ToList();
            //sort by passed identifiers
            var sortedNewsItems = new List<NewsItem>();
            foreach (int id in newsIds)
            {
                var newsItem = newsItems.Find(x => x.Id == id);
                if (newsItem != null)
                    sortedNewsItems.Add(newsItem);
            }
            return sortedNewsItems;
        }

        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        public virtual IPagedList<NewsItem> GetAllNews(int languageId, int storeId,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _newsItemRepository.Table;
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }
            query = query.OrderByDescending(n => n.CreatedOnUtc);

            //Store mapping
            if (storeId > 0)
            {
                query = from n in query
                        join sm in _storeMappingRepository.Table on n.Id equals sm.EntityId into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToStores || (sm.EntityName == "NewsItem" && storeId == sm.StoreId)
                        select n;

                //only distinct items (group by ID)
                query = from n in query
                        group n by n.Id
                        into nGroup
                        orderby nGroup.Key
                        select nGroup.FirstOrDefault();
                query = query.OrderByDescending(n => n.CreatedOnUtc);
            }

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }

        /// <summary>
        /// Gets all news not in top featured news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        public virtual IPagedList<NewsItem> GetListNews(int languageId, int storeId,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _newsItemRepository.Table;
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }
            query = query.OrderByDescending(n => n.CreatedOnUtc);

            //Store mapping
            if (storeId > 0)
            {
                query = from n in query
                        join sm in _storeMappingRepository.Table on n.Id equals sm.EntityId into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToStores || (sm.EntityName == "NewsItem" && storeId == sm.StoreId)
                        select n;

                //only distinct items (group by ID)
                query = from n in query
                        group n by n.Id
                            into nGroup
                            orderby nGroup.Key
                            select nGroup.FirstOrDefault();
                query = query.OrderByDescending(n => n.CreatedOnUtc);
            }

            var newsTopItems = GetTopFeaturedNews(languageId, storeId, 0, 5);
            if (newsTopItems.Count > 0)
            {
                var listId = newsTopItems.Select(x => x.Id).ToList();
                query = query.Where(x => !listId.Contains(x.Id));
            }

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }

        /// <summary>
        /// Gets top featured news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        public virtual IPagedList<NewsItem> GetTopFeaturedNews(int languageId, int storeId,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _newsItemRepository.Table;
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published && n.Featured);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }
            query = query.OrderByDescending(n => n.CreatedOnUtc);

            //Store mapping
            if (storeId > 0)
            {
                query = from n in query
                        join sm in _storeMappingRepository.Table on n.Id equals sm.EntityId into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToStores || (sm.EntityName == "NewsItem" && storeId == sm.StoreId)
                        select n;

                //only distinct items (group by ID)
                query = from n in query
                        group n by n.Id
                            into nGroup
                            orderby nGroup.Key
                            select nGroup.FirstOrDefault();
                query = query.OrderByDescending(n => n.CreatedOnUtc);
            }

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }

        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        public virtual void InsertNews(NewsItem news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            _newsItemRepository.Insert(news);

            //event notification
            _eventPublisher.EntityInserted(news);
        }

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        public virtual void UpdateNews(NewsItem news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            _newsItemRepository.Update(news);
            
            //event notification
            _eventPublisher.EntityUpdated(news);
        }
        
        /// <summary>
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <returns>Comments</returns>
        public virtual IList<NewsComment> GetAllComments(int customerId)
        {
            var query = from c in _newsCommentRepository.Table
                        orderby c.CreatedOnUtc
                        where (customerId == 0 || c.CustomerId == customerId)
                        select c;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets a news comment
        /// </summary>
        /// <param name="newsCommentId">News comment identifier</param>
        /// <returns>News comment</returns>
        public virtual NewsComment GetNewsCommentById(int newsCommentId)
        {
            if (newsCommentId == 0)
                return null;

            return _newsCommentRepository.GetById(newsCommentId);
        }

        /// <summary>
        /// Deletes a news comment
        /// </summary>
        /// <param name="newsComment">News comment</param>
        public virtual void DeleteNewsComment(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            _newsCommentRepository.Delete(newsComment);
        }

        #endregion

        #region News pictures

        /// <summary>
        /// Deletes a news picture
        /// </summary>
        /// <param name="newsPicture">news picture</param>
        public virtual void DeleteNewsPicture(NewsPicture newsPicture)
        {
            if (newsPicture == null)
                throw new ArgumentNullException("newsPicture");

            _newsPictureRepository.Delete(newsPicture);

            //event notification
            _eventPublisher.EntityDeleted(newsPicture);
        }

        /// <summary>
        /// Gets a news pictures by news identifier
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>news pictures</returns>
        public virtual IList<NewsPicture> GetNewsPicturesByNewsId(int newsId)
        {
            var query = from pp in _newsPictureRepository.Table
                        where pp.NewsId == newsId
                        orderby pp.DisplayOrder
                        select pp;
            var newsPictures = query.ToList();
            return newsPictures;

        }

        /// <summary>
        /// Gets a news picture
        /// </summary>
        /// <param name="newsPictureId">news picture identifier</param>
        /// <returns>news picture</returns>
        public virtual NewsPicture GetNewsPictureById(int newsPictureId)
        {
            if (newsPictureId == 0)
                return null;

            return _newsPictureRepository.GetById(newsPictureId);
        }

        /// <summary>
        /// Inserts a news picture
        /// </summary>
        /// <param name="newsPicture">news picture</param>
        public virtual void InsertNewsPicture(NewsPicture newsPicture)
        {
            if (newsPicture == null)
                throw new ArgumentNullException("newsPicture");

            _newsPictureRepository.Insert(newsPicture);

            //event notification
            _eventPublisher.EntityInserted(newsPicture);
        }

        /// <summary>
        /// Updates a news picture
        /// </summary>
        /// <param name="newsPicture">news picture</param>
        public virtual void UpdateNewsPicture(NewsPicture newsPicture)
        {
            if (newsPicture == null)
                throw new ArgumentNullException("newsPicture");

            _newsPictureRepository.Update(newsPicture);

            //event notification
            _eventPublisher.EntityUpdated(newsPicture);
        }

        #endregion

        #region Related news

        /// <summary>
        /// Deletes a related news
        /// </summary>
        /// <param name="relatedNews">Related news</param>
        public virtual void DeleteRelatedNews(RelatedNews relatedNews)
        {
            if (relatedNews == null)
                throw new ArgumentNullException("relatedNews");

            _relatedNewsRepository.Delete(relatedNews);

            //event notification
            _eventPublisher.EntityDeleted(relatedNews);
        }

        /// <summary>
        /// Gets a related News collection by News identifier
        /// </summary>
        /// <param name="NewsId1">The first News identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related News collection</returns>
        public virtual IList<RelatedNews> GetRelatedNewsByNewsId1(int newsId1, bool showHidden = false)
        {
            var query = from rp in _relatedNewsRepository.Table
                        join p in _newsItemRepository.Table on rp.NewsId2 equals p.Id
                        where rp.NewsId1 == newsId1 &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder
                        select rp;
            var relatedNews = query.ToList();

            return relatedNews;
        }

        /// <summary>
        /// Gets a related News
        /// </summary>
        /// <param name="relatedNewsId">Related News identifier</param>
        /// <returns>Related News</returns>
        public virtual RelatedNews GetRelatedNewsById(int relatedNewsId)
        {
            if (relatedNewsId == 0)
                return null;

            return _relatedNewsRepository.GetById(relatedNewsId);
        }

        /// <summary>
        /// Inserts a related News
        /// </summary>
        /// <param name="relatedNews">Related News</param>
        public virtual void InsertRelatedNews(RelatedNews relatedNews)
        {
            if (relatedNews == null)
                throw new ArgumentNullException("relatedNews");

            _relatedNewsRepository.Insert(relatedNews);

            //event notification
            _eventPublisher.EntityInserted(relatedNews);
        }

        /// <summary>
        /// Updates a related News
        /// </summary>
        /// <param name="relatedNews">Related News</param>
        public virtual void UpdateRelatedNews(RelatedNews relatedNews)
        {
            if (relatedNews == null)
                throw new ArgumentNullException("relatedNews");

            _relatedNewsRepository.Update(relatedNews);

            //event notification
            _eventPublisher.EntityUpdated(relatedNews);
        }

        #endregion

        #region News ViewTracking

        /// <summary>
        /// Gets list top view count of newsItem in 24h
        /// </summary>
        /// <returns>News ViewTracking collection</returns>
        public virtual IList<NewsViewTracking> GetTopNewsViewTracking()
        {
            var query = from p in _newsViewTrackingRepository.Table.AsEnumerable()
                        where p.TrackingDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-1)
                        && p.TrackingDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                        orderby p.ViewCount descending
                        select p;
            var newsViewTrackings = query.ToList();

            return newsViewTrackings;
        }

        /// <summary>
        /// Gets list top view count of newsItem in 24h by category
        /// </summary>
        /// <returns>News ViewTracking collection</returns>
        public virtual IList<NewsViewTracking> GetTopNewsViewTrackingByCategory(int categoryId)
        {
            var query = from p in _newsViewTrackingRepository.Table.AsEnumerable()
                        join pc in _newsItemCategoryRepository.Table.AsEnumerable() on p.NewsItemId equals pc.NewsId
                        where p.TrackingDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-1)
                        && p.TrackingDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                        && pc.CategoryId == categoryId
                        orderby p.ViewCount descending
                        select p;
            var newsViewTrackings = query.ToList();

            return newsViewTrackings;
        }

        /// <summary>
        /// Gets view count of newsItem in 24h
        /// </summary>
        /// <param name="NewsId">News identifier</param>
        /// <returns>News ViewTracking</returns>
        public virtual NewsViewTracking GetNewsViewTrackingById(int newsId)
        {
            if (newsId == 0)
                return null;

            var query = from p in _newsViewTrackingRepository.Table.AsEnumerable()
                        where p.NewsItemId == newsId && (p.TrackingDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                        && p.TrackingDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1))
                        select p;
            NewsViewTracking newsViewTracking = query.ToList().FirstOrDefault();

            return newsViewTracking;
        }

        /// <summary>
        /// Inserts a News ViewTracking
        /// </summary>
        /// <param name="newsViewTracking">News ViewTracking</param>
        public virtual void InsertNewsViewTracking(NewsViewTracking newsViewTracking)
        {
            if (newsViewTracking == null)
                throw new ArgumentNullException("newsViewTracking");

            _newsViewTrackingRepository.Insert(newsViewTracking);

            //event notification
            _eventPublisher.EntityInserted(newsViewTracking);
        }

        /// <summary>
        /// Update a News ViewTracking
        /// </summary>
        /// <param name="newsViewTracking">News ViewTracking</param>
        public virtual void UpdateNewsViewTracking(NewsViewTracking newsViewTracking)
        {
            if (newsViewTracking == null)
                throw new ArgumentNullException("newsViewTracking");

            _newsViewTrackingRepository.Update(newsViewTracking);

            //event notification
            _eventPublisher.EntityUpdated(newsViewTracking);
        }

        /// <summary>
        /// Count news view in day
        /// </summary>
        /// <param name="iNewsId"></param>
        public virtual void TrackingNewsView(int iNewsId)
        {
            if (iNewsId != 0)
            {
                var newsViewTracking = GetNewsViewTrackingById(iNewsId);
                if (newsViewTracking != null && newsViewTracking.Id > 0)
                {
                    newsViewTracking.ViewCount++;
                    UpdateNewsViewTracking(newsViewTracking);
                }
                else
                {
                    newsViewTracking = new NewsViewTracking();
                    newsViewTracking.NewsItemId = iNewsId;
                    newsViewTracking.ViewCount = 1;
                    newsViewTracking.TrackingDate = DateTime.Now;
                    InsertNewsViewTracking(newsViewTracking);
                }
            }
        }

        #endregion
        #endregion
    }
}
