using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductProperty : BaseEntity, ILocalizedEntity
    {
        private ICollection<ProductPropertyValue> _lstproductpropertyvalue;

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int GroupId { get; set; }
        public virtual string IconSmall { get; set; }
        public virtual string IconMedium { get; set; }
        public virtual string IconLarge { get; set; }
        public virtual bool IsMapping { get; set; }
        public virtual byte Type { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual bool IsFilterExtension { get; set; }

        public virtual bool IsDeleted { get; set; }
        public virtual int? DeletedBy { get; set; }
        public virtual DateTime? DeletedAt { get; set; }
        public virtual bool IsActived { get; set; }
        public virtual int? ActivedBy { get; set; }
        public virtual DateTime? ActivedAt { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public virtual ICollection<ProductPropertyValue> ListProductPropertyValue
        {
            get { return _lstproductpropertyvalue ?? (_lstproductpropertyvalue = new List<ProductPropertyValue>()); }
            set { _lstproductpropertyvalue = value; }
        }
    }
}
