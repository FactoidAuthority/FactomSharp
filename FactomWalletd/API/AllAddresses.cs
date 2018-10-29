using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// all-addresses https://docs.factom.com/api#all-addresses
    ///
    /// Retrieve all of the Factoid and Entry Credit addresses stored in the wallet.
    /// </summary>
    public class AllAddresses 
    {
        public AllAddressesResult       Result    {get; private set;}
        public AllAddressesRequest      Request   {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public AllAddresses(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new AllAddressesRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AllAddressesResult>(reply.Content);
                return true;
            }
            return false;
        }
    
        public class AllAddressesRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "all-addresses";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        public class AllAddressesResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    
            public class Result
            {
                [JsonProperty("addresses")]
                public Address[] Addresses { get; set; }
                
                public class Address
                {
                    [JsonProperty("public")]
                    public string Public { get; set; }
            
                    [JsonProperty("secret")]
                    public string Secret { get; set; }
                }
            }   
        }
    }
}
