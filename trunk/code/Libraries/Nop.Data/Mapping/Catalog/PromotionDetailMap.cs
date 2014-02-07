using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PromotionDetailMap : EntityTypeConfiguration<PromotionDetail>
    {
        public PromotionDetailMap()
        {
            this.ToTable("PromotionDetail");
            this.HasKey(c => c.Id);
            this.Property(c => c.PromotionId).IsRequired();
            this.Property(c => c.ProductId).IsRequired();
            this.Property(c => c.PromotionName).IsRequired().HasMaxLength(255);
            this.Property(c => c.ProductName).IsRequired().HasMaxLength(400);
            this.Property(c => c.CategoryId);
            this.Property(c => c.CategoryName).IsRequired().HasMaxLength(255);
            this.Property(c => c.ManufactureId);
            this.Property(c => c.ManufactureName).IsRequired().HasMaxLength(255);
        }
    }
}