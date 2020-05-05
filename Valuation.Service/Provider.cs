namespace Valuation.Service
{
    public class Provider
    {
        public Provider(string serviceName, string serviceAgent, string key)
        {
            ServiceName = serviceName;
            ServiceAgent = serviceAgent;
            Key = key;
        }

        public string ServiceName { get; }
        public string ServiceAgent { get; }
        public string Key { get; }
    }
}