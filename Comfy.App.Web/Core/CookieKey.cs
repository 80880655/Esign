
namespace Comfy.App.Web.Core
{
    public class CookieKey
    {
        public string AppName = System.Configuration.ConfigurationManager.AppSettings["AppName"].ToString();

        public string LoginInfo
        {
            get { return AppName + "LoginInfo"; }
        }

        public string UserId
        {
            get { return AppName + "UserId"; }
        }

        public string OrgId
        {
            get { return AppName + "OrgId"; }
        }

        public string RegionIndex
        {
            get { return AppName + "RegionIndex"; }
        }

        public string PageSize
        {
            get { return AppName + "PageSize"; }
        }

        public string Region
        {
            get { return AppName + "Region"; }
        }
    }
}
