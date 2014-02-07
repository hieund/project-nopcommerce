using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Media;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Services.News
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class NewsExtensions
    {
        /// <summary>
        /// Get a default picture of a news 
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="pictureService">Picture service</param>
        /// <returns>News picture</returns>
        public static Picture GetDefaultNewsPicture(this NewsItem source, IPictureService pictureService)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (pictureService == null)
                throw new ArgumentNullException("pictureService");

            var picture = pictureService.GetPicturesByNewsId(source.Id, 1).FirstOrDefault();
            return picture;
        }
    }
}
