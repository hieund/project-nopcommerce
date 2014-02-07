using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductPropertyValueMappingMap : EntityTypeConfiguration<ProductPropertyValueMapping>
    {
        public ProductPropertyValueMappingMap()
        {
            this.ToTable("Product_PropertyValue_Mapping");
            this.HasKey(ppvm => ppvm.Id);
            this.Property(ppvm => ppvm.SourceId).IsRequired();
            this.Property(ppvm => ppvm.DestinationId).IsRequired();
            this.Property(ppvm => ppvm.MapType).IsRequired();

            //this.HasRequired(ppv =>ppv.ProductPropertyValueMapping)
        }
    }
}