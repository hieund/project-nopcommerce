using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductPropertyMap : EntityTypeConfiguration<ProductProperty>
    {
        public ProductPropertyMap()
        {
            this.ToTable("Product_Property");
            this.HasKey(c => c.Id);
            this.Property(c => c.GroupId).IsRequired();
            this.Property(c => c.Name).IsRequired().HasMaxLength(200);
            this.Property(c => c.Description).HasMaxLength(2000);
            this.Property(c => c.IconSmall).HasMaxLength(50);
            this.Property(c => c.IconMedium).HasMaxLength(50);
            this.Property(c => c.IconLarge).HasMaxLength(50); 
            this.Property(c => c.IsMapping).IsRequired();
            this.Property(c => c.IsFilterExtension);
            this.Property(c => c.Type).IsRequired();
            this.Property(c => c.DisplayOrder).IsRequired();
            this.Property(c => c.IsDeleted).IsRequired();
            this.Property(c => c.DeletedBy).IsOptional();
            this.Property(c => c.DeletedAt).IsOptional();
            this.Property(c => c.IsActived).IsRequired();
            this.Property(c => c.ActivedAt).IsOptional();
            this.Property(c => c.ActivedBy).IsOptional();
            this.Property(c => c.CreatedAt).IsRequired();
            this.Property(c => c.CreatedBy).IsRequired();
        }
    }
}