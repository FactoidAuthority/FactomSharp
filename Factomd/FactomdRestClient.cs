using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    public class FactomdRestClient
    {
    
        public RestClient RestClient {get; private set;}
        
        public List<string> URLlist = new List<string>();
        public int URLlistIndex = -1;
        public int TimeOutMs {get; private set;}
        
        public FactomdRestClient(RestClient restclient)
        {
            RestClient = restclient;
        }
    
        public FactomdRestClient(string url, int timeOutMs = 10000)
        {
            TimeOutMs = timeOutMs;
            SetURLs(url);
            OpenNextRestClient();
        }
        
        void SetURLs(string urls)
        {
            var urlList = urls.Split(',',';');
            foreach(var url in urlList)
            {
                if (!url.EndsWith("/v2"))
                  URLlist.Add(url + "/v2");
                 else 
                  URLlist.Add(url);
            }
            URLlistIndex = -1;
        }
        
        bool OpenNextRestClient()
        {
            URLlistIndex++;
            if (URLlistIndex >= URLlist.Count)
            {
                 URLlistIndex=-1;
                 return false;
            }
            var url = URLlist[URLlistIndex];
            
            RestClient = new RestClient(url)
            {
                Timeout = TimeOutMs
            };
            return true;
        }
        
        
        public IRestResponse MakeRequest<T>(T requestData) where T : class
        {
                var request = new RestRequest(Method.POST);
                request.JsonSerializer = new NewtonsoftJsonSerializer();
                request.AddJsonBody(requestData);
    
                var response = RestClient.Execute(request);
                
                switch (response.StatusCode)
                {
                    case 0:
                    case HttpStatusCode.RequestTimeout:
                        if (OpenNextRestClient()) response = MakeRequest<T>(requestData);
                        break;
                }
                
                return response;
        }
        
        
    }
}
