using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum MapProvider
    {
        [Description("OpenStreetMap")]
        OpenStreetMaps = 10,

        [Description("Microsoft Bing Maps")]
        BingMaps = 20
    }
}