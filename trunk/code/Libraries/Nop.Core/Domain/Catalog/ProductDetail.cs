using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductDetail : BaseEntity, ILocalizedEntity
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public int GroupId { get; set; }

        public int PropertyId { get; set; }

        public int PropertyType { get; set; }

        public string Value { get; set; }
    }
}
