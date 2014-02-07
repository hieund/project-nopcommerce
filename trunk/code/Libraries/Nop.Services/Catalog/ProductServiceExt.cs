using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    public partial class ProductService : IProductService
    {
        #region Constants

        private const string PRODUCTDETAIL_PATTERN_KEY = "Nop.productdetailbyproductid.id-{0}";

        #endregion

        #region Fields

        private readonly IRepository<ProductDetail> _productDetailRepository;
        private readonly IRepository<ProductPropertyGroup> _productPropertyGroupRepository;
        private readonly IRepository<ProductProperty> _productPropertyRepository;
        private readonly IRepository<ProductPropertyValue> _productPropertyValueRepository;

        #endregion

        #region Product Detail

        /// <summary>
        /// Get product details specification by product's id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IList<ProductDetail> GetProductDetailByProductId(int productId)
        {
            if (productId <= 0)
                return null;
            string key = string.Format(PRODUCTDETAIL_PATTERN_KEY, productId);
            return _cacheManager.Get(key, () =>
            {
                var prodDetails = from pd in _productDetailRepository.Table
                                  join pp in _productPropertyRepository.Table on pd.PropertyId equals pp.Id
                                  join pg in _productPropertyGroupRepository.Table on pd.GroupId equals pg.Id
                                  where pd.ProductId == productId &&
                                        pd.PropertyType == pp.Type &&
                                        !pp.IsDeleted &&
                                        pp.IsActived &&
                                        !pg.IsDeleted &&
                                        pg.IsActived
                                  select pd;

                return prodDetails.ToList();
            });
        }

        /// <summary>
        /// Insert a product detail
        /// </summary>
        /// <param name="productDetail"></param>
        public void InsertProductDetail(ProductDetail pd)
        {
            if (pd == null)
                throw new ArgumentNullException("productDetail");
            _productDetailRepository.Insert(pd);

            //remove cache
            _cacheManager.RemoveByPattern(PRODUCTDETAIL_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(pd);
        }

        public void UpdateProductDetail(ProductDetail pd)
        {
            if (pd == null)
                throw new ArgumentNullException("productDetail");

            _productDetailRepository.Update(pd);

            //remove cache
            _cacheManager.RemoveByPattern(PRODUCTDETAIL_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(pd);
        }

        #endregion

        #region Product Search Engine

        /// <summary>
        /// Update search engine for fulltext search
        /// </summary>
        /// <param name="p"></param>
        public void UpdateProductIndex(Product p)
        {
            //IndexWriter idxWriter = new IndexWriter();
        }

        #endregion
    }
}
