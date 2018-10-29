using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd
{
    /// <summary>
    /// properties https://docs.factom.com/api#properties
    ///
    /// Retrieve current properties of factom-walletd, including the wallet and wallet API versions.
    /// </summary>
    public class Properties 
    {
        public PropertiesResult          Result    {get; private set;}
        public PropertiesRequest         Request   {get; private set;}
        public FactomWalletdRestClient   Client    {get; private set;}
        public string                    JsonReply {get; private set;}
        
        public Properties(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new PropertiesRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
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
                [JsonProperty("walletversion")]
                public string walletversion { get; set; }
                
                [JsonProperty("walletapiversion")]
                public string walletapiversion { get; set; }
            }
        }
    }
}
