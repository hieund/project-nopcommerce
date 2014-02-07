using System;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Game
{
    /// <summary>
    /// Represents a LuckyBox
    /// </summary>
    public partial class LuckyBoxDetail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the lucky box program id
        /// </summary>
        public int LuckyBoxId { get; set; }

        /// <summary>
        /// Gets or sets the gift name
        /// </summary>
        public string GiftName { get; set; }

        /// <summary>
        /// Gets or sets the picture id
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the number of win per cycle
        /// </summary>
        public int NumberPerCycle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
