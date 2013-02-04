using System;
using System.Runtime.Serialization;

namespace Gather.Api.Models
{
    [Serializable]
    [DataContract]
    public class LocationModel
    {
        [DataMember]
        public virtual int Id { get; set; }

        [DataMember]
        public virtual decimal Latitude { get; set; }

        [DataMember]
        public virtual decimal Longitude { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual int ParentId { get; set; }
    }
}