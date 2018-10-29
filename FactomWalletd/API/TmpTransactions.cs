using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd
{
    /// <summary>
    /// tmp-transactions https://docs.factom.com/api#tmp-transactions
    ///
    /// Lists all the current working transactions in the wallet. These are transactions that are not yet sent.
    /// </summary>
    public class TmpTransactions
    {
        public TmpTransactionsResult        Result    {get; private set;}
        public TmpTransactionsRequest       Request   {get; private set;}
        public FactomWalletdRestClient      Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public TmpTransactions(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new TmpTransactionsRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<TmpTransactionsResult>(reply.Content);
                return true;
            }
            return false;
        }
        
        public class TmpTransactionsRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "tmp-transactions";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        public class TmpTransactionsResult
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
                    [JsonProperty("tx-name")]
                    public string TxName { get; set; }
            
                    [JsonProperty("txid")]
                    public string Txid { get; set; }
            
                    [JsonProperty("totalinputs")]
                    public long Totalinputs { get; set; }
            
                    [JsonProperty("totaloutputs")]
                    public long Totaloutputs { get; set; }
            
                    [JsonProperty("totalecoutputs")]
                    public long Totalecoutputs { get; set; }
                }
            }
        }
    }
}
