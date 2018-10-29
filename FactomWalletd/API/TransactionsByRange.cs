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
    /// transactions (By Range) https://docs.factom.com/api#using-a-range
    ///
    /// There are a few ways to search for a transaction
    ///
    /// Using a Range
    /// This will retrieve all transactions within a given block height range.
    /// </summary>
    public class TransactionsByRange
    {
        public TransactionsRequest          Request   {get; private set;}
        public TransactionsResult           Result    {get; private set;}
        public FactomWalletdRestClient      Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public TransactionsByRange(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(long start, long end)
        {
            Request = new TransactionsRequest();
            Request.param.start = start;
            Request.param.end = end;

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
                [JsonProperty("start")]
                public long start   { get; set; }
        
                [JsonProperty("end")]
                public long end     { get; set; }
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
                    [JsonProperty("blockheight")]
                    public long Blockheight { get; set; }
            
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
