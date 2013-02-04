using System;
using System.Runtime.Serialization;

namespace Gather.Api.Models
{
    [Serializable]
    [DataContract]
    public class UserModel
    {
        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string EmailDisclosureLevel { get; set; }

        [DataMember]
        public string Telephone { get; set; }

        [DataMember]
        public string TelephoneDisclosureLevel { get; set; }

        [DataMember]
        public string Website { get; set; }

        [DataMember]
        public string WebsiteDisclosureLevel { get; set; }
    }
}