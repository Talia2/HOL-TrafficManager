namespace WorldApp.Controllers
{
    using System.Web.Mvc;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using WorldApp.Models;
    using WorldApp.Services;

    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CloudServiceViewModel model = new CloudServiceViewModel()
            {
                HttpHost = Request["HTTP_HOST"],
                ClientIPAddress = Request.UserHostAddress,
                ClientHostName = Request.UserHostName,
                ServerName = Request["SERVER_NAME"],
                CurrentRegion = RoleEnvironment.GetConfigurationSettingValue("HostedServiceRegion"),
                DnsTtl = RoleEnvironment.GetConfigurationSettingValue("DnsTtl")
            };

            ServiceManager manager = new ServiceManager();
            foreach (var service in manager.GetHostedServiceStatus())
            {
                model.HostedServices.Add(service.RowKey, service);
            }

            return this.View(model);
        }

        public ActionResult EnableTraffic(string serviceUrlPrefix, bool enabled)
        {
            ServiceManager manager = new ServiceManager();
            manager.UpdateHostedServiceStatus(serviceUrlPrefix, enabled);

            return this.RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return this.View();
        }
    }
}
