using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd
{
    /// <summary>
    /// generate-ec-address https://docs.factom.com/api#generate-ec-address
    ///
    /// Create a new Entry Credit Address and store it in the wallet.
    /// </summary>
    public class GenerateEcAddress 
    {
        public GenerateEcAddressResult      Result    {get; private set;}
        public GenerateEcAddressRequest     Request   {get; private set;}
        public FactomWalletdRestClient      Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public GenerateEcAddress(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new GenerateEcAddressRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<GenerateEcAddressResult>(reply.Content);
                return true;
            }
            return false;
        }
        
        public class GenerateEcAddressRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "generate-ec-address";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
        public class GenerateEcAddressResult
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
