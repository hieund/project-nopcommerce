using System;
namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public partial class Promotion : BaseEntity
    {
        public string PromotionName { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserCreated { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UserUpdated { get; set; }
        public Boolean Published { get; set; }
        public Boolean Deleted { get; set; }
    }

}
