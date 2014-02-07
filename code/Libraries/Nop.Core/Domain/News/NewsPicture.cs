﻿using Nop.Core.Domain.Media;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news picture mapping
    /// </summary>
    public partial class NewsPicture : BaseEntity
    {
        /// <summary>
        /// Gets or sets the news identifier
        /// </summary>
        public int NewsId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the picture
        /// </summary>
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets the news
        /// </summary>
        public virtual NewsItem NewsItem { get; set; }
    }

}
