namespace Gather.Web.Areas.Admin.Models.Common
{
    public class DeleteConfirmationModel
    {
        public string ActionName { get; set; }
        public string AdditionalActionName { get; set; }
        public string AdditionalControllerName { get; set; }
        public string AdditionalData { get; set; }
        public bool Ajax { get; set; }
        public string ControllerName { get; set; }
        public int Id { get; set; }
        public string Page { get; set; }
        public string Search { get; set; }
    }
}