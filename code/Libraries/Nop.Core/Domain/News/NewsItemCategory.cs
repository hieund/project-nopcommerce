namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a new category mapping
    /// </summary>
    public partial class NewsItemCategory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the news identifier
        /// </summary>
        public int NewsId { get; set; }

        /// <summary>
        /// Gets or sets the news category identifier
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news is featured
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual NewsCategory NewsCategory { get; set; }

        /// <summary>
        /// Gets the news
        /// </summary>
        public virtual NewsItem NewsItem { get; set; }

    }

}
