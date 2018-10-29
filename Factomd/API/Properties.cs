using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// properties https://docs.factom.com/api#properties
    ///
    /// Retrieve current properties of the Factom system, including the software and the API versions.
    /// </summary>
    public class Properties
    {
        public PropertiesRequest    Request   {get; private set;}
        public PropertiesResult     Result    {get; private set;}
        public FactomdRestClient    Client    {get; private set;}
        public string               JsonReply {get; private set;}
        
        public Properties(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run()
        {
            Request = new PropertiesRequest();
                        
            return Run(Request);
        }

    
        public bool Run(PropertiesRequest requestData)
        {
            var reply = Client.MakeRequest<PropertiesRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<PropertiesResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        
        public class PropertiesRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "properties";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
       public class PropertiesResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
            public class Result
            {
                [JsonProperty("factomdversion")]
                public long FactomdVersion { get; set; }
        
                [JsonProperty("factomdapiversion")]
                public long FactomdApiVersion { get; set; }
            }
        }
    }
}