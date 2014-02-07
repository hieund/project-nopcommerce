using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Game;

namespace Nop.Data.Mapping.Game
{
    public partial class LuckyBoxDrawMap : EntityTypeConfiguration<LuckyBoxDraw>
    {
        public LuckyBoxDrawMap()
        {
            this.ToTable("LuckyBoxDraw");
            this.HasKey(m => m.Id);
            this.Property(m => m.LuckyBoxId);
            this.Property(m => m.LuckyBoxDetailId);
            this.Property(m => m.LuckyBoxCode);
            this.Property(m => m.CustomerAddress);
            this.Property(m => m.CustomerEmail);
            this.Property(m => m.CustomerName);
            this.Property(m => m.CustomerPhone);
            this.Property(m => m.ReceiptNo);
            this.Property(m => m.CreatedOnUtc);
            this.Property(m => m.IsCompleted);
        }
    }
}