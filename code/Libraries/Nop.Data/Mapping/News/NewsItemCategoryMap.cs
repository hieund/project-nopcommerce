using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    public partial class NewsItemCategoryMap : EntityTypeConfiguration<NewsItemCategory>
    {
        public NewsItemCategoryMap()
        {
            this.ToTable("News_Category_Mapping");
            this.HasKey(pc => pc.Id);

            this.HasRequired(pc => pc.NewsCategory)
                .WithMany()
                .HasForeignKey(pc => pc.CategoryId);


            this.HasRequired(pc => pc.NewsItem)
                .WithMany(p => p.NewsItemCategories)
                .HasForeignKey(pc => pc.NewsId);
        }
    }
}