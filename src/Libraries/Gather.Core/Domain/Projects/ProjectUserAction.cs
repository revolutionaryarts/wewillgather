using System.ComponentModel;

namespace Gather.Core.Domain.Projects
{
    public enum ProjectUserAction
    {
        [Description("Added")]
        Added = 10,

        [Description("Removed")]
        Removed = 20
    }
}