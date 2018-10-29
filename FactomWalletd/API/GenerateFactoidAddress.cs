using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// generate-factoid-address https://docs.factom.com/api#generate-factoid-address
    ///
    /// Create a new Entry Credit Address and store it in the wallet.
    /// </summary>
    public class GenerateFactoidAddress 
    {
        public GenerateFactoidAddressResult     Result    {get; private set;}
        public GenerateFactoidAddressRequest    Request   {get; private set;}
        public FactomWalletdRestClient          Client    {get; private set;}
        public string                           JsonReply {get; private set;}
        
        public GenerateFactoidAddress(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new GenerateFactoidAddressRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<GenerateFactoidAddressResult>(reply.Content);
                return true;
            }
            return false;
        }
        
        public class GenerateFactoidAddressRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "generate-factoid-address";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
        public class GenerateFactoidAddressResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    
            public class Result
            {
                [JsonProperty("public")]
                public string Public { get; set; }
        
                [JsonProperty("secret")]
                public string Secret { get; set; }
            }
        }
    }
}
