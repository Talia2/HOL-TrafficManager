namespace WorldApp.Controllers
{
    using System.Web.Mvc;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using WorldApp.Services;

    public class AppHealthController : Controller
    {
        public ActionResult Index()
        {
            string serviceUrlPrefix = RoleEnvironment.GetConfigurationSettingValue("HostedServiceUrlPrefix");

            ServiceManager manager = new ServiceManager();
            bool isOnline = manager.GetHostedServiceStatus(serviceUrlPrefix);
            if (!isOnline)
            {
                Response.Close();
            }
            
            return new EmptyResult();
        }
    }
}
