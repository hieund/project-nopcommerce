using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsItem : BaseEntity, ISlugSupported, IStoreMappingSupported
    {
        private ICollection<NewsComment> _newsComments;
        private ICollection<NewsPicture> _newsPictures;
        private ICollection<NewsItemCategory> _newsItemCategories;

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the news title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the short text
        /// </summary>
        public string Short { get; set; }

        /// <summary>
        /// Gets or sets the full text
        /// </summary>
        public string Full { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news item is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the news item start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the news item end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news post comments are allowed 
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets the total number of comments
        /// <remarks>
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the news comments
        /// </summary>
        public virtual ICollection<NewsComment> NewsComments
        {
            get { return _newsComments ?? (_newsComments = new List<NewsComment>()); }
            protected set { _newsComments = value; }
        }

        /// <summary>
        /// Gets or sets the collection of NewsPicture
        /// </summary>
        public virtual ICollection<NewsPicture> NewsPictures
        {
            get { return _newsPictures ?? (_newsPictures = new List<NewsPicture>()); }
            protected set { _newsPictures = value; }
        }

        /// <summary>
        /// Gets or sets the collection of NewsItemCategory
        /// </summary>
        public virtual ICollection<NewsItemCategory> NewsItemCategories
        {
            get { return _newsItemCategories ?? (_newsItemCategories = new List<NewsItemCategory>()); }
            protected set { _newsItemCategories = value; }
        }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the news item is featured
        /// </summary>
        public bool Featured { get; set; }

        /// <summary>
        /// Gets or sets the news picture
        /// </summary>
        public int PictureId { get; set; }
    }
}