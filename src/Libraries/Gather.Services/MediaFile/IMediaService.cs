using Gather.Core.Domain.MediaFile;
using Gather.Core.Domain.Common;
using System.Collections.Generic;

namespace Gather.Services.MediaFile
{
    public interface IMediaService
    {
        /// <summary>
        /// Deletes a media file
        /// </summary>
        /// <param name="media"></param>
        void DeleteMedia(Media media);

        /// <summary>
        /// Gets the media based of the entity and entity id
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityTypeId"></param>
        /// <returns></returns>
        IList<Media> GetAllMediaByEntityId(EntityType entityType, int entityTypeId);

        /// <summary>
        /// Get a media file by id
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        Media GetMediaById(int mediaId);

        /// <summary>
        /// Inserts a media file
        /// </summary>
        /// <param name="media"></param>
        void InsertMedia(Media media);

    }
}
