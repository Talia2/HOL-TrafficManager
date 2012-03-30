namespace WorldApp.Services
{
    using Microsoft.WindowsAzure.StorageClient;

    public class HostedServiceStatus : TableServiceEntity
    {
        public string Region { get; set; }
        public bool IsOnline { get; set; }
    }
}