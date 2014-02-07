using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Game
{
    public class LuckyBoxDrawModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Game.LuckyBoxDraw.Fields.Code")]
        public string LuckyBoxCode { get; set; }

        public int LuckyBoxId { get; set; }

        public int LuckyBoxDetailId { get; set; }
        [NopResourceDisplayName("Admin.Game.LuckyBoxDetail.Fields.GiftName")]
        public string GiftName { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDraw.Fields.CustomerName")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDraw.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDraw.Fields.CustomerPhone")]
        public string CustomerPhone { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDraw.Fields.CustomerAddress")]
        public string CustomerAddress { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDraw.Fields.ReceiptNo")]
        public string ReceiptNo { get; set; }

        public DateTime CreatedOnUtd { get; set; }

        public bool IsCompleted { get; set; }
    }
}