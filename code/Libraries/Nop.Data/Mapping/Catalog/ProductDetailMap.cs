using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductDetailMap : EntityTypeConfiguration<ProductDetail>
    {
        public ProductDetailMap()
        {
            this.ToTable("Product_Detail");
            this.HasKey(c => c.Id);
            this.Property(c => c.GroupId).IsRequired();
            this.Property(c => c.CategoryId).IsRequired();
            this.Property(c => c.ProductId).IsRequired();
            this.Property(c => c.PropertyId).IsRequired();
            this.Property(c => c.PropertyType).IsRequired();
            this.Property(c => c.Value).HasMaxLength(2000);
        }
    }
}