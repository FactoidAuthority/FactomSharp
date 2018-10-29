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
    /// new-transaction https://docs.factom.com/api#new-transaction
    ///
    /// This will create a new transaction. The txid is in flux until the final transaction is signed. Until then, it should
    /// not be used or recorded.
    ///
    /// When dealing with transactions all factoids are represented in factoshis. 1 factoid is 1e8 factoshis, meaning you can
    /// never send anything less than 0 to a transaction (0.5).
    /// </summary>
    public class NewTransaction
    {
        public NewTransactionRequest       Request   {get; private set;}
        public NewTransactionResult        Result    {get; private set;}
        public FactomWalletdRestClient     Client    {get; private set;}
        public string                      JsonReply {get; private set;}
        
        public NewTransaction(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string TxName)
        {
            Request = new NewTransactionRequest();
            Request.param.txname = TxName;

            return Run(Request);
        }
    
    
        public bool Run(NewTransactionRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<NewTransactionResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class NewTransactionRequest
        {
            protected internal NewTransactionRequest()
            {
              param = new NewTransactionRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "new-transaction";
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
        
        
        public class NewTransactionResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
            
            public class Result
            {
                [JsonProperty("feesrequired")]
                public long Feesrequired { get; set; }
        
                [JsonProperty("signed")]
                public bool Signed { get; set; }
        
                [JsonProperty("name")]
                public string Name { get; set; }
        
                [JsonProperty("timestamp")]
                public long Timestamp { get; set; }
        
                [JsonProperty("totalecoutputs")]
                public long Totalecoutputs { get; set; }
        
                [JsonProperty("totalinputs")]
                public long Totalinputs { get; set; }
        
                [JsonProperty("totaloutputs")]
                public long Totaloutputs { get; set; }
        
                [JsonProperty("inputs")]
                public object Inputs { get; set; }
        
                [JsonProperty("outputs")]
                public object Outputs { get; set; }
        
                [JsonProperty("ecoutputs")]
                public object Ecoutputs { get; set; }
        
                [JsonProperty("txid")]
                public string Txid { get; set; }
            }
        }
    }
}
