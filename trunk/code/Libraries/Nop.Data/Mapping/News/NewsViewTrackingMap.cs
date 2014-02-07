using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    public partial class NewsViewTrackingMap : EntityTypeConfiguration<NewsViewTracking>
    {
        public NewsViewTrackingMap()
        {
            this.ToTable("News_ViewTracking");
            this.HasKey(c => c.Id);
        }
    }
}