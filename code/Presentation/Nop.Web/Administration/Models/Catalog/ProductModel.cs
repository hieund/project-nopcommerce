﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Stores;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(ProductValidator))]
    public partial class ProductModel : BaseNopEntityModel, ILocalizedModel<ProductLocalizedModel>
    {
        public ProductModel()
        {
            Locales = new List<ProductLocalizedModel>();
            ProductVariantModels = new List<ProductVariantModel>();
            ProductPictureModels = new List<ProductPictureModel>();
            CopyProductModel = new CopyProductModel();
            AvailableProductTemplates = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductTemplate")]
        public int ProductTemplateId { get; set; }
        public IList<SelectListItem> AvailableProductTemplates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Vendor")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductTags")]
        public string ProductTags { get; set; }
        
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Published")]
        public bool Published { get; set; }
        


        public IList<ProductLocalizedModel> Locales { get; set; }


        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public int[] SelectedCustomerRoleIds { get; set; }

        //Store mapping
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }

        //existing product variants
        public IList<ProductVariantModel> ProductVariantModels { get; set; }
        //default variant (the first one) 
        public ProductVariantModel FirstProductVariantModel { get; set; }


        //vendor
        public bool IsLoggedInAsVendor { get; set; }



        //categories
        public int NumberOfAvailableCategories { get; set; }

        //manufacturers
        public int NumberOfAvailableManufacturers { get; set; }
        




        //pictures
        public ProductPictureModel AddPictureModel { get; set; }
        public IList<ProductPictureModel> ProductPictureModels { get; set; }





        //add specification attribute model
        public AddProductSpecificationAttributeModel AddSpecificationAttributeModel { get; set; }



        //copy product
        public CopyProductModel CopyProductModel { get; set; }
        
        #region Nested classes
        
        public partial class AddProductSpecificationAttributeModel : BaseNopEntityModel
        {
            public AddProductSpecificationAttributeModel()
            {
                AvailableAttributes = new List<SelectListItem>();
                AvailableOptions = new List<SelectListItem>();
            }
            
            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute")]
            public int SpecificationAttributeId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttributeOption")]
            public int SpecificationAttributeOptionId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue")]
            public string CustomValue { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.AllowFiltering")]
            public bool AllowFiltering { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.ShowOnProductPage")]
            public bool ShowOnProductPage { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public IList<SelectListItem> AvailableAttributes { get; set; }
            public IList<SelectListItem> AvailableOptions { get; set; }
        }
        
        public partial class ProductPictureModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        
        public partial class ProductCategoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.Categories.Fields.Category")]
            [UIHint("ProductCategory")]
            public string Category { get; set; }

            public int ProductId { get; set; }

            public int CategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Categories.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class ProductManufacturerModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.Manufacturers.Fields.Manufacturer")]
            [UIHint("ProductManufacturer")]
            public string Manufacturer { get; set; }

            public int ProductId { get; set; }

            public int ManufacturerId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Manufacturers.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Manufacturers.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class RelatedProductModel : BaseNopEntityModel
        {
            public int ProductId1 { get; set; }

            public int ProductId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.Product")]
            public string Product2Name { get; set; }
            
            [NopResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class AddRelatedProductModel : BaseNopModel
        {
            public AddRelatedProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
            }

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

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class AddPromotionProductModel : BaseNopModel
        {
            public AddPromotionProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
            }

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

            public int PromotionId { get; set; }

            public string PromotionName { get; set; }

            public int[] SelectedProductIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class CrossSellProductModel : BaseNopEntityModel
        {
            public int ProductId1 { get; set; }

            public int ProductId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.CrossSells.Fields.Product")]
            public string Product2Name { get; set; }
        }

        public partial class AddCrossSellProductModel : BaseNopModel
        {
            public AddCrossSellProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
            }

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

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }


        public partial class ProductPropertyGroupModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.PropertyGroups.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.PropertyGroups.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class ProductPropertyModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.Property.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Property.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public int GroupId { get; set; }

            public string Value { get; set; }

            public byte Type { get; set; }
        }

        public partial class ProductPropertyValueModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.PropertyValue.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.PropertyValue.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public int PropertyId { get; set; }

            public int Value { get; set; }
        }

        public partial class ProductDetailModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            public int CategoryId { get; set; }

            public int GroupId { get; set; }

            public int PropertyId { get; set; }

            public int PropertyType { get; set; }

            public string Value { get; set; }
        }

        #endregion
    }

    public partial class ProductLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}