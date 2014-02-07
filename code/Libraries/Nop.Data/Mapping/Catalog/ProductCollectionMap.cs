using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductCollectionMap : EntityTypeConfiguration<ProductCollection>
    {
        public ProductCollectionMap()
        {
            this.ToTable("ProductCollection");
            this.HasKey(c => c.Id);
        }
    }
}