using System;
using System.Web;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductOverviewModel : BaseNopEntityModel
    {
        public ProductOverviewModel()
        {
            ProductPrice = new ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            SpecificationAttributeModels = new List<ProductSpecificationModel>();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool isNew { get; set; }
        public bool isHot { get; set; }
        //price
        public ProductPriceModel ProductPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //specification attributes
        public IList<ProductSpecificationModel> SpecificationAttributeModels { get; set; }

        #region Nested Classes

        public partial class ProductPriceModel : BaseNopModel
        {
            public int Id { get; set; }
            public string OldPrice { get; set; }
            public string Price { get; set; }
            public decimal OldPriceValue { get; set; }
            public decimal PriceValue { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }

            public bool AvailableForPreOrder { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }
        }

        #endregion
    }
}