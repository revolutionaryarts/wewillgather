using System.ComponentModel.DataAnnotations;

namespace Gather.Web.Models.Install
{
    public class StepTwoModel
    {
        [Required, Display(Name = "Facebook App ID")]
        public string FacebookAppId { get; set; }

        [Required, Display(Name = "Facebook App Secret")]
        public string FacebookAppSecret { get; set; }

        [Required, Display(Name = "Twitter Consumer Key")]
        public string TwitterConsumerKey { get; set; }

        [Required, Display(Name = "Twitter Consumer Secret")]
        public string TwitterConsumerSecret { get; set; }
    }
}