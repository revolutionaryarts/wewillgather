using System;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.MediaFile;
using Gather.Web.Areas.Admin.Models;

namespace Gather.Web.Models.Media
{
    public class MediaModel : BaseModel
    {
        public int EntityId { get; set; }

        public EntityType EntityType { get; set; }

        public int EntityTypeId { get; set; }

        public string FileName { get; set; }

        public FileType FileType { get; set; }

        public int FileTypeId { get; set; }

        public string Link { get; set; }

        public string Name { get; set; }

        public DateTime UploadedDate { get; set; }

        public int UploadedById { get; set; }
    }
}