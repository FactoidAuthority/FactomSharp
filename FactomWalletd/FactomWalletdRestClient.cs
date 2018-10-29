using System;
using RestSharp;

namespace FactomSharp.FactomWalletd
{
    public class FactomWalletdRestClient
    {
    
        public RestClient RestClient {get; private set;}
        
        public FactomWalletdRestClient(RestClient restclient)
        {
            RestClient = restclient;
        }
    
        public FactomWalletdRestClient(string url)
        {
            if (!url.EndsWith("/v2")) url = url + "/v2";
            RestClient = new RestClient(url);
        }
        
    }
}
