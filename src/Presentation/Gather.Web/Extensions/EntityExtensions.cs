using System;
using System.Linq;
using Gather.Core;
using Gather.Core.Domain.Api;
using Gather.Core.Domain.Categories;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.MediaFile;
using Gather.Core.Domain.Messages;
using Gather.Core.Domain.ModerationQueue;
using Gather.Core.Domain.Pages;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Security;
using Gather.Core.Domain.Settings;
using Gather.Core.Domain.SuccessStories;
using Gather.Core.Domain.Tweets;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Core.Seo;
using Gather.Services.MediaFile;
using Gather.Services.Security;
using Gather.Services.Users;
using Gather.Web.Areas.Admin.Models.Project;
using Gather.Web.Models.Api;
using Gather.Web.Models.Comment;
using Gather.Web.Models.Media;
using Gather.Web.Models.MessageQueues;
using Gather.Web.Models.ModerationQueue;
using Gather.Web.Models.Project;
using Gather.Web.Models.Security;
using Gather.Web.Models.Setting;
using Gather.Web.Models.User;
using Gather.Web.Models.Category;
using Gather.Web.Models.Location;
using Gather.Web.Models.SuccessStory;
using Gather.Web.Models.Page;
using Gather.Web.Models.Tweet;

namespace Gather.Web.Extensions
{
    public static class EntityExtensions
    {

        public static LocationFilterModel ToFilterModel(this Location entity)
        {
            if (entity == null)
                return null;

            var model = new LocationFilterModel
            {
                Id = entity.Id,
                Name = entity.Name              
            };

            return model;
        }

        public static BaseProjectModel ToModel(this BaseProject entity)
        {
            if (entity == null)
                return null;

            var model = new BaseProjectModel
            {
                Categories = entity.Categories.Select(ToModel).ToList(),
                CreatedById = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                EndDate = entity.EndDate,
                Id = entity.Id,
                Latitude = entity.Latitude,
                Locations = entity.Locations.Select(ToModel).ToList(),
                Longitude = entity.Longitude,
                Name = entity.Name,
                NumberOfVolunteers = entity.NumberOfVolunteers,
                Objective = entity.Objective,
                Owners = entity.Owners.Select(ToModel).ToList(),
                RemainingNumberOfVolunteers = entity.RemainingNumberOfVolunteers,
                SeoName = entity.GetSeoName(),
                StartDate = entity.StartDate,
                Volunteers = entity.Volunteers.Select(ToModel).ToList()
            };

            return model;
        }

        public static ApiAuthenticationModel ToModel(this ApiAuthentication entity)
        {
            if (entity == null)
                return null;

            var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
            var model = new ApiAuthenticationModel
            {
                Active = entity.Active,
                CreatedDate = entity.CreatedDate,
                Deleted = entity.Deleted,
                Id = entity.Id,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedDate = entity.LastModifiedDate,
                NameOfApplication = entity.NameOfApplication,
                ApiUser = entity.ApiUser.ToModel(),
                Description = entity.Description,
                SecretKey = entity.SecretKey,
                WebsiteAddress = entity.WebsiteAddress,
                AccessToken = encryptionService.EncryptText(entity.SecretKey + entity.WebsiteAddress)
            };

            return model;
        }

        public static CategoryModel ToModel(this Category entity)
        {
            if (entity == null)
                return null;

            var model = new CategoryModel
            {
                Active = entity.Active,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                Deleted = entity.Deleted,
                Id = entity.Id,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name
            };

            return model;
        }

        public static CommentModel ToModel(this Comment entity)
        {
            if (entity == null)
                return null;

            var model = new CommentModel
            {
                Active = entity.Active,
                Author = entity.Author.ToModel(),
                CreatedDate = entity.CreatedDate,
                Deleted = entity.Deleted,
                Id = entity.Id,
                LastModifiedDate = entity.LastModifiedDate,
                ModerationRequestCount = entity.ModerationRequestCount,
                Responses = entity.Responses.Select(ToModel).OrderBy(x => x.CreatedDate).ToList(),
                UserComment = entity.UserComment,               
            };

            return model;
        }

        public static LocationModel ToModel(this Location entity)
        {
            if (entity == null)
                return null;

            var model = new LocationModel
            {
                Active = entity.Active,
                CreatedDate = entity.CreatedDate,
                Deleted = entity.Deleted,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedDate = entity.LastModifiedDate,
                HashTag = entity.HashTag,
                Id = entity.Id,
                Name = entity.Name,
                ParentLocation = entity.ParentLocation.ToModel(),
                SeoName = entity.GetSeoName()
            };

            return model;
        }

        public static MediaModel ToModel(this Media entity)
        {
            if (entity == null)
                return null;

            var model = new MediaModel
            {
                EntityId = entity.EntityId,
                EntityType = entity.EntityType,
                EntityTypeId = entity.EntityTypeId,
                FileName = entity.FileName,
                FileType = entity.FileType,
                FileTypeId = entity.FileTypeId,
                Id = entity.Id,
                Link = entity.Link,
                Name = entity.Name,
                UploadedById = entity.UploadedById,
                UploadedDate = entity.UploadedDate
            };

            return model;
        }

        public static PageModel ToModel(this Page entity)
        {
            if (entity == null)
                return null;

            var mediaService = EngineContext.Current.Resolve<IMediaService>();

            var model = new PageModel
            {
                Active = entity.Active,
                Content = entity.Content.EncodeEmails(),
                CreatedDate = entity.CreatedDate,
                Deleted = entity.Deleted,
                FileTitle = entity.FileTitle,
                Id = entity.Id,
                IsSystemPage = entity.IsSystemPage,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedDate = entity.LastModifiedDate,
                Media = mediaService.GetAllMediaByEntityId(EntityType.Page, entity.Id).Select(x => x.ToModel()).ToList(),
                MetaDescription = entity.MetaDescription,
                MetaKeywords = entity.MetaKeywords,
                MetaTitle = entity.MetaTitle,
                Priority = entity.Priority,
                Title = entity.Title
            };

            return model;
        }

        public static PermissionRecordModel ToModel(this PermissionRecord entity)
        {
            if (entity == null)
                return null;

            var model = new PermissionRecordModel
            {
                Name = entity.Name,
                SystemName = entity.SystemName                
            };

            return model;
        }

        public static ProjectModel ToModel(this Project entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectModel
            {
                Categories = entity.Categories.Select(ToModel).ToList(),
                ChildFriendly = entity.ChildFriendly,
                CreatedById = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                EmailAddress = entity.EmailAddress,
                EmailDisclosureId = entity.EmailDisclosureId,
                EmailDisclosureLevel = entity.EmailDisclosureLevel,
                EndDate = entity.EndDate,
                Equipment = entity.Equipment,
                GettingThere = entity.GettingThere,
                Id = entity.Id,
                IsRecurring = entity.IsRecurring,
                LastModeratorApprovalBy = entity.LastModeratorApprovalBy,
                LastModeratorApprovalDate = entity.LastModeratorApprovalDate,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedDate = entity.LastModifiedDate,
                Latitude = entity.Latitude,
                Locations = entity.Locations.Select(ToModel).ToList(),
                Longitude = entity.Longitude,
                Name = entity.Name,
                NumberOfVolunteers = entity.NumberOfVolunteers,
                Objective = entity.Objective,
                Owners = entity.Owners.Select(ToModel).ToList(),
                Recurrence = entity.Recurrence,
                RecurrenceIntervalId = entity.RecurrenceIntervalId,
                RemainingNumberOfVolunteers = entity.NumberOfVolunteers - entity.Volunteers.Count,
                SeoName = entity.GetSeoName(),
                Skills = entity.Skills,
                StartDate = entity.StartDate,
                Status = entity.Status,
                StatusId = entity.StatusId,
                Telephone = entity.Telephone,
                TelephoneDisclosureId = entity.TelephoneDisclosureId,
                TelephoneDisclosureLevel = entity.TelephoneDisclosureLevel,
                Volunteers = entity.Volunteers.Select(ToModel).ToList(),
                VolunteerBenefits = entity.VolunteerBenefits,
                Website = entity.Website,
                WebsiteDisclosureId = entity.WebsiteDisclosureId,
                WebsiteDisclosureLevel = entity.WebsiteDisclosureLevel,
                ModerationNotes = entity.ModeratorNotes
            };
       
            return model;
        }

        public static ProjectLocationModel ToModel(this ProjectLocation entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectLocationModel
            {
                Location = entity.Location.ToModel(),
                LocationId = entity.LocationId,
                Primary = entity.Primary
            };

            return model;
        }

        public static SettingModel ToModel(this Setting entity)
        {
            if (entity == null)
                return null;

            var model = new SettingModel
            {
                Name = entity.Name,
                Value = entity.Value
            };

            return model;
        }

        public static SuccessStoryModel ToModel(this SuccessStory entity)
        {
            if (entity == null)
                return null;

            var mediaService = EngineContext.Current.Resolve<IMediaService>();

            var model = new SuccessStoryModel
            {
                Active = entity.Active,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                Deleted = entity.Deleted,
                Id = entity.Id,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedDate = entity.LastModifiedDate,
                Title = entity.Title,
                Author = entity.Author.ToModel(),
                AuthorId = entity.Author.Id,
                ShortSummary = entity.ShortSummary,
                Article = entity.Article,
                ProjectId = entity.ProjectId,
                MetaTitle = entity.MetaTitle,
                MetaDescription = entity.MetaDescription,
                MetaKeywords = entity.MetaKeywords,
                SeName = SeoExtensions.GetSeoName(entity.Title),
                Media = mediaService.GetAllMediaByEntityId(EntityType.SuccessStory, entity.Id).FirstOrDefault().ToModel()
            };

            return model;
        }

        public static TweetModel ToModel(this Tweet entity)
        {
            if (entity == null)
                return null;

            var model = new TweetModel
            {
                CreatedDate = entity.CreatedDate,
                Id = entity.Id,
                Text = entity.Text,
                TwitterId = entity.TwitterId.ToString(),
                TwitterProfile = entity.TwitterProfile,
                TwitterName = entity.TwitterName,
                UserName = entity.UserName
            };
            TimeSpan dateDifference = DateTime.Now.Subtract(entity.CreatedDate);
            if(dateDifference.Days > 0)
                model.DateDifference = entity.CreatedDate.ToString("dd MMM");
            else if(dateDifference.Hours > 0)
                model.DateDifference = dateDifference.Hours + "h";
            else if(dateDifference.Minutes > 0)
                model.DateDifference = dateDifference.Minutes + "m";
            else
                model.DateDifference = dateDifference.Seconds + "s";

            model.CreatedDateString = entity.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss");

            return model;
        }

        public static UserModel ToModel(this User entity)
        {
            if (entity == null)
                return null;

            var model = new UserModel
            {
                Active = entity.Active,
                ContactUsBio = entity.ContactUsBio,
                CreatedDate = entity.CreatedDate,
                DisplayName = entity.DisplayName,
                Email = entity.Email,
                EmailDisclosureId = entity.EmailDisclosureId,
                EmailDisclosureLevel = entity.EmailDisclosureLevel,
                FacebookDisplayName = entity.FacebookDisplayName,
                FacebookProfile = entity.FacebookProfile,
                Id = entity.Id,
                LastLoginDate = entity.LastLoginDate,
                PrimaryAuthMethod = entity.PrimaryAuthMethod,
                PrimaryAuthMethodId = entity.PrimaryAuthMethodId,
                ProfilePicture = entity.ProfilePicture,
                ShowOnContactUs = entity.ShowOnContactUs,
                Telephone = entity.Telephone,
                TelephoneDisclosureId = entity.TelephoneDisclosureId,
                TelephoneDisclosureLevel = entity.TelephoneDisclosureLevel,  
                TwitterDisplayName = entity.TwitterDisplayName,
                TwitterProfile = entity.TwitterProfile,
                UserName = entity.UserName,
                UserRoles = entity.UserRoles.Select(ToModel).ToList(),
                Website = entity.Website,
                WebsiteDisclosureId = entity.WebsiteDisclosureId,
                WebsiteDisclosureLevel = entity.WebsiteDisclosureLevel
            };

            return model;
        }

        public static UserRoleModel ToModel(this UserRole entity)
        {
            if (entity == null)
                return null;

            var model = new UserRoleModel
            {
                Active = entity.Active,
                CreatedDate = entity.CreatedDate,
                Id = entity.Id,
                IsSiteOwnerRole = (entity.SystemName == SystemUserRoleNames.SiteOwner),
                IsSystemRole = entity.IsSystemRole,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name,
                SystemName = entity.SystemName,
                PermissionRecords = entity.PermissionRecords.Select(ToModel).ToList()
            };

            return model;
        }

        public static MessageQueueModel ToModel(this MessageQueue entity)
        {
            if (entity == null)
                return null;

            var model = new MessageQueueModel
            {
                Body = entity.Body,
                Id = entity.Id,
                CreatedOn = entity.CreatedOn,
                SentOn = entity.SentOn,
                Priority = entity.Priority,
                SentTries = entity.SentTries,
                ShortBody = entity.ShortBody,
                Subject = entity.Subject,
                User = entity.User.ToModel()
            };

            return model;
        }

        #region Moderation Queue items

        public static ModerationQueueModel ToModel(this ModerationQueue entity)
        {
            if (entity == null)
                return null;

            var userService = EngineContext.Current.Resolve<IUserService>();

            var model = new ModerationQueueModel
            {
                Id = entity.Id,
                CreatedByUser = userService.GetUserById(entity.CreatedBy).ToModel(),
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                ModeratedBy = entity.ModeratedBy,
                ModeratedDate = entity.ModeratedDate,
                RequestTypeId = entity.RequestTypeId,
                RequestType = entity.RequestType,
                StatusId = entity.StatusId,
                StatusType = entity.StatusType,
                Notes = entity.Notes,
                RequestTypeDescription = entity.RequestType.GetDescription()
            };

            return model;
        }

        public static ProjectCommentModerationModel ToModel(this ProjectCommentModeration entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectCommentModerationModel
            {
                Id = entity.Id,
                Comment = entity.Comment.ToModel(),
                ComplaintId = entity.ComplaintId,
                ComplaintType = entity.ComplaintType,
                TypeDescription = entity.ComplaintType.GetDescription(),
                ModerationQueue = entity.ModerationQueue.ToModel(),
                Reason = entity.Reason
            };

            return model;
        }

        public static ProjectModerationModel ToModel(this ProjectModeration entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectModerationModel
            {
                Id = entity.Id,
                Project = entity.Project.ToModel(),
                ComplaintId = entity.ComplaintId,
                ComplaintType = entity.ComplaintType,
                ModerationQueue = entity.ModerationQueue.ToModel(),
                Reason = entity.Reason
            };

            return model;
        }

        public static ProjectModerationDisputeModel ToDisputeModel(this ProjectModeration entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectModerationDisputeModel
            {
                Id = entity.Id,
                Project = entity.Project.ToModel(),
                ComplaintId = entity.ComplaintId,
                ComplaintType = entity.ComplaintType,
                TypeDescription = entity.ComplaintType.GetDescription(),
                ModerationQueue = entity.ModerationQueue.ToModel(),
                Reason = entity.Reason
            };

            return model;
        }

        public static ProjectUserHistoryModel ToModel(this ProjectUserHistory entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectUserHistoryModel
            {
                Id = entity.Id,
                AffectedUser = entity.AffectedUser.ToModel(),
                CommittingUser = entity.CommittingUser.ToModel(),
                CreatedDate = entity.CreatedDate,
                Project = entity.Project.ToModel(),
                ProjectUserAction = entity.ProjectUserAction,
                ProjectUserActionId = entity.ProjectUserActionId
            };

            return model;
        }

        public static ProjectWithdrawalModel ToModel(this ProjectWithdrawal entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectWithdrawalModel
            {
                Id = entity.Id,
                Project = entity.Project.ToModel(),
                ModerationQueue = entity.ModerationQueue.ToModel(),        
                Reason = entity.Reason
            };

            return model;
        }
        
        #endregion

    }
}