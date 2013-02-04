using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gather.Api.Models
{
    [DataContract]
    public class CommentModel
    {
        public CommentModel()
        {
            Responses = new List<CommentModel>();
        }

        [DataMember]
        public virtual UserModel Author { get; set; }

        [DataMember]
        public virtual string Comment { get; set; }

        [DataMember]
        public ICollection<CommentModel> Responses { get; set; }
    }
}