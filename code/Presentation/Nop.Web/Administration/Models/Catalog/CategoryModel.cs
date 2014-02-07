using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Stores;
using Nop.Admin.Validators.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(CategoryValidator))]
    public partial class CategoryModel : BaseNopEntityModel, ILocalizedModel<CategoryLocalizedModel>
    {
        public CategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<CategoryLocalizedModel>();
            AvailableCategoryTemplates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryTemplate")]
        [AllowHtml]
        public int CategoryTemplateId { get; set; }
        public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PriceRanges")]
        [AllowHtml]
        public string PriceRanges { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CategoryLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        [AllowHtml]
        public string HtmlCompare { get; set; }

        //ACL
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public int[] SelectedCustomerRoleIds { get; set; }

        //Store mapping
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }


        public IList<DropDownItem> ParentCategories { get; set; }


        //discounts
        public List<Discount> AvailableDiscounts { get; set; }
        public int[] SelectedDiscountIds { get; set; }


        #region Nested classes

        public partial class CategoryProductModel : BaseNopEntityModel
        {
            public int CategoryId { get; set; }

            public int ProductId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Product")]
            public string ProductName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.DisplayOrder")]
            //we don't name it DisplayOrder because Telerik has a small bug 
            //"if we have one more editor with the same name on a page, it doesn't allow editing"
            //in our case it's category.DisplayOrder
            public int DisplayOrder1 { get; set; }
        }

        public partial class AddCategoryProductModel : BaseNopModel
        {
            public AddCategoryProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
            }
            public GridModel<ProductModel> Products { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }

            public int CategoryId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        #region Product Property Group
        public partial class ProductPropertyGroupModel : BaseNopEntityModel
        {
            public int GroupId { get; set; }

            public int CategoryId { get; set; }

            [Required(ErrorMessage = "Admin.Catalog.Categories.ProductPropertyGroup.Fields.Name")]
            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.Description")]
            //[UIHint("EditorInGrid")]
            //[AllowHtml]
            public string Description { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.IsMapping")]
            public bool IsMapping { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.IsActived")]
            public bool IsActived { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.IconSmall")]
            public string IconSmall { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.IconMedium")]
            public string IconMedium { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.IconLarge")]
            public string IconLarge { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyGroup.Fields.DisplayOrder")]
            //we don't name it DisplayOrder because Telerik has a small bug 
            //"if we have one more editor with the same name on a page, it doesn't allow editing"
            //in our case it's category.DisplayOrder
            public int DisplayOrder1 { get; set; }
        }
        #endregion

        #region Product Property
        public partial class ProductPropertyModel : BaseNopEntityModel
        {
            public int PropertyId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.Description")]
            //[UIHint("EditorInGrid")]
            //[AllowHtml]
            public string Description { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.IsMapping")]
            public bool IsMapping { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.IsActived")]
            public bool IsActived { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.IconSmall")]
            public string IconSmall { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.IconMedium")]
            public string IconMedium { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.IconLarge")]
            public string IconLarge { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.DisplayOrder")]
            //we don't name it DisplayOrder because Telerik has a small bug 
            //"if we have one more editor with the same name on a page, it doesn't allow editing"
            //in our case it's category.DisplayOrder
            public int DisplayOrder1 { get; set; }

            public int GroupId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.Type")]
            [UIHint("ProductPropertyType")]
            public string PropertyType { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductProperty.Fields.IsFilterExtension")]
            public bool IsFilterExtension { get; set; }
        }
        #endregion

        #region Product Property Value
        public partial class ProductPropertyValueModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValue.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValue.Fields.Value")]
            public int Value { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValue.Fields.IsActived")]
            public bool IsActived { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValue.Fields.DisplayOrder")]
            //we don't name it DisplayOrder because Telerik has a small bug 
            //"if we have one more editor with the same name on a page, it doesn't allow editing"
            //in our case it's category.DisplayOrder
            public int DisplayOrder1 { get; set; }

            public int ProductPropertyId { get; set; }
        }
        #endregion

        #region Product Property Value Mapping
        public partial class ProductPropertyValueMappingModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValueMapping.Fields.SourceId")]
            public int SourceId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValueMapping.Fields.SourceName")]
            public string SourceName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValueMapping.Fields.DestinationId")]
            public int DestinationId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValueMapping.Fields.DestinationName")]
            public string DestinationName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.ProductPropertyValueMapping.Fields.MappingType")]
            [UIHint("PropertyMappingValueType")]
            public byte MappingType { get; set; }

        }
        #endregion

        #endregion
    }

    public partial class CategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}