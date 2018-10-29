using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// get-height https://docs.factom.com/api#get-height
    /// 
    /// Get the current hight of blocks that have been cached by the wallet while syncing.
    /// </summary>
    public class GetHeight 
    {
        public GetHeightResult          Result    {get; private set;}
        public GetHeightRequest         Request   {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public GetHeight(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new GetHeightRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<GetHeightResult>(reply.Content);
                return true;
            }
            return false;
        }
        
        public class GetHeightRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "get-height";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
        public class GetHeightResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    
            public class Result
            {
                [JsonProperty("height")]
                public long Height { get; set; }
            }
        }
    }
}
