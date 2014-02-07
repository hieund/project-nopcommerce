using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Game;

namespace Nop.Data.Mapping.Game
{
    public partial class LuckyBoxDetailMap : EntityTypeConfiguration<LuckyBoxDetail>
    {
        public LuckyBoxDetailMap()
        {
            this.ToTable("LuckyBoxDetail");
            this.HasKey(m => m.Id);
            this.Property(m => m.GiftName).IsRequired().HasMaxLength(100);
            this.Property(m => m.PictureId).IsOptional();
            this.Property(m => m.NumberPerCycle);
            this.Property(m => m.Deleted);
            this.Property(m => m.CreatedOnUtc);
            this.Property(m => m.UpdatedOnUtc);
        }
    }
}