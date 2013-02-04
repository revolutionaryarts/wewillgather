using System;
using System.Collections.Generic;

namespace Gather.Core.Domain.Locations
{
    public class Location : BaseEntity
    {
        private ICollection<Location> _childLocations;

        /// <summary>
        /// Gets or sets the Child locations
        /// </summary>
        public virtual ICollection<Location> ChildLocations
        {
            get { return _childLocations ?? (_childLocations = new List<Location>()); }
            set { _childLocations = value; }
        }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the location hashtag
        /// </summary>
        public virtual string HashTag { get; set; }

        /// <summary>
        /// Gets or sets the region flag
        /// </summary>
        public virtual bool IsRegion { get; set; }
    
        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the project latitude
        /// </summary>
        public virtual decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the project longitude
        /// </summary>
        public virtual decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the location Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent location
        /// </summary>
        public virtual Location ParentLocation { get; set; }

        /// <summary>
        /// Gets or sets the location seo name
        /// </summary>
        public virtual string SeoName { get; set; }
    }

    public class LocationNameState
    {
        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets if the location is a state
        /// </summary>
        public bool State { get; set; }
    }
}