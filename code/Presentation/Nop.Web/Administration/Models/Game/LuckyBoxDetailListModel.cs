using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Game
{
    public class LuckyBoxDetailListModel : BaseNopEntityModel
    {
        public GridModel<LuckyBoxDetailModel> Gifts { get; set; }
    }
}