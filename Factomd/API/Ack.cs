using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    
    /// <summary>
    /// Ack. https://docs.factom.com/api#ack
    /// This api call is used to find the status of a transaction, whether it be a factoid, reveal entry, or commit entry.
    /// When using this, you must specify the type of the transaction by giving the chainid field 1 of 3 values:
    ///
    /// f for factoid transactions
    /// c for entry credit transactions (commit entry/chain)
    /// ################################################################ for reveal entry/chain
    /// Where # is the ChainID of the entry
    /// The status types returned are as follows:
    /// 
    /// “Unknown” : Not found anywhere
    /// “NotConfirmed” : Found on local node, but not in network (Holding Map)
    /// “TransactionACK” : Found in network, but not written to the blockchain yet (ProcessList)
    /// “DBlockConfirmed” : Found in Blockchain
    /// You may also provide the full marshaled transaction, instead of a hash, and it will be hashed for you.
    ///
    /// The responses vary based on the type:
    /// </summary>
    
    public class Ack
    {
        public AckRequest     Request   {get; private set;}
        public AckResult      Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string         JsonReply {get; private set;}
        
        public enum Status
        {
            RequestFailed,
            Unknown,
            NotConfirmed,
            TransactionACK,
            DBlockConfirmed
        }
        
        public Ack(FactomdRestClient client)
        {
            Client = client;
        }
    
        public Status CheckFactoidTransaction(string hash)
        {
            return Check(hash,"f");
        }
    
        public Status CheckCommit(string hash)
        {
            return Check(hash,"c");
        }
        
        public Status CheckReveal(string chainID,string entryHash)
        {
            return Check(entryHash,chainID);
        }
        
        private Status Check(string hash,string chainid)
        {
            Request = new AckRequest();
            Request.param.Chainid=chainid;
            Request.param.hash = hash;
            
            var execute = Run(Request);
            if (!execute) return Status.RequestFailed;
            
            Status status = Status.Unknown;
            if (!Enum.TryParse(Result?.result?.Entrydata?.Status ?? "null", out status))
                    if (!Enum.TryParse(Result?.result?.Commitdata?.Status ?? "null", out status))
                        Enum.TryParse(Result?.result.Status ?? "unknown", out status);
            
            return status;
        }
    
        public bool Run(AckRequest requestData)
        {
            var reply = Client.MakeRequest<AckRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AckResult>(reply.Content);
                return true;
            }

            return false;
        }
    
        
        public class AckRequest
        {
            public AckRequest()
            {
              param = new AckRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "ack";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("hash")]
                public string hash { get; set; }
                
                [JsonProperty("chainid")]
                public string Chainid { get; set; }
        
                [JsonProperty("fulltransaction")]
                public string Fulltransaction { get; set; }
            }
        }
        
        
       public class AckResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("committxid")]
                public string Committxid { get; set; }
        
                [JsonProperty("entryhash")]
                public string Entryhash { get; set; }
        
                [JsonProperty("commitdata")]
                public Data Commitdata { get; set; }
        
                [JsonProperty("entrydata")]
                public Data Entrydata { get; set; }
                
                [JsonProperty("txid")]
                public string TxID { get; set; }
                
                [JsonProperty("status")]
                public string Status { get; set; }
                
                public class Data
                {
                    [JsonProperty("status")]
                    public string Status { get; set; }
                }
            }
        }
    }
}