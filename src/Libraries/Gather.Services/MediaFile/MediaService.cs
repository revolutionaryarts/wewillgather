using System;
using System.IO;
using System.Linq;
using Gather.Core;
using Gather.Core.Domain.MediaFile;
using Gather.Core.Data;
using System.Collections.Generic;
using Gather.Core.Domain.Common;

namespace Gather.Services.MediaFile
{
    public class MediaService : IMediaService
    {

        #region Fields

        private readonly IRepository<Media> _mediaRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public MediaService(IRepository<Media> mediaRepository, IWorkContext workContext)
        {
            _mediaRepository = mediaRepository;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Permanently delete a a media file
        /// </summary>
        /// <param name="media">Media</param>
        public void DeleteMedia(Media media)
        {
            if (media == null)
                throw new ArgumentNullException("media");

            string file = System.Web.HttpContext.Current.Server.MapPath("/Uploads/Media/" + media.FileName);
            if(File.Exists(file))
                File.Delete(file);

            _mediaRepository.Delete(media, true);
        }

        /// <summary>
        /// Gets all the media based of the entity and the id
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityTypeId"></param>
        /// <returns>Returns a list of media</returns>
        public IList<Media> GetAllMediaByEntityId(EntityType entityType, int entityTypeId)
        {
            var query = _mediaRepository.Table;

            query = query.Where(s => s.EntityId == entityTypeId);
            query = query.Where(s => s.EntityTypeId == (int)entityType);

            query = query.OrderBy(s => s.Id);

            var pages = query.ToList();
            return pages;
        }

        /// <summary>
        /// Get a media file by id
        /// </summary>
        /// <param name="mediaId">Id of media to retrieve</param>
        /// <returns>Media</returns>
        public Media GetMediaById(int mediaId)
        {
            if (mediaId == 0)
                return null;
            
            var media = _mediaRepository.GetById(mediaId);
            return media;
        }

        /// <summary>
        /// Insert a media file
        /// </summary>
        /// <param name="media">Media File</param>
        public void InsertMedia(Media media)
        {
            if (media == null)
                throw new ArgumentNullException("media");

            if ((Path.GetExtension(media.FileName) ?? "").ToLower() == ".pdf")
                media.FileType = FileType.PDF;
            else
                media.FileType = FileType.Image;
            
            media.UploadedById = _workContext.CurrentUser.Id;
            media.UploadedDate = DateTime.Now;

            _mediaRepository.Insert(media);
        }

        #endregion

    }
}
