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
using System;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(PromotionValidator))]
    public partial class PromotionModel : BaseNopEntityModel, ILocalizedModel<PromotionLocalizedModel>
    {
        public PromotionModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<PromotionLocalizedModel>();
            PromotionDetailModels = new List<PromotionDetailModel>();
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
        [AllowHtml]
        public string SearchProductName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
        public int SearchCategoryId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public int[] SelectedProductIds { get; set; }

        [Display(Name="Tên chương trình")]
        public string PromotionName { get; set; }

        [Display(Name="Nội dung chương trình")]
        public string Description { get; set; }

        public IList<PromotionLocalizedModel> Locales { get; set; }

        public IList<PromotionDetailModel> PromotionDetailModels { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }

        public decimal TotalAmount { get; set; }

        [NopResourceDisplayName("Ngày bắt đầu")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Ngày kết thúc")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDate { get; set; }

        public DateTime? CreateDate { get; set; }

        public int UserCreated { get; set; }

        [NopResourceDisplayName("Kích hoạt")]
        public bool Published { get; set; }

        [NopResourceDisplayName("TT Hiển thị")]
        public int DisplayOrder { get; set; }

        public bool Deleted { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int UserUpdated { get; set; }

    }
    public partial class PromotionDetailModel : BaseNopEntityModel
    {
        public int Id { get; set; }

        public int PromotionId { get; set; }

        public string PromotionName { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }
    }
    public partial class PromotionLocalizedModel : ILocalizedModelLocal
    {

        public int LanguageId { get; set; }

        [Display(Name="Tên chương trình khuyến mãi")]
        public string PromotionName { get; set; }

        [Display(Description="Nội dung khuyến mãi")]
        public string Description { get; set; }

        public decimal TotalAmount { get; set; }

        [NopResourceDisplayName("Ngày bắt đầu")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Ngày kết thúc")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDate { get; set; }
        
        public DateTime? CreateDate { get; set; }

        public int UserCreated { get; set; }

        [NopResourceDisplayName("Kích hoạt")]
        public bool Published { get; set; }

        [NopResourceDisplayName("TT Hiển thị")]
        public int DisplayOrder { get; set; }

        public bool Deleted { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int UserUpdated { get; set; }
    }
}