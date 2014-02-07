using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Customers;
using Nop.Admin.Validators.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(ProductPropertyGroupValidator))]
    public partial class ProductPropertyGroupModel : BaseNopEntityModel, ILocalizedModel<ProductPropertyGroupLocalizedModel>
    {
        public ProductPropertyGroupModel()
        {
            Locales = new List<ProductPropertyGroupLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.CategoryId")]
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.ProductPropertyGroupName")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.IsMapping")]
        public bool IsMapping { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.IconSmall")]
        public string IconSmall { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.IconMedium")]
        public string IconMedium { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.IconLarge")]
        public string IconLarge { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ProductPropertyGroupLocalizedModel> Locales { get; set; }
    }

    public partial class ProductPropertyGroupLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.ProductPropertyGroupName")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductPropertyGroup.Fields.Description")]
        public string Description { get; set; }
    }
}