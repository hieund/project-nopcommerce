using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductConfigModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public int GroupId { get; set; }

        public int PropertyId { get; set; }

        public string PropertyName { get; set; }

        public int PropertyType { get; set; }

        public string Value { get; set; }
    }
}