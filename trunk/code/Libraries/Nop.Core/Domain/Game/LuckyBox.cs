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
    public partial class LuckyBox : BaseEntity, ISlugSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of rows
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Gets or sets the number of win cycle
        /// </summary>
        public int WinCycle { get; set; }

        /// <summary>
        /// Gets or sets the date and time of Begin LuckyBox
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of End LuckyBox
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

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

        /// <summary>
        /// Gets or sets the state of receipt require
        /// </summary>
        public bool IsReceiptRequired { get; set; }
    }
}
