using System;
using Gather.Core.Domain.Common;

namespace Gather.Core.Domain.MediaFile
{
    public class Media : BaseEntity
    {
        /// <summary>
        /// The id of the item it links to in the table
        /// </summary>
        public virtual int EntityId { get; set; }

        /// <summary>
        /// The Entity type
        /// </summary>
        public virtual EntityType EntityType
        {
            get { return (EntityType)EntityTypeId; }
            set { EntityTypeId = (int)value; }
        }

        /// <summary>
        /// The Entity type id for the database
        /// </summary>
        public virtual int EntityTypeId { get; set; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public virtual string FileName { get; set; }

        /// <summary>
        /// The type of file
        /// </summary>
        public virtual FileType FileType
        {
            get { return (FileType)FileTypeId; }
            set { FileTypeId = (int)value; }
        }

        /// <summary>
        /// The type of file identifier
        /// </summary>
        public virtual int FileTypeId { get; set; }

        /// <summary>
        /// The link assigned to the image
        /// </summary>
        public virtual string Link { get; set; }

        /// <summary>
        /// The user friendly name for the file 
        /// </summary>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// The date the media was uploaded
        /// </summary>
        public virtual DateTime UploadedDate { get; set; }

        /// <summary>
        /// The id of the user that uploaded the media
        /// </summary>
        public virtual int UploadedById { get; set; }
    }
}