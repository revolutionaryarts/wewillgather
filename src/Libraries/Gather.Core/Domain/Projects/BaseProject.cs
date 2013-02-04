using System;
using System.Collections.Generic;
using Gather.Core.Domain.Categories;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Projects
{
    public class BaseProject : BaseEntity
    {
        private ICollection<Category> _categories;
        private ICollection<User> _owners;
        private ICollection<ProjectLocation> _locations; 
        private ICollection<User> _volunteers;

        /// <summary>
        /// Gets or sets the categories
        /// </summary>
        public virtual ICollection<Category> Categories
        {
            get { return _categories ?? (_categories = new List<Category>()); }
            set { _categories = value; }
        }

        /// <summary>
        /// Gets or sets the child-friendly flag
        /// </summary>
        public virtual bool ChildFriendly { get; set; }

        /// <summary>
        /// Gets or sets the id of the user that created the project
        /// </summary>
        public virtual int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the project creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the projects end date
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Equipment
        /// </summary>
        public virtual string Equipment { get; set; }

        /// <summary>
        /// Gets or sets the Getting there value
        /// </summary>
        public virtual string GettingThere { get; set; }

        /// <summary>
        /// Gets or sets the project latitude
        /// </summary>
        public virtual decimal Latitude { get; set; }
        
        /// <summary>
        /// Gets or sets the project associated locations
        /// </summary>
        public virtual ICollection<ProjectLocation> Locations
        {
            get { return _locations ?? (_locations = new List<ProjectLocation>()); }
            set { _locations = value; }
        }

        /// <summary>
        /// Gets or sets the project longitude
        /// </summary>
        public virtual decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the projects name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Number of volunteers
        /// </summary>
        public virtual int NumberOfVolunteers { get; set; }

        /// <summary>
        /// Gets or sets the objective
        /// </summary>
        public virtual string Objective { get; set; }

        /// <summary>
        /// Gets or sets the owners of the project
        /// </summary>
        public virtual ICollection<User> Owners
        {
            get { return _owners ?? (_owners = new List<User>()); }
            set { _owners = value; }
        }

        /// <summary>
        /// Gets or sets the number of remaining volunteers
        /// </summary>
        public virtual int RemainingNumberOfVolunteers { get; set; }

        /// <summary>
        /// Gets or sets the skills required
        /// </summary>
        public virtual string Skills { get; set; }

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the projects volunteer benefits
        /// </summary>
        public virtual string VolunteerBenefits { get; set; }

        /// <summary>
        /// Gets or sets the list of volunteers
        /// </summary>
        public virtual ICollection<User> Volunteers
        {
            get { return _volunteers ?? (_volunteers = new List<User>()); }
            set { _volunteers = value; }
        }
    }
}