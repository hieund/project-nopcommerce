using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    public partial class NewsPictureMap : EntityTypeConfiguration<NewsPicture>
    {
        public NewsPictureMap()
        {
            this.ToTable("News_Picture_Mapping");
            this.HasKey(pp => pp.Id);

            this.HasRequired(pp => pp.Picture)
                .WithMany(p => p.NewsPictures)
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.NewsItem)
                .WithMany(p => p.NewsPictures)
                .HasForeignKey(pp => pp.NewsId);
        }
    }
}