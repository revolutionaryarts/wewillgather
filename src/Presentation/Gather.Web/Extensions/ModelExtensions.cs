using System.Linq;
using Gather.Core;
using Gather.Core.Domain.Api;
using Gather.Core.Domain.Categories;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Pages;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.SuccessStories;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Services.Security;
using Gather.Web.Models.Api;
using Gather.Web.Models.Category;
using Gather.Web.Models.Comment;
using Gather.Web.Models.Location;
using Gather.Web.Models.Page;
using Gather.Web.Models.Project;
using Gather.Web.Models.SuccessStory;
using Gather.Web.Models.User;

namespace Gather.Web.Extensions
{
    public static class ModelExtensions
    {
        public static ApiAuthentication ToEntity(this ApiAuthenticationModel model)
        {
            if (model == null)
                return null;

            var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();

            var entity = new ApiAuthentication
            {
                Active = model.Active,
                CreatedDate = model.CreatedDate,
                Deleted = model.Deleted,
                Id = model.Id,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                NameOfApplication = model.NameOfApplication,
                ApiUser = model.ApiUser.ToEntity(),
                Description = model.Description,
                SecretKey = encryptionService.CreateSaltKey(6),
                WebsiteAddress = model.WebsiteAddress
            };

            return entity;
        }

        public static Category ToEntity(this CategoryModel model)
        {
            if (model == null)
                return null;

            var entity = new Category
            {
                Active = model.Active,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                Deleted = model.Deleted,
                Id = model.Id,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                Name = model.Name
            };

            return entity;
        }

        public static Comment ToEntity(this CommentModel model)
        {
            if (model == null)
                return null;

            var entity = new Comment
            {
                Active = model.Active,
                Author = model.Author.ToEntity(),
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                Deleted = model.Deleted,
                Id = model.Id,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                ModeratedBy = model.LastModifiedBy,
                ModeratedDate = model.LastModifiedDate,
                ModerationRequestCount = model.ModerationRequestCount,
                Project = model.Project.ToEntity(),
                UserComment = model.UserComment
            };

            return entity;
        }

        public static Location ToEntity(this LocationModel model)
        {
            if (model == null)
                return null;

            var entity = new Location
            {
                Active = model.Active,
                CreatedDate = model.CreatedDate,
                Deleted = model.Deleted,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                HashTag = model.HashTag,
                Id = model.Id,
                ParentLocation = model.ParentLocation.ToEntity(),
                Name = model.Name
            };

            return entity;
        }

        public static Page ToEntity(this PageModel model)
        {
            if (model == null)
                return null;

            var entity = new Page
            {
                Content = model.Content,
                Title = model.Title,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                MetaTitle = model.MetaTitle,
                Priority = model.Priority
            };

            return entity;
        }

        public static Project ToEntity(this ProjectModel model)
        {
            if (model == null)
                return null;

            var entity = new Project
            {
                Categories = model.Categories.Select(ToEntity).ToList(),
                ChildFriendly = model.ChildFriendly,
                CreatedBy = model.CreatedById,
                CreatedDate = model.CreatedDate,
                EmailAddress = model.EmailAddress.StripHtml(),
                EmailDisclosureId = model.EmailDisclosureId,
                EndDate = model.EndDate,
                Equipment = model.Equipment.StripHtml(),
                GettingThere = model.GettingThere.StripHtml(),
                Id = model.Id,
                IsRecurring = model.IsRecurring,
                LastModeratorApprovalBy = model.LastModeratorApprovalBy,
                LastModeratorApprovalDate = model.LastModeratorApprovalDate,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                Latitude = model.Latitude,
                Locations = model.Locations.Select(ToEntity).ToList(),
                Longitude = model.Longitude,
                Name = model.Name.StripHtml(),
                NumberOfVolunteers = model.NumberOfVolunteers,
                Objective = model.Objective.StripHtml(),
                Owners = model.Owners.Select(ToEntity).ToList(),
                Recurrence = model.Recurrence,
                RecurrenceInterval = model.RecurrenceInterval,
                RecurrenceIntervalId = model.RecurrenceIntervalId,
                Skills = model.Skills.StripHtml(),
                StartDate = model.StartDate,
                Status = model.Status,
                StatusId = model.StatusId,
                Telephone = model.Telephone.StripHtml(),
                TelephoneDisclosureId = model.TelephoneDisclosureId,
                Volunteers = model.Volunteers.Select(ToEntity).ToList(),
                VolunteerBenefits = model.VolunteerBenefits.StripHtml(),
                Website = model.Website.StripHtml(),
                WebsiteDisclosureId = model.WebsiteDisclosureId
            };

            return entity;
        }

        public static ProjectLocation ToEntity(this ProjectLocationModel model)
        {
            if (model == null)
                return null;

            var entity = new ProjectLocation
            {
                Location = model.Location.ToEntity(),
                LocationId = model.LocationId,
                Primary = model.Primary,
                Project = model.Project.ToEntity(),
                ProjectId = model.ProjectId
            };

            return entity;
        }

        public static Project ToEntity(this ProjectModel model, Project entity)
        {
            if (model == null || entity == null)
                return null;

            entity.ChildFriendly = model.ChildFriendly;
            entity.CreatedBy = model.CreatedById;
            entity.EmailAddress = model.EmailAddress.StripHtml();
            entity.EmailDisclosureId = model.EmailDisclosureId;
            entity.EndDate = model.EndDate;
            entity.Equipment = model.Equipment.StripHtml();
            entity.GettingThere = model.GettingThere.StripHtml();
            entity.Id = model.Id;
            entity.IsRecurring = model.IsRecurring;
            entity.LastModeratorApprovalBy = model.LastModeratorApprovalBy;
            entity.LastModeratorApprovalDate = model.LastModeratorApprovalDate;
            entity.LastModifiedBy = model.LastModifiedBy;
            entity.LastModifiedDate = model.LastModifiedDate;
            entity.Latitude = model.Latitude;
            entity.Longitude = model.Longitude;
            entity.Name = model.Name.StripHtml();
            entity.NumberOfVolunteers = model.NumberOfVolunteers;
            entity.Objective = model.Objective.StripHtml();
            entity.Recurrence = model.Recurrence;
            entity.RecurrenceInterval = model.RecurrenceInterval;
            entity.RecurrenceIntervalId = model.RecurrenceIntervalId;
            entity.Skills = model.Skills.StripHtml();
            entity.StartDate = model.StartDate;
            entity.Status = model.Status;
            entity.StatusId = model.StatusId;
            entity.Telephone = model.Telephone.StripHtml();
            entity.TelephoneDisclosureId = model.TelephoneDisclosureId;
            entity.VolunteerBenefits = model.VolunteerBenefits.StripHtml();
            entity.Website = model.Website.StripHtml();
            entity.WebsiteDisclosureId = model.WebsiteDisclosureId;

            return entity;
        }

        public static SuccessStory ToEntity(this SuccessStoryModel model)
        {
            if (model == null)
                return null;

            var entity = new SuccessStory
            {
                Title = model.Title,
                ShortSummary = model.ShortSummary,
                Article = model.Article,
                MetaTitle = model.MetaTitle,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
            };

            return entity;
        }

        public static User ToEntity(this UserModel model)
        {
            if (model == null)
                return null;

            var entity = new User
            {
                Active = model.Active,
                ContactUsBio = model.ContactUsBio,
                CreatedDate = model.CreatedDate,
                DisplayName = model.DisplayName,
                Email = model.Email,
                EmailDisclosureId = model.EmailDisclosureId,
                EmailDisclosureLevel = model.EmailDisclosureLevel,
                FacebookProfile = model.FacebookProfile,
                Id = model.Id,
                LastLoginDate = model.LastLoginDate,
                ShowOnContactUs = model.ShowOnContactUs,
                Telephone = model.Telephone,
                TelephoneDisclosureId = model.TelephoneDisclosureId,
                TelephoneDisclosureLevel = model.TelephoneDisclosureLevel,
                TwitterProfile = model.TwitterProfile,
                UserName = model.UserName,
                UserRoles = model.UserRoles.Select(ToEntity).ToList(),
                Website = model.Website,
                WebsiteDisclosureId = model.WebsiteDisclosureId,
                WebsiteDisclosureLevel = model.WebsiteDisclosureLevel
            };

            return entity;
        }

        public static UserRole ToEntity(this UserRoleModel model)
        {
            if (model == null)
                return null;

            var entity = new UserRole
            {
                Name = model.Name,
                SystemName = model.SystemName
            };

            return entity;
        }
    }
}