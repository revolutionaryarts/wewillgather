using Gather.Core.Domain.Users;

namespace Gather.Services.Install
{
    public interface IInstallService
    {
        bool InstallCoreData();
        void InstallUserData(User siteOwner, string mailFromDisplayName, string mailFromEmail, string mailHost, int mailPort, string mailUsername, string mailPassword, string twitterAccessToken, string twitterAccessTokenSecret);
    }
}