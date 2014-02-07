using System;
namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public partial class PromotionDetail : BaseEntity
    {
        public int PromotionId { get; set; }
        public int ProductId { get; set; }
        public string PromotionName{ get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ManufactureId { get; set; }
        public string ManufactureName { get; set; }
    }

}
