using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;
namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface ICategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteCategory(Category category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<Category> GetAllCategories(string categoryName = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        IList<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategoriesDisplayedOnHomePage();

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        Category GetCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        void InsertCategory(Category category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateCategory(Category category);

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateHasDiscountsApplied(Category category);

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        void DeleteProductCategory(ProductCategory productCategory);

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IPagedList<ProductCategory> GetProductCategoriesByCategoryId(int categoryId,
            int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        IList<ProductCategory> GetProductCategoriesByProductId(int productId, bool showHidden = false);

        /// <summary>
        /// Get a total number of featured products by category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Number of featured products</returns>
        int GetTotalNumberOfFeaturedProducts(int categoryId);

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        ProductCategory GetProductCategoryById(int productCategoryId);

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void InsertProductCategory(ProductCategory productCategory);

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void UpdateProductCategory(ProductCategory productCategory);

        #region Product Property Groups

        /// <summary>
        /// Gets a product properties group mapping collection
        /// </summary>
        /// <param name="productpropertiesgroupid">productpropertiesgroup identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        ProductPropertyGroup GetProductPropertyGroupById(int productPropertyGroupId);

        /// <summary>
        /// Get property groups of category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IList<ProductPropertyGroup> GetProductPropertyGroupByCategoryId(int categoryId);

        /// <summary>
        /// Inserts a ProductPropertyGroup mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void InsertProductPropertyGroup(ProductPropertyGroup productPropertyGroup);

        /// <summary>
        /// Updates the ProductPropertyGroup mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void UpdateProductPropertyGroup(ProductPropertyGroup productPropertyGroup);

        /// <summary>
        /// Deletes a product properties group
        /// </summary>
        /// <param name="productpropertygroup">productpropertygroup</param>
        void DeleteProductPropertyGroup(ProductPropertyGroup productPropertyGroup);

        #endregion

        #region Product Properties

        /// <summary>
        /// Get detail of Product Property
        /// </summary>
        /// <param name="productPropertyId"></param>
        /// <returns></returns>
        ProductProperty GetProductPropertyById(int productPropertyId);

        /// <summary>
        /// Get list of Product Properties by it's group
        /// </summary>
        /// <param name="productPropertyGroupId"></param>
        /// <returns></returns>
        List<ProductProperty> GetProductPropertiesByProductPropertyGroupId(int productPropertyGroupId);

        /// <summary>
        /// Get list of Product Properties by it's group
        /// </summary>
        /// <param name="productPropertyGroupId"></param>
        /// <returns></returns>
        List<ProductProperty> GetProductPropertiesByProductPropertyGroupIdForCategory(int productPropertyGroupId);
        /// <summary>
        /// Get list of Product Properties by it's category
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        List<ProductProperty> GetProductPropertiesByProductCategoryId(int productCategoryId);

        /// <summary>
        /// Insert a Product Property
        /// </summary>
        /// <param name="productProperty"></param>
        void InsertProductProperty(ProductProperty productProperty);

        /// <summary>
        /// Update a Product Property
        /// </summary>
        /// <param name="productProperty"></param>
        void UpdateProductProperty(ProductProperty productProperty);

        /// <summary>
        /// Delete a Product Property
        /// </summary>
        /// <param name="productProperty"></param>
        void DeleteProductProperty(ProductProperty productProperty);

        #endregion

        #region Product Property Values

        /// <summary>
        /// Get detail of Product Property Value
        /// </summary>
        /// <param name="productPropertyValueId"></param>
        /// <returns></returns>
        ProductPropertyValue GetProductPropertyValueById(int productPropertyValueId);

        /// <summary>
        /// Get list of Product Property Values by it's Property
        /// </summary>
        /// <param name="productPropertyId"></param>
        /// <returns></returns>
        List<ProductPropertyValue> GetProductPropertyValuesByProductPropertyId(int productPropertyId);

        /// <summary>
        /// Get list of Product Property Values by it's Property for Category
        /// </summary>
        /// <param name="productPropertyId"></param>
        /// <returns></returns>
        List<ProductPropertyValue> GetProductPropertyValuesByProductPropertyIdForCategory(int productPropertyId);

        /// <summary>
        /// Get list of Product Property Values by it's category
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        List<ProductPropertyValue> GetProductPropertyValuesByProductCategoryId(int productCategoryId);

        /// <summary>
        /// Insert a Product Property Value
        /// </summary>
        /// <param name="productPropertyValue"></param>
        void InsertProductPropertyValue(ProductPropertyValue productPropertyValue);

        /// <summary>
        /// Update a Product Property Value
        /// </summary>
        /// <param name="productPropertyValue"></param>
        void UpdateProductPropertyValue(ProductPropertyValue productPropertyValue);

        /// <summary>
        /// Delete a Product Property Value
        /// </summary>
        /// <param name="productPropertyValue"></param>
        void DeleteProductPropertyValue(ProductPropertyValue productPropertyValue);

        #endregion

        #region Product Property Value Mapping
        /// <summary>
        /// Insert the product property value mapping
        /// </summary>
        /// <param name="productpropertyvaluemapping"></param>
        void InsertProductPropertyValueMapping(ProductPropertyValueMapping productpropertyvaluemapping);

        /// <summary>
        /// Update the product property value mapping
        /// </summary>
        /// <param name="productpropertyvaluemapping"></param>
        void UpdateProductPropertyValueMapping(ProductPropertyValueMapping productpropertyvaluemapping);

        /// <summary>
        /// Delete the product property value mapping
        /// </summary>
        /// <param name="productpropertyvaluemapping"></param>
        void DeleteProductPropertyValueMapping(ProductPropertyValueMapping productpropertyvaluemapping);

        /// <summary>
        /// Get list the product property value mapping by sourceId
        /// </summary>
        /// <param name="productpropertyvaluemapping"></param>
        List<ProductPropertyValueMapping> GetListProductPropertyValueMapping(int propertysourceId);

        #endregion

        #region Manufacture Categories

        /// <summary>
        /// Lay tat ca danh sach nganh hang theo ma nha san xuat 
        /// </summary>
        /// <returns></returns>
        List<Category> GetAllCategoryByManufactureId(int manufacturerId);

        #endregion
    }
}
