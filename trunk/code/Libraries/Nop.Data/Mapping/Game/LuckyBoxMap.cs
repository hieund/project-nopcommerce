using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Game;

namespace Nop.Data.Mapping.Game
{
    public partial class LuckyBoxMap : EntityTypeConfiguration<LuckyBox>
    {
        public LuckyBoxMap()
        {
            this.ToTable("LuckyBox");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);
            this.Property(m => m.Description).HasMaxLength(400);
            this.Property(m => m.Rows);
            this.Property(m => m.Columns);
            this.Property(m => m.WinCycle);
            this.Property(m => m.IsReceiptRequired);
            this.Property(m => m.BeginDate);
            this.Property(m => m.EndDate);
            this.Property(m => m.Deleted);
            this.Property(m => m.Published);
            this.Property(m => m.CreatedOnUtc);
            this.Property(m => m.UpdatedOnUtc);
        }
    }
}