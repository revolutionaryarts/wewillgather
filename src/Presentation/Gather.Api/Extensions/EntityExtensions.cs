using System.Linq;
using Gather.Api.Models;
using Gather.Core;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;

namespace Gather.Api.Extensions
{
    public static class EntityExtensions
    {

        public static CommentModel ToModel(this Comment entity)
        {
            if (entity == null)
                return null;

            var model = new CommentModel
            {
                Author = entity.Author.ToModel(),
                Comment = entity.UserComment,
                Responses = entity.Responses.Select(ToModel).ToList()
            };

            return model;
        }

        public static LocationModel ToModel(this Location entity)
        {
            if (entity == null)
                return null;

            var model = new LocationModel
            {
                Id = entity.Id,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Name = entity.Name,
                ParentId = entity.ParentLocation != null ? entity.ParentLocation.Id : 0
            };

            return model;
        }

        public static ProjectModel ToModel(this Project entity)
        {
            if (entity == null)
                return null;

            var model = new ProjectModel
            {
                Categories = entity.Categories.Select(x => x.Name).ToList(),
                ChildFriendly = entity.ChildFriendly,
                Comments = entity.Comments.Select(ToModel).ToList(),
                EmailAddress = entity.EmailAddress,
                EmailDisclosureLevel = entity.EmailDisclosureLevel.GetDescription(),
                EndDate = entity.EndDate,
                Equipment = entity.Equipment ?? "",
                GettingThere = entity.GettingThere ?? "",
                IsRecurring = entity.IsRecurring,
                Latitude = entity.Latitude,
                Locations = entity.Locations.Select(x => x.Location.ToModel()).ToList(),
                Longitude = entity.Longitude,
                Name = entity.Name,
                NumberOfVolunteers = entity.NumberOfVolunteers,
                Objective = entity.Objective ?? "",
                Owners = entity.Owners.Select(ToModel).ToList(),
                Skills = entity.Skills ?? "",
                StartDate = entity.StartDate,
                Status = entity.Status.GetDescription(),
                Telephone = entity.Telephone ?? "",
                TelephoneDisclosureLevel = entity.TelephoneDisclosureLevel.GetDescription(),
                VolunteerBenefits = entity.VolunteerBenefits ?? "",
                Volunteers = entity.Volunteers.Select(ToModel).ToList(),
                Website = entity.Website ?? "",
                WebsiteDisclosureLevel = entity.WebsiteDisclosureLevel.GetDescription()
            };

            return model;
        }

        public static UserModel ToModel(this User entity)
        {
            if (entity == null)
                return null;

            var model = new UserModel
            {
                DisplayName = entity.DisplayName,
                Email = entity.Email ?? "",
                EmailDisclosureLevel = entity.EmailDisclosureLevel.GetDescription(),
                Telephone = entity.Telephone ?? "",
                TelephoneDisclosureLevel = entity.TelephoneDisclosureLevel.GetDescription(),
                Website = entity.Website ?? "",
                WebsiteDisclosureLevel = entity.WebsiteDisclosureLevel.GetDescription()                
            };

            return model;
        }

    }
}