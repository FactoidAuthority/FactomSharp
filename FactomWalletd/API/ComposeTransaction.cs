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
    /// compose-transaction https://docs.factom.com/api#compose-transaction
    ///
    /// Compose transaction marshals the transaction into a hex encoded string. The string can be inputted into the factomd
    ///  API factoid-submit to be sent to the network.
    /// </summary>
    public class ComposeTransaction
    {
        public ComposeTransactionRequest    Request   {get; private set;}
        public ComposeTransactionResult     Result    {get; private set;}
        public FactomWalletdRestClient      Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public ComposeTransaction(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string TxName)
        {
            Request = new ComposeTransactionRequest();
            Request.param.txname = TxName;

            return Run(Request);
        }
    
    
        public bool Run(ComposeTransactionRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ComposeTransactionResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class ComposeTransactionRequest
        {
            protected internal ComposeTransactionRequest()
            {
              param = new ComposeTransactionRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "compose-transaction";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            
            public class Params
            {
                [JsonProperty("tx-name")]
                public string txname { get; set; }
            }    
            
        }
        
        
        public class ComposeTransactionResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
            
            public class Result
            {
                [JsonProperty("jsonrpc")]
                public string Jsonrpc { get; set; }
        
                [JsonProperty("id")]
                public long Id { get; set; }
        
                [JsonProperty("params")]
                public Params param { get; set; }
        
                [JsonProperty("method")]
                public string Method { get; set; }
            
        
                public class Params
                {
                    [JsonProperty("transaction")]
                    public string Transaction { get; set; }
                }
            }
        }
    }
}
