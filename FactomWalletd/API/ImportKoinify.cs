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
    /// import-koinify https://docs.factom.com/api#import-koinify
    /// 
    /// Import a Koinify crowd sale address into the wallet. In our examples we used the word “yellow” twelve times, note
    /// that in your case the master passphrase will be different.
    /// </summary>
    public class ImportKoinify
    {
        public ImportKoinifyRequest         Request   {get; private set;}
        public ImportKoinifyResult          Result    {get; private set;}
        public FactomWalletdRestClient      Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public ImportKoinify(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(String words)
        {
            Request = new ImportKoinifyRequest();
            Request.param.words = words;

            return Run(Request);
        }
    
        public bool Run(ImportKoinifyRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ImportKoinifyResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class ImportKoinifyRequest
        {
            protected internal ImportKoinifyRequest()
            {
              param = new ImportKoinifyRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "import-koinify";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            public class Params
            {
                [JsonProperty("words")]
                public string words { get; set; }
            }
        }
        
        
        public class ImportKoinifyResult
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
