using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    public partial class RelatedNewsMap : EntityTypeConfiguration<RelatedNews>
    {
        public RelatedNewsMap()
        {
            this.ToTable("RelatedNews");
            this.HasKey(c => c.Id);
        }
    }
}