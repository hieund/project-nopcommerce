using Nop.Core.Domain.Game;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Game
{
    public class LuckyBoxDetailModel : BaseNopEntityModel
    {
        public int LuckyBoxId { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDetail.Fields.GiftName")]
        public string GiftName { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDetail.Fields.Picture")]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        
        [NopResourceDisplayName("Admin.Game.LuckyBoxDetail.Fields.Picture")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Game.LuckyBoxDetail.Fields.NumberPerCycle")]
        public int NumberPerCycle { get; set; }

        public LuckyBoxDraw WinDraw { get; set; }
    }
}