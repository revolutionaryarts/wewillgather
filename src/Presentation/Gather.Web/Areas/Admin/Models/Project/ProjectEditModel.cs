using System;
using Gather.Web.Models.Project;

namespace Gather.Web.Areas.Admin.Models.Project
{    

    public class ProjectEditModel
    {

        public ProjectEditModel()
        {
            NotifyVolunteers = true;
        }
            
        public virtual int Id { get; set; }
        public ProjectModel Project { get; set; }
        public ProjectModel ParentProject { get; set; }
        public Boolean NotifyVolunteers { get; set; }
        public String AuthorMessage { get; set; }
        public String VolunteersMessage { get; set; }
    }
}