using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// NewsCategory service
    /// </summary>
    public partial class PromotionService : IPromotionService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// </remarks>
        private const string GETPROMOTION_KEY = "Nop.promotion-{0}-{1}-{2}";

        private const string PROMOTION_PATTERN_KEY = "Nop.promotion.";

        private const string PROMOTIONDETAIL_PATTERN_KEY = "Nop.promotiondetail.";

        #endregion

        #region Fields

        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<PromotionDetail> _promotiondetailRepository;
        private readonly IRepository<Product> _productRepository;
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
        public PromotionService(ICacheManager cacheManager,
            IRepository<Promotion> promotionRepository,
            IRepository<PromotionDetail> promotiondetailRepository,
            IRepository<Product> productRepository,
            IRepository<AclRecord> aclRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._promotionRepository = promotionRepository;
            this._promotiondetailRepository = promotiondetailRepository;
            this._productRepository = productRepository;
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
        public virtual void DeletePromotion(Promotion promo)
        {
            if (promo == null)
                throw new ArgumentNullException("promotion");
            promo.Published = false;
            promo.Deleted = true;
            UpdatePromotion(promo);
            //IList<PromotionDetail> lst = GetPromotionDetailByPromotion(promo.Id);
            //if (lst != null && lst.Count() > 0)
            //{
            //    foreach (var item in lst)
            //    {
            //        DeletePromotionDetail(item);
            //    }
            //}

        }

        /// <summary>
        /// Gets all news categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<Promotion> GetAllPromo(string promoName = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _promotionRepository.Table;
            if (!String.IsNullOrWhiteSpace(promoName))
                query = query.Where(c => c.PromotionName.Contains(promoName));
            query = query.Where(c => c.Deleted == false);
            query = query.OrderBy(c => c.Id);


            var unsortedpromotions = query.ToList();

            //sort categories
            //var sortedCategories = unsortedCategories.SortCategoriesForTree();

            //paging
            return new PagedList<Promotion>(unsortedpromotions, pageIndex, pageSize);
        }
        public virtual IList<Promotion> GetPromotionByIds(int[] promoIds)
        {
            if (promoIds == null || promoIds.Length == 0)
                return new List<Promotion>();

            var query = from p in _promotionRepository.Table
                        where promoIds.Contains(p.Id)
                        select p;
            var promotions = query.ToList();
            //sort by passed identifiers
            var sortedPromotions = new List<Promotion>();
            foreach (int id in promoIds)
            {
                var promotion = promotions.Find(x => x.Id == id);
                if (promotion != null)
                    sortedPromotions.Add(promotion);
            }
            return sortedPromotions;
        }

        /// <summary>
        /// Gets a news category
        /// </summary>
        /// <param name="categoryId">NewsCategory identifier</param>
        /// <returns>NewsCategory</returns>
        public virtual Promotion GetPromotionById(int promoId)
        {
            if (promoId == 0)
                return null;

            return _promotionRepository.GetById(promoId);
        }

        /// <summary>
        /// Inserts newsCategory
        /// </summary>
        /// <param name="category">NewsCategory</param>
        public virtual void InsertPromotion(Promotion promo)
        {
            if (promo == null)
                throw new ArgumentNullException("Promotion");

            _promotionRepository.Insert(promo);

            //cache
            _cacheManager.RemoveByPattern(PROMOTION_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(promo);
        }

        /// <summary>
        /// Updates the newsCategory
        /// </summary>
        /// <param name="category">NewsCategory</param>
        public virtual void UpdatePromotion(Promotion promo)
        {
            if (promo == null)
                throw new ArgumentNullException("Promotion");

            _promotionRepository.Update(promo);

            //cache
            _cacheManager.RemoveByPattern(PROMOTION_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(promo);
        }


        #region promotion detail
        public virtual IList<PromotionDetail> GetPromotionDetailByPromotion(int promoId,string productName, int categoryId, int manufactureId)
        {
            //string key = string.Format("Nop.promotion-GetPromotionDetailByPromotion_{0}", promoId);
            //return _cacheManager.Get(key, () =>
            //{
            var query = from pd in _promotiondetailRepository.Table
                        where (pd.PromotionId == promoId)
                        select pd;
            if (query == null)
                return null;
            if (!String.IsNullOrWhiteSpace(productName))
                query = query.Where(p => p.ProductName.Contains(productName));

            if (categoryId > 0)
            {
                query = from p in query
                        where p.CategoryId == categoryId
                        select p;
            }
            if (manufactureId > 0)
            {
                query = from p in query
                        where p.ManufactureId == manufactureId
                        select p;
            }
            return query.ToList();
            //});
        }

        public virtual void InsertPromotionDetail(PromotionDetail promodetail)
        {
            if (promodetail == null)
                throw new ArgumentNullException("PromotionDetail");

            _promotiondetailRepository.Insert(promodetail);

            //cache
            _cacheManager.RemoveByPattern(PROMOTIONDETAIL_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(promodetail);
        }
        public virtual void UpdatePromotionDetail(PromotionDetail promodetail)
        {
            if (promodetail == null)
                throw new ArgumentNullException("PromotionDetail");

            _promotiondetailRepository.Update(promodetail);

            //cache
            _cacheManager.RemoveByPattern(PROMOTIONDETAIL_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(promodetail);
        }
        public virtual PromotionDetail GetPromotionDetailById(int promodetailId)
        {
            if (promodetailId == 0)
                return null;

            return _promotiondetailRepository.GetById(promodetailId);
        }
        public virtual void DeletePromotionDetail(PromotionDetail promodetail)
        {
            if (promodetail == null)
                throw new ArgumentNullException("promotion");
            _promotiondetailRepository.Delete(promodetail);
        }
        public virtual bool CheckPromotionDetailExist(int PromotionId, int ProductId)
        {
            var query = from pd in _promotiondetailRepository.Table
                        where (pd.PromotionId == PromotionId && pd.ProductId == ProductId)
                        select pd;


            return query.ToList().Count() > 0;
        }
        public virtual IList<PromotionDetail> GetPromotionDetailByProductId(int PromotionId, int ProductId)
        {
            var query = from pd in _promotiondetailRepository.Table
                        where (pd.PromotionId == PromotionId && pd.ProductId == ProductId)
                        select pd;


            return query.ToList();
        }
        public virtual IList<PromotionDetail> GetPromotionDetailByIds(int[] promodetailIds)
        {
            if (promodetailIds == null || promodetailIds.Length == 0)
                return new List<PromotionDetail>();

            var query = from p in _promotiondetailRepository.Table
                        where promodetailIds.Contains(p.Id)
                        select p;
            var promotiondetails = query.ToList();
            //sort by passed identifiers
            var sortedPromotiondetails = new List<PromotionDetail>();
            foreach (int id in promodetailIds)
            {
                var promotiondetail = promotiondetails.Find(x => x.Id == id);
                if (promotiondetail != null)
                    sortedPromotiondetails.Add(promotiondetail);
            }
            return sortedPromotiondetails;
        }
        #endregion
        #endregion
    }
}
