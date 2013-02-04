namespace Gather.Web.Models
{
    public class BasePageModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Hashtag { get; set; }
    }
}