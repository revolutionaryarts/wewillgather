using System.ComponentModel;

namespace Gather.Core.Domain.Projects
{
    public enum ProjectSortType
    {
        [Description("Start Date")]
        StartDate = 10,

        [Description("Created Date")]
        CreatedDate = 20,

        [Description("Good People")]
        Volunteers = 30
    }

    public enum ProjectSortDirection
    {
        Ascending = 10,
        Descending = 20
    }
}