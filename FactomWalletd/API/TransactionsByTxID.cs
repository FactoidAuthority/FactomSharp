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
    /// transactions (By TxID) https://docs.factom.com/api#transactions
    ///
    /// This will retrieve a transaction by the given TxID. This call is the fastest way to retrieve a transaction,
    /// but it will not display the height of the transaction. If a height is in the response, it will be 0. To retrieve
    /// the height of a transaction, use the 'By Address’ method
    /// 
    /// This call in the backend actually pushes the request to factomd. For a more informative response, it is advised
    /// to use the factomd transaction method    
    /// </summary>
    public class TransactionsByTxID
    {
        public TransactionsRequest          Request   {get; private set;}
        public TransactionsResult           Result    {get; private set;}
        public FactomWalletdRestClient      Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public TransactionsByTxID(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string txid)
        {
            Request = new TransactionsRequest();
            Request.param.txid = txid;

            return Run(Request);
        }
    
        public bool Run(TransactionsRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<TransactionsResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class TransactionsRequest
        {
            protected internal TransactionsRequest()
            {
              param = new TransactionsRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "transactions";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            public class Params
            {
                [JsonProperty("txid")]
                public string txid   { get; set; }
            }
        }
        
        public class TransactionsResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
            

            public class Result
            {
                [JsonProperty("transactions")]
                public Transaction[] Transactions { get; set; }
            
        
                public class Transaction
                {
                    [JsonProperty("feespaid")]
                    public long Feespaid { get; set; }
            
                    [JsonProperty("signed")]
                    public bool Signed { get; set; }
            
                    [JsonProperty("timestamp")]
                    public long Timestamp { get; set; }
            
                    [JsonProperty("totalecoutputs")]
                    public long Totalecoutputs { get; set; }
            
                    [JsonProperty("totalinputs")]
                    public long Totalinputs { get; set; }
            
                    [JsonProperty("totaloutputs")]
                    public long Totaloutputs { get; set; }
            
                    [JsonProperty("inputs")]
                    public Put[] Inputs { get; set; }
            
                    [JsonProperty("outputs")]
                    public object Outputs { get; set; }
            
                    [JsonProperty("ecoutputs")]
                    public Put[] Ecoutputs { get; set; }
            
                    [JsonProperty("txid")]
                    public string Txid { get; set; }
                
                    public class Put
                    {
                        [JsonProperty("address")]
                        public string Address { get; set; }
                
                        [JsonProperty("amount")]
                        public long Amount { get; set; }
                    }
                }
            }
        }
    }
}
