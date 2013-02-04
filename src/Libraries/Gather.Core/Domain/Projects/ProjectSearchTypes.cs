using System.ComponentModel;

namespace Gather.Core.Domain.Projects
{
    public enum ProjectChildFriendly
    {
        [Description("Doesn't matter")]
        NoPreference = 10,

        [Description("Yes, kids can help")]
        Friendly = 15,

        [Description("Sorry, no kids!")]
        UnFriendly = 20
    }

    public enum ProjectSearchRadius
    {
        [Description("5 miles")]
        FiveMiles = 5,

        [Description("10 miles")]
        TenMiles = 10,

        [Description("15 miles")]
        FifteenMiles = 15,

        [Description("50 miles")]
        FiftyMiles = 18,

        [Description("Any distance")]
        AnyDistance = 20
    }

    public enum ProjectSearchStart
    {
        [Description("Whenever")]
        Whenever = 10,

        [Description("Today or tomorrow")]
        TodayOrTomorrow = 15,

        [Description("This week")]
        ThisWeek = 20,

        [Description("This month")]
        ThisMonth = 25
    }
}