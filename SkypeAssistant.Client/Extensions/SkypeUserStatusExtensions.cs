using SkypeAssistant.Client.Models;
using SKYPE4COMLib;

namespace SkypeAssistant.Client.Extensions
{
    public static class SkypeUserStatusExtensions
    {
        public static UserStatus ToUserStatus(this TUserStatus status)
        {
            UserStatus userStatus;
            switch (status)
            {
                case TUserStatus.cusOnline:
                    userStatus = UserStatus.Online;
                    break;
                case TUserStatus.cusOffline:
                case TUserStatus.cusLoggedOut:
                    userStatus = UserStatus.Offline;
                    break;
                case TUserStatus.cusDoNotDisturb:
                    userStatus = UserStatus.Busy;
                    break;
                case TUserStatus.cusAway:
                    userStatus = UserStatus.Away;
                    break;
                default:
                    userStatus = UserStatus.None;
                    break;
            }
            return userStatus;
        }

        public static UserStatus ToUserStatus(this TOnlineStatus status)
        {
            UserStatus userStatus;
            switch (status)
            {
                case TOnlineStatus.olsOnline:
                    userStatus = UserStatus.Online;
                    break;
                case TOnlineStatus.olsOffline:
                case TOnlineStatus.olsNotAvailable:
                    userStatus = UserStatus.Offline;
                    break;
                case TOnlineStatus.olsDoNotDisturb:
                    userStatus = UserStatus.Busy;
                    break;
                case TOnlineStatus.olsAway:
                    userStatus = UserStatus.Away;
                    break;
                default:
                    userStatus = UserStatus.None;
                    break;
            }
            return userStatus;
        }
    }
}
