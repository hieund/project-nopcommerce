using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Stores;
using Nop.Admin.Validators.News;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Localization;
using Telerik.Web.Mvc.UI;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.News
{
    [Validator(typeof(NewsCategoryValidator))]
    public partial class NewsCategoryModel : BaseNopEntityModel, ILocalizedModel<NewsCategoryLocalizedModel>
    {
        public NewsCategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<NewsCategoryLocalizedModel>();
            AvailableCategoryTemplates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.News.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryTemplate")]
        [AllowHtml]
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

        [NopResourceDisplayName("Admin.News.Categories.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<NewsCategoryLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        public IList<DropDownItem> ParentCategories { get; set; }

        #region Nested classes

        public partial class CategoryNewsItemModel : BaseNopEntityModel
        {
            public int CategoryId { get; set; }

            public int NewsId { get; set; }

            [NopResourceDisplayName("Admin.News.Categories.NewsItems.Fields.Title")]
            public string Title { get; set; }

            [NopResourceDisplayName("Admin.News.Categories.NewsItem.Fields.IsFeatured")]
            public bool IsFeatured { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.DisplayOrder")]
            //we don't name it DisplayOrder because Telerik has a small bug 
            //"if we have one more editor with the same name on a page, it doesn't allow editing"
            //in our case it's category.DisplayOrder
            public int DisplayOrder1 { get; set; }
        }

        public partial class AddCategoryNewsItemModel : BaseNopModel
        {
            public AddCategoryNewsItemModel()
            {
                AvailableCategories = new List<SelectListItem>();
            }
            public GridModel<NewsItemModel> NewsItems { get; set; }

            [NopResourceDisplayName("Admin.News.NewsItems.List.SearchTitle")]
            [AllowHtml]
            public string SearchTitle { get; set; }
            [NopResourceDisplayName("Admin.News.NewsItems.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            
            public IList<SelectListItem> AvailableCategories { get; set; }

            public int CategoryId { get; set; }

            public int[] SelectedNewsIds { get; set; }
        }

        #endregion
    }

    public partial class NewsCategoryLocalizedModel : ILocalizedModelLocal
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