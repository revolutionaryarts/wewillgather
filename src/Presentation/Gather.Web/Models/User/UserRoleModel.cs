using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Security;

namespace Gather.Web.Models.User
{
    public class UserRoleModel : BaseModel
    {
        public UserRoleModel()
        {
            AvailablePermissionRecords = new List<PermissionRecordModel>();
            PermissionRecords = new List<PermissionRecordModel>();
        }

        public bool Active { get; set; }

        public IList<PermissionRecordModel> AvailablePermissionRecords { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsSiteOwnerRole { get; set; }

        [Required]
        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [Required]
        public string Name { get; set; }

        public IList<PermissionRecordModel> PermissionRecords { get; set; } 

        [Required]
        [Display(Name = "System Name")]
        public string SystemName { get; set; }
    }
}