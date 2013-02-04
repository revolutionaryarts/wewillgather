using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gather.Api.Models
{
    [Serializable]
    [DataContract]
    public class ProjectModel
    {
        public ProjectModel()
        {
            Categories = new List<string>();
            Comments = new List<CommentModel>();
            Locations = new List<LocationModel>();
            Owners = new List<UserModel>();
            Volunteers = new List<UserModel>();
        }

        [DataMember]
        public ICollection<string> Categories { get; set; }

        [DataMember]
        public virtual bool ChildFriendly { get; set; }

        [DataMember]
        public ICollection<CommentModel> Comments { get; set; }

        [DataMember]
        public virtual string EmailAddress { get; set; }

        [DataMember]
        public virtual string EmailDisclosureLevel { get; set; }

        [DataMember]
        public virtual DateTime? EndDate { get; set; }

        [DataMember]
        public virtual string Equipment { get; set; }

        [DataMember]
        public virtual string GettingThere { get; set; }

        [DataMember]
        public virtual bool IsRecurring { get; set; }

        [DataMember]
        public virtual decimal Latitude { get; set; }

        [DataMember]
        public virtual decimal Longitude { get; set; }

        [DataMember]
        public ICollection<LocationModel> Locations { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual int NumberOfVolunteers { get; set; }

        [DataMember]
        public virtual string Objective { get; set; }

        [DataMember]
        public ICollection<UserModel> Owners { get; set; }

        [DataMember]
        public virtual string Skills { get; set; }

        [DataMember]
        public virtual DateTime? StartDate { get; set; }

        [DataMember]
        public virtual string Status { get; set; }

        [DataMember]
        public virtual string Telephone { get; set; }

        [DataMember]
        public virtual string TelephoneDisclosureLevel { get; set; }

        [DataMember]
        public virtual string VolunteerBenefits { get; set; }

        [DataMember]
        public ICollection<UserModel> Volunteers { get; set; }

        [DataMember]
        public virtual string Website { get; set; }

        [DataMember]
        public virtual string WebsiteDisclosureLevel { get; set; }
    }
}