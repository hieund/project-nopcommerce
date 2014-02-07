using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Game
{
    public partial class LuckyBoxDraw : BaseEntity
    {
        public int LuckyBoxId { get; set; }
        public int LuckyBoxDetailId { get; set; }
        public string GiftName { get; set; }
        public string LuckyBoxCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool IsCompleted { get; set; }
    }
}
