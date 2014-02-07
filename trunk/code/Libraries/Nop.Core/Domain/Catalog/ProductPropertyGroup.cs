using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product property group
    /// </summary>
    public partial class ProductPropertyGroup : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual string IconSmall { get; set; }
        public virtual string IconMedium { get; set; }
        public virtual string IconLarge { get; set; }
        public virtual bool IsMapping { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual bool IsDeleted { get; set; }
        public virtual int? DeletedBy { get; set; }
        public virtual DateTime? DeletedAt { get; set; }
        public virtual bool IsActived { get; set; }
        public virtual int? ActivedBy { get; set; }
        public virtual DateTime? ActivedAt { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public virtual bool SubjectToAcl { get; set; }
    }

}
