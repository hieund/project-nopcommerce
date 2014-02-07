namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a related news
    /// </summary>
    public partial class RelatedNews : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first news identifier
        /// </summary>
        public int NewsId1 { get; set; }

        /// <summary>
        /// Gets or sets the second news identifier
        /// </summary>
        public int NewsId2 { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

}
