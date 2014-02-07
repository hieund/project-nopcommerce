using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.News
{
    public partial class NewsCategoryListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchCategoryName { get; set; }

        public GridModel<NewsCategoryModel> NewsCategories { get; set; }
    }
}