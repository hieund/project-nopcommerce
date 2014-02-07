using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;

namespace Nop.Web.Models.Checkout
{
    public partial class QuickOrderModel : BaseNopModel
    {
        public QuickOrderModel()
        {
            ProductVariant = new ProductVariant();
            BillingAddress = new Address();
        }

        public ProductVariant ProductVariant { get; set; }
        public Address BillingAddress { get; set; }
        public int ProductVariantId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Price { get; set; }
        public string OldPrice { get; set; }
    }
}