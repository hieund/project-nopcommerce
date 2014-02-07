using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class PromotionListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.Promotion.List.SearchPromotionName")]
        [AllowHtml]
        public string SearchPromotionName { get; set; }

        public GridModel<PromotionModel> Promotions { get; set; }
    }
}