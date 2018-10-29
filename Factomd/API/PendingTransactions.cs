using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// pending-transactions https://docs.factom.com/api#pending-transactions
    ///
    /// Returns an array of factoid transactions that have not yet been recorded in the blockchain, but are known to the system.
    /// </summary>
    public class PendingTransactions
    {
        public PendingTransactionsRequest   Request   {get; private set;}
        public PendingTransactionsResult    Result    {get; private set;}
        public FactomdRestClient            Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public PendingTransactions(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string address)
        {
            Request = new PendingTransactionsRequest();
            Request.param.address = address;
                        
            return Run(Request);
        }

        public bool Run(PendingTransactionsRequest requestData)
        {
            var reply = Client.MakeRequest<PendingTransactionsRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<PendingTransactionsResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class PendingTransactionsRequest
        {
            public PendingTransactionsRequest()
            {
              param = new PendingTransactionsRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "pending-transactions";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("address")]
                public string address { get; set; }
            }
        }
        
        
        public class PendingTransactionsResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }

            public class Result
            {
                [JsonProperty("transactionid")]
                public string Transactionid { get; set; }
        
                [JsonProperty("status")]
                public string Status { get; set; }
        
                [JsonProperty("inputs")]
                public Put[] Inputs { get; set; }
        
                [JsonProperty("outputs")]
                public Put[] Outputs { get; set; }
        
                [JsonProperty("ecoutputs")]
                public object[] Ecoutputs { get; set; }
        
                [JsonProperty("fees")]
                public long Fees { get; set; }
            
                    public class Put
                    {
                        [JsonProperty("amount")]
                        public long Amount { get; set; }
                
                        [JsonProperty("address")]
                        public string Address { get; set; }
                
                        [JsonProperty("useraddress")]
                        public string Useraddress { get; set; }
                    }
                        
            }
        }
    }
}