using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PromotionMap : EntityTypeConfiguration<Promotion>
    {
        public PromotionMap()
        {
            this.ToTable("Promotion");
            this.HasKey(c => c.Id);
            this.Property(c => c.PromotionName).IsRequired().HasMaxLength(255);
            this.Property(c => c.Description).HasMaxLength(255);
            this.Property(c => c.TotalAmount).IsRequired();
            this.Property(c => c.StartDate).IsRequired();
            this.Property(c => c.EndDate).IsRequired();
            this.Property(c => c.DisplayOrder).IsRequired();
            this.Property(c => c.Published).IsRequired();
            this.Property(c => c.Deleted).IsRequired();
            
        }
    }
}