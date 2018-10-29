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
    /// delete-transaction https://docs.factom.com/api#delete-transaction
    ///
    /// Deletes a working transaction in the wallet. The full transaction will be returned, and then deleted.
    /// </summary>
    public class DeleteTransaction
    {
        public DeleteTransactionRequest    Request   {get; private set;}
        public DeleteTransactionResult     Result    {get; private set;}
        public FactomWalletdRestClient     Client    {get; private set;}
        public string                      JsonReply {get; private set;}
        
        public DeleteTransaction(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string TxName)
        {
            Request = new DeleteTransactionRequest();
            Request.param.txname = TxName;

            return Run(Request);
        }
    
        public bool Run(DeleteTransactionRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<DeleteTransactionResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class DeleteTransactionRequest
        {
            protected internal DeleteTransactionRequest()
            {
              param = new DeleteTransactionRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "delete-transaction";
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
        
        public class DeleteTransactionResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
            
            public class Result
            {
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
            }
        }
    }
}
