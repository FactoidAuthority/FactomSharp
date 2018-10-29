using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using FactomSharp.Factomd;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// address https://docs.factom.com/api#address
    ///
    /// Retrieve the public and private parts of a Factoid or Entry Credit address stored in the wallet.
    /// </summary>
    public class Address
    {
        public AddressRequest           Request   {get; private set;}
        public AddressResult            Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public Address(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string address)
        {
            Request = new AddressRequest();
            Request.param.Address = address;

            return Run(Request);
        }
    
    
        public bool Run(AddressRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AddressResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class AddressRequest
        {
            protected internal AddressRequest()
            {
              param = new AddressRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "address";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            
            public class Params
            {
                [JsonProperty("address")]
                public string Address { get; set; }
            }    
            
        }
        
        public class AddressResult
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
