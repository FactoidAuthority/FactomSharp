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
    /// import-addresses https://docs.factom.com/api#import-addresses
    ///
    /// Import Factoid and/or Entry Credit address secret keys into the wallet.
    /// </summary>
    public class ImportAddresses
    {
        public ImportAddressesRequest      Request   {get; private set;}
        public ImportAddressesResult       Result    {get; private set;}
        public FactomWalletdRestClient     Client    {get; private set;}
        public string                      JsonReply {get; private set;}
        
        public ImportAddresses(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(String SecretAddress, string PublicAddress)
        {
            var address = new ImportAddressesRequest.Params.Address()
            {
                 Secret = SecretAddress,
                 Public = PublicAddress
            };
            
            Request = new ImportAddressesRequest();
            Request.param.Addresses = new ImportAddressesRequest.Params.Address[]{address};

            return Run(Request);
        }
    
        public bool Run(ImportAddressesRequest.Params.Address[] addresses)
        {
            Request = new ImportAddressesRequest();
            Request.param.Addresses = addresses;

            return Run(Request);
        }
    
        public bool Run(ImportAddressesRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ImportAddressesResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class ImportAddressesRequest
        {
            protected internal ImportAddressesRequest()
            {
              param = new ImportAddressesRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "import-addresses";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            
            public class Params
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
        
        
        public class ImportAddressesResult
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
