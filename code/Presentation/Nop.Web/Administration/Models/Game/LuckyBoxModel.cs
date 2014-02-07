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
    public class LuckyBoxModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.Rows")]
        public int Rows { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.Columns")]
        public int Columns { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.WinCycle")]
        public int WinCycle { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.IsReceiptRequired")]
        public bool IsReceiptRequired { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.Begin")]
        public DateTime BeginDate { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.End")]
        public DateTime EndDate { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBox.Fields.Published")]
        public bool Published { get; set; }

        public List<LuckyBoxDetailModel> Gifts { get; set; }

        public List<LuckyBoxDrawModel> Draws { get; set; }

        public LuckyBoxDetailModel AddGiftModel { get; set; }

        public List<LuckyBoxDetailModel> LuckyTable { get; set; }

        public List<LuckyBoxDetailModel> LuckyTableDisplay { get; set; }
    }
}