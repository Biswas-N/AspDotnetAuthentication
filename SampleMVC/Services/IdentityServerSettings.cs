namespace SampleMVC.Services
{
    public class IdentityServerSettings
    {
        public string DiscoveryUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseHttps { get; set; }
    }
}