using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.News;

namespace Nop.Services.News
{
    /// <summary>
    /// News service interface
    /// </summary>
    public partial interface INewsService
    {
        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsItem">News item</param>
        void DeleteNews(NewsItem newsItem);

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News</returns>
        NewsItem GetNewsById(int newsId);

        IList<NewsItem> GetNewsByIds(int[] newsIds);
        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        IPagedList<NewsItem> GetAllNews(int languageId, int storeId,
            int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets all news not in top featured news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        IPagedList<NewsItem> GetListNews(int languageId, int storeId,
            int pageIndex, int pageSize, bool showHidden = false);
        /// <summary>
        /// Gets top featured news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        IPagedList<NewsItem> GetTopFeaturedNews(int languageId, int storeId,
            int pageIndex, int pageSize, bool showHidden = false);
        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        void InsertNews(NewsItem news);

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        void UpdateNews(NewsItem news);

        /// <summary>
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <returns>Comments</returns>
        IList<NewsComment> GetAllComments(int customerId);

        /// <summary>
        /// Gets a news comment
        /// </summary>
        /// <param name="newsCommentId">News comment identifier</param>
        /// <returns>News comment</returns>
        NewsComment GetNewsCommentById(int newsCommentId);

        /// <summary>
        /// Deletes a news comment
        /// </summary>
        /// <param name="newsComment">News comment</param>
        void DeleteNewsComment(NewsComment newsComment);

        #region News pictures

        /// <summary>
        /// Deletes a news picture
        /// </summary>
        /// <param name="newsPicture">news picture</param>
        void DeleteNewsPicture(NewsPicture newsPicture);

        /// <summary>
        /// Gets a news pictures by news identifier
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>news pictures</returns>
        IList<NewsPicture> GetNewsPicturesByNewsId(int newsId);

        /// <summary>
        /// Gets a news picture
        /// </summary>
        /// <param name="newsPictureId">news picture identifier</param>
        /// <returns>news picture</returns>
        NewsPicture GetNewsPictureById(int newsPictureId);

        /// <summary>
        /// Inserts a news picture
        /// </summary>
        /// <param name="newsPicture">news picture</param>
        void InsertNewsPicture(NewsPicture newsPicture);

        /// <summary>
        /// Updates a news picture
        /// </summary>
        /// <param name="newsPicture">news picture</param>
        void UpdateNewsPicture(NewsPicture newsPicture);

        #region Related news

        /// <summary>
        /// Deletes a related news
        /// </summary>
        /// <param name="relatedNews">Related news</param>
        void DeleteRelatedNews(RelatedNews relatedNews);

        /// <summary>
        /// Gets a related News collection by News identifier
        /// </summary>
        /// <param name="NewsId1">The first News identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related News collection</returns>
        IList<RelatedNews> GetRelatedNewsByNewsId1(int newsId1, bool showHidden = false);

        /// <summary>
        /// Gets a related News
        /// </summary>
        /// <param name="relatedNewsId">Related News identifier</param>
        /// <returns>Related News</returns>
        RelatedNews GetRelatedNewsById(int relatedNewsId);

        /// <summary>
        /// Inserts a related News
        /// </summary>
        /// <param name="relatedNews">Related News</param>
        void InsertRelatedNews(RelatedNews relatedNews);

        /// <summary>
        /// Updates a related News
        /// </summary>
        /// <param name="relatedNews">Related News</param>
        void UpdateRelatedNews(RelatedNews relatedNews);
        #endregion

        #region News ViewTracking

        /// <summary>
        /// Gets list top view count of newsItem in 24h
        /// </summary>
        /// <returns>News ViewTracking collection</returns>
        IList<NewsViewTracking> GetTopNewsViewTracking();

        /// <summary>
        /// Gets list top view count of newsItem in 24h by category
        /// </summary>
        /// <returns>News ViewTracking collection</returns>
        IList<NewsViewTracking> GetTopNewsViewTrackingByCategory(int categoryId);
        /// <summary>
        /// Gets view count of newsItem in 24h
        /// </summary>
        /// <param name="NewsId">News identifier</param>
        /// <returns>News ViewTracking</returns>
        NewsViewTracking GetNewsViewTrackingById(int newsId);

        /// <summary>
        /// Inserts a News ViewTracking
        /// </summary>
        /// <param name="newsViewTracking">News ViewTracking</param>
        void InsertNewsViewTracking(NewsViewTracking newsViewTracking);

        /// <summary>
        /// Update a News ViewTracking
        /// </summary>
        /// <param name="newsViewTracking">News ViewTracking</param>
        void UpdateNewsViewTracking(NewsViewTracking newsViewTracking);

        /// <summary>
        /// Count news view in day
        /// </summary>
        /// <param name="iNewsId"></param>
        void TrackingNewsView(int iNewsId);
        #endregion
        #endregion
    }
}
