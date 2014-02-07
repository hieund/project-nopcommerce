using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductPropertyValueMapping : BaseEntity, ILocalizedEntity
    {
        public virtual int SourceId { get; set; }

        public virtual int DestinationId { get; set; }

        public virtual byte MapType { get; set; }

        //public virtual ProductPropertyValue ProductPropertyValue { get; set; }
    }
}
