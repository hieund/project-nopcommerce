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
    public partial class CategoryService : ICategoryService
    {
        #region Constants
        private const string PRODUCTPROPERTYGROUPS_PATTERN_KEY = "Nop.productpropertygroup.id-{0}";
        private const string PRODUCTPROPERTYGROUPSALL_BY_CATEGORYID_KEY = "Nop.productpropertygroup.allbycategoryid-{0}";
        private const string PRODUCTPROPERTY_PATTERN_KEY = "Nop.productproperty.id-{0}";
        private const string PRODUCTPROPERTY_BY_CATEGORYID_KEY = "Nop.productproperty.allbycategoryid-{0}";
        private const string PRODUCTPROPERTY_BY_GROUPID_KEY = "Nop.productproperty.allbygroupid-{0}";
        private const string PRODUCTPROPERTYVALUE_BY_PROPERTYID_KEY = "Nop.productpropertyvalue.allbypropertyid-{0}";
        private const string PRODUCTPROPERTYVALUE_BY_VALUEID_KEY = "Nop.productpropertyvalue.allbyvalueid-{0}";
        private const string PRODUCTPROPERTYVALUEMAPPING_BY_PROPERTYID_KEY = "Nop.productpropertyvaluemapping.allbyproductpropertyid-{0}";

        private const string CATEGORIES_BYMANUFACTURERID = "Nop.allcategory.bymanufactureid-{0}";
        #endregion

        #region Fields
        private readonly IRepository<ProductPropertyGroup> _productPropertyGroupRepository;
        private readonly IRepository<ProductProperty> _productPropertyRepository;
        private readonly IRepository<ProductPropertyValue> _productPropertyValueRepository;
        private readonly IRepository<ProductPropertyValueMapping> _productPropertyValueMappingRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        #endregion

        #region Product Property Groups
        /// <summary>
        /// Get property groups of category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IList<ProductPropertyGroup> GetProductPropertyGroupByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTYGROUPSALL_BY_CATEGORYID_KEY, categoryId);
            return _cacheManager.Get(key, () =>
            {
                var query = from ppg in _productPropertyGroupRepository.Table
                            where ppg.CategoryId == categoryId &&
                                  !ppg.IsDeleted
                            orderby ppg.DisplayOrder
                            select ppg;
                var propertyGroups = query.ToList();
                return propertyGroups;
            });
        }

        /// <summary>
        /// Gets a Product Property Group mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product Property Group mapping collection</returns>
        public virtual ProductPropertyGroup GetProductPropertyGroupById(int productPropertyGroupId)
        {
            if (productPropertyGroupId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTYGROUPS_PATTERN_KEY, productPropertyGroupId);
            return _cacheManager.Get(key, () =>
            {
                return _productPropertyGroupRepository.GetById(productPropertyGroupId);
            });
        }

        /// <summary>
        /// Inserts a product property group mapping
        /// </summary>
        /// <param name="ProductPropertyGroup">>Product Property Group mapping</param>
        public virtual void InsertProductPropertyGroup(ProductPropertyGroup productPropertyGroup)
        {
            if (productPropertyGroup == null)
                throw new ArgumentNullException("productPropertyGroup");

            _productPropertyGroupRepository.Insert(productPropertyGroup);
            //cache
            _cacheManager.RemoveByPattern(string.Format(PRODUCTPROPERTYGROUPS_PATTERN_KEY, productPropertyGroup.CategoryId));
            _cacheManager.RemoveByPattern(PRODUCTPROPERTYGROUPS_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityInserted(productPropertyGroup);
        }

        /// <summary>
        /// Updates the Product Property Group mapping 
        /// </summary>
        /// <param name="ProductPropertyGroup">>Product Property Group mapping</param>
        public virtual void UpdateProductPropertyGroup(ProductPropertyGroup productPropertyGroup)
        {
            if (productPropertyGroup == null)
                throw new ArgumentNullException("productPropertyGroup");

            _productPropertyGroupRepository.Update(productPropertyGroup);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productPropertyGroup);
        }

        /// <summary>
        /// Deletes a Product Property Group mapping
        /// </summary>
        /// <param name="productPropertyGroup">Product Property Group</param>
        public virtual void DeleteProductPropertyGroup(ProductPropertyGroup productPropertyGroup)
        {
            if (productPropertyGroup == null)
                throw new ArgumentNullException("productPropertyGroup");

            productPropertyGroup.IsDeleted = true;
            _productPropertyGroupRepository.Delete(productPropertyGroup);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productPropertyGroup);
        }

        #endregion

        #region Product Property

        /// <summary>
        /// Get Product Property detail by Id
        /// </summary>
        /// <param name="productPropertyId"></param>
        /// <returns></returns>
        public ProductProperty GetProductPropertyById(int productPropertyId)
        {
            if (productPropertyId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTY_PATTERN_KEY, productPropertyId);
            return _cacheManager.Get(key, () =>
            {
                ProductProperty productproperty = _productPropertyRepository.GetById(productPropertyId);
                ICollection<ProductPropertyValue> lst = GetProductPropertyValuesByProductPropertyId(productPropertyId);
                productproperty.ListProductPropertyValue = lst;
                return productproperty;
            });


        }

        /// <summary>
        /// Get list of Product Properties by a Property Group Id
        /// </summary>
        /// <param name="productPropertyGroupId"></param>
        /// <returns></returns>
        public List<ProductProperty> GetProductPropertiesByProductPropertyGroupId(int productPropertyGroupId)
        {
            if (productPropertyGroupId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTY_BY_GROUPID_KEY, productPropertyGroupId);
            return _cacheManager.Get(key, () =>
            {
                //SELECT * FROM PRODUCT_PROPERTY PP WHERE PP.GROUPID = [input] AND PP.ISDELETED = 0 AND PP.ISACTIVED = 1
                var query = from pp in _productPropertyRepository.Table
                            where pp.GroupId == productPropertyGroupId &&
                                  !pp.IsDeleted &&
                                  pp.IsActived
                            orderby pp.DisplayOrder
                            select pp;
                var properties = query.ToList();
                return properties;
            });
        }

        /// <summary>
        /// Get list of Product Properties by a Property Group Id
        /// </summary>
        /// <param name="productPropertyGroupId"></param>
        /// <returns></returns>
        public List<ProductProperty> GetProductPropertiesByProductPropertyGroupIdForCategory(int productPropertyGroupId)
        {
            if (productPropertyGroupId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTY_BY_GROUPID_KEY, productPropertyGroupId);
            return _cacheManager.Get(key, () =>
            {
                //SELECT * FROM PRODUCT_PROPERTY PP WHERE PP.GROUPID = [input] AND PP.ISDELETED = 0 
                var query = from pp in _productPropertyRepository.Table
                            where pp.GroupId == productPropertyGroupId &&
                                  !pp.IsDeleted
                            // && pp.IsActived
                            orderby pp.DisplayOrder
                            select pp;
                var properties = query.ToList();
                return properties;
            });
        }

        /// <summary>
        /// Get list of Product Properties by Product Category Id, just for saving db queries
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        public List<ProductProperty> GetProductPropertiesByProductCategoryId(int productCategoryId)
        {
            if (productCategoryId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTY_BY_CATEGORYID_KEY, productCategoryId);
            return _cacheManager.Get(key, () =>
            {
                //SELECT * FROM PRODUCT_PROPERTYGROUP WHERE CATEGORYID = [input] AND ISDELETED = 0 AND ISACTIVED = 1 INTO QUERY
                var query = from ppg in _productPropertyGroupRepository.Table
                            where ppg.CategoryId == productCategoryId &&
                                  !ppg.IsDeleted &&
                                  ppg.IsActived
                            select ppg;
                //SELECT * FROM PRODUCT_PROPERTY PP JOIN QUERY PPG ON PPG.ID = PP.GROUPID WHERE PP.ISDELETED = 0 AND PP.ISACTIVED = 1
                var query1 = from ppg in query
                             join pp in _productPropertyRepository.Table
                             on ppg.Id equals pp.GroupId into rs_pp_ppg
                             from pp_ppg in rs_pp_ppg
                             where !pp_ppg.IsDeleted &&
                                   pp_ppg.IsActived
                             orderby pp_ppg.DisplayOrder
                             select pp_ppg;
                var properties = query1.ToList();
                List<ProductProperty> lst = new List<ProductProperty>();
                if (properties.Count > 0)
                {
                    foreach (var property in properties)
                    {
                        ProductProperty productproperty = new ProductProperty();
                        productproperty = property;
                        productproperty.ListProductPropertyValue = GetProductPropertyValuesByProductPropertyId(property.Id);
                        lst.Add(productproperty);
                    }
                }
                return lst;
            });
        }

        public void InsertProductProperty(ProductProperty productProperty)
        {
            if (productProperty == null)
                throw new ArgumentNullException("productProperty");

            _productPropertyRepository.Insert(productProperty);
            //cache
            _cacheManager.RemoveByPattern(string.Format(PRODUCTPROPERTY_BY_GROUPID_KEY, productProperty.GroupId));
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productProperty);
        }

        public void UpdateProductProperty(ProductProperty productProperty)
        {
            if (productProperty == null)
                throw new ArgumentNullException("productProperty");

            _productPropertyRepository.Update(productProperty);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productProperty);
        }

        public void DeleteProductProperty(ProductProperty productProperty)
        {
            if (productProperty == null)
                throw new ArgumentNullException("productProperty");

            productProperty.IsDeleted = true;
            _productPropertyRepository.Delete(productProperty);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productProperty);
        }

        #endregion

        #region Product Property Value

        public ProductPropertyValue GetProductPropertyValueById(int productPropertyValueId)
        {
            if (productPropertyValueId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTYVALUE_BY_VALUEID_KEY, productPropertyValueId);
            return _cacheManager.Get(key, () =>
            {
                return _productPropertyValueRepository.GetById(productPropertyValueId);
            });
        }

        public List<ProductPropertyValue> GetProductPropertyValuesByProductPropertyId(int productPropertyId)
        {
            if (productPropertyId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTYVALUE_BY_PROPERTYID_KEY, productPropertyId);
            return _cacheManager.Get(key, () =>
            {
                //SELECT * FROM PRODUCT_PROPERTY PP WHERE PP.PRODUCTPROPERTY = [input] AND PP.ISDELETED = 0 AND PP.ISACTIVED = 1
                var query = from ppv in _productPropertyValueRepository.Table
                            where ppv.PropertyId == productPropertyId &&
                                  !ppv.IsDeleted &&
                                  ppv.IsActived
                            orderby ppv.DisplayOrder
                            select ppv;
                var propertyValues = query.ToList();
                return propertyValues;
            });
        }

        public List<ProductPropertyValue> GetProductPropertyValuesByProductPropertyIdForCategory(int productPropertyId)
        {
            if (productPropertyId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTYVALUE_BY_PROPERTYID_KEY, productPropertyId);
            return _cacheManager.Get(key, () =>
            {
                //SELECT * FROM PRODUCT_PROPERTY PP WHERE PP.PRODUCTPROPERTY = [input] AND PP.ISDELETED = 0
                var query = from ppv in _productPropertyValueRepository.Table
                            where ppv.PropertyId == productPropertyId &&
                                  !ppv.IsDeleted
                            //&& ppv.IsActived
                            orderby ppv.DisplayOrder
                            select ppv;
                var propertyValues = query.ToList();
                return propertyValues;
            });
        }

        public List<ProductPropertyValue> GetProductPropertyValuesByProductCategoryId(int productCategoryId)
        {
            throw new NotImplementedException();
        }

        public void InsertProductPropertyValue(ProductPropertyValue productPropertyValue)
        {
            if (productPropertyValue == null)
                throw new ArgumentNullException("productPropertyValue");

            _productPropertyValueRepository.Insert(productPropertyValue);
            //cache
            _cacheManager.RemoveByPattern(string.Format(PRODUCTPROPERTYVALUE_BY_PROPERTYID_KEY, productPropertyValue.PropertyId));
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productPropertyValue);
        }

        public void UpdateProductPropertyValue(ProductPropertyValue productPropertyValue)
        {
            if (productPropertyValue == null)
                throw new ArgumentNullException("productPropertyValue");

            _productPropertyValueRepository.Update(productPropertyValue);
            //cache
            _cacheManager.RemoveByPattern(string.Format(PRODUCTPROPERTYVALUE_BY_PROPERTYID_KEY, productPropertyValue.PropertyId));
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productPropertyValue);
        }

        public void DeleteProductPropertyValue(ProductPropertyValue productPropertyValue)
        {
            if (productPropertyValue == null)
                throw new ArgumentNullException("productPropertyValue");

            _productPropertyValueRepository.Delete(productPropertyValue);
            //cache
            _cacheManager.RemoveByPattern(string.Format(PRODUCTPROPERTYVALUE_BY_PROPERTYID_KEY, productPropertyValue.PropertyId));
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productPropertyValue);
        }

        #endregion

        #region Product Property Value Mapping
        public void InsertProductPropertyValueMapping(ProductPropertyValueMapping productpropertyvaluemapping)
        {
            if (productpropertyvaluemapping == null)
                throw new ArgumentNullException("ProductPropertyValueMapping");

            _productPropertyValueMappingRepository.Insert(productpropertyvaluemapping);
            //cache
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productpropertyvaluemapping);
        }

        public void UpdateProductPropertyValueMapping(ProductPropertyValueMapping productpropertyvaluemapping)
        {
            if (productpropertyvaluemapping == null)
                throw new ArgumentNullException("ProductPropertyValueMapping");

            _productPropertyValueMappingRepository.Update(productpropertyvaluemapping);
            //cache
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productpropertyvaluemapping);
        }

        public void DeleteProductPropertyValueMapping(ProductPropertyValueMapping productpropertyvaluemapping)
        {
            if (productpropertyvaluemapping == null)
                throw new ArgumentNullException("ProductPropertyValueMapping");

            _productPropertyValueMappingRepository.Delete(productpropertyvaluemapping);
            //cache
            _cacheManager.RemoveByPattern(PRODUCTPROPERTY_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productpropertyvaluemapping);
        }

        public List<ProductPropertyValueMapping> GetListProductPropertyValueMapping(int propertysourceId)
        {
            if (propertysourceId == 0)
                return null;
            string key = string.Format(PRODUCTPROPERTYVALUEMAPPING_BY_PROPERTYID_KEY, propertysourceId);
            return _cacheManager.Get(key, () =>
            {
                //SELECT * FROM PRODUCT_PROPERTYVALUE_MAPPING PPVP WHERE PP.PRODUCTPROPERTY = [input] AND PP.ISDELETED = 0
                var query = from ppvm in _productPropertyValueMappingRepository.Table
                            where ppvm.SourceId == propertysourceId
                            select ppvm;
                var propertyvalueMappings = query.ToList();
                return propertyvalueMappings;
            });
        }
        #endregion

        #region Manufacture Categories
        public List<Category> GetAllCategoryByManufactureId(int manufacturerId)
        {
            if (manufacturerId == 0)
                return new List<Category>();
            string key = string.Format(CATEGORIES_BYMANUFACTURERID, manufacturerId);
            return _cacheManager.Get(key, () =>
            {
                var query = from lstcategory in
                                (from pc in _productCategoryRepository.Table
                                 join pm in _productManufacturerRepository.Table
                                 on pc.ProductId equals pm.ProductId
                                 join c in _categoryRepository.Table
                                 on pc.CategoryId equals c.Id
                                 where pm.ManufacturerId == manufacturerId && !c.Deleted
                                 orderby c.DisplayOrder
                                 select c)
                            group lstcategory by lstcategory.Id into lst
                            select lst;

                var temp = query.ToList();

                List<Category> lstcate = new List<Category>();
                foreach (var cate in temp)
                {
                    lstcate.Add(new Category
                    {
                        Id = cate.Key,
                        Name = cate.Select(m => m.Name).First(),
                        MetaTitle = cate.Select(m => m.MetaTitle).First(),
                        MetaDescription = cate.Select(m => m.MetaDescription).First(),
                        MetaKeywords = cate.Select(m => m.MetaKeywords).First()
                    });
                }
                return lstcate;
            });
        }
        #endregion
    }
}
