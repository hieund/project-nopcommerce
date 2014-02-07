namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a related product
    /// </summary>
    public partial class NewsViewTracking : BaseEntity
    {
        /// <summary>
        /// Gets or sets the newsitem identifier
        /// </summary>
        public int NewsItemId { get; set; }

        /// <summary>
        /// Gets or sets the tracking date
        /// </summary>
        public System.DateTime TrackingDate { get; set; }

        /// <summary>
        /// Gets or sets the View count
        /// </summary>
        public int ViewCount { get; set; }
    }

}