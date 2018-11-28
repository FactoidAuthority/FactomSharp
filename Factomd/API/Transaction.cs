using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// send-raw-message https://docs.factom.com/api#transaction
    ///     
    /// Retrieve details of a factoid transaction using a transaction’s hash (or corresponding transaction id).
    /// 
    /// Note that information regarding the
    /// 
    /// directory block height,
    /// directory block keymr, and
    /// transaction block keymr
    /// are also included.
    /// 
    /// The "blockheight” parameter in the response will always be 0 when using this call, refer to “includedindirectoryblockheight”
    /// if you need the height.
    /// 
    /// Note: This call will also accept an entry hash as input, in which case the returned data concerns the entry. The returned
    /// fields and their format are shown in the 2nd Example Response at right.
    /// 
    /// Note: If the input hash is non-existent, the returned fields will be as follows:
    /// 
    /// “includedintransactionblock”:“”
    /// “includedindirectoryblock”:“”
    /// “includedindirectoryblockheight”:-1
    /// </summary>
    public class Transaction
    {
        public TransactionRequest    Request   {get; private set;}
        public TransactionResult     Result    {get; private set;}
        public FactomdRestClient     Client    {get; private set;}
        public string                JsonReply {get; private set;}
        
        public Transaction(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string hash)
        {
            Request = new TransactionRequest();
            Request.param.hash = hash;
                        
            return Run(Request);
        }

        public bool Run(TransactionRequest requestData)
        {
            var reply = Client.MakeRequest<TransactionRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<TransactionResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        public class TransactionRequest
        {
            public TransactionRequest()
            {
              param = new TransactionRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "transaction";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("hash")]
                public string hash { get; set; }
            }
        }
        
        
        public class TransactionResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("factoidtransaction")]
                public Factoidtransaction factoidtransaction { get; set; }
        
                [JsonProperty("includedintransactionblock")]
                public string includedintransactionblock { get; set; }
        
                [JsonProperty("includedindirectoryblock")]
                public string includedindirectoryblock { get; set; }
        
                [JsonProperty("includedindirectoryblockheight")]
                public long includedindirectoryblockheight { get; set; }
            
            
                public class Factoidtransaction
                {
                    [JsonProperty("millitimestamp")]
                    public long Millitimestamp { get; set; }
            
                    [JsonProperty("inputs")]
                    public Put[] Inputs { get; set; }
            
                    [JsonProperty("outputs")]
                    public Put[] Outputs { get; set; }
            
                    [JsonProperty("outecs")]
                    public Put[] Outecs { get; set; }
            
                    [JsonProperty("rcds")]
                    public string[] Rcds { get; set; }
            
                    [JsonProperty("sigblocks")]
                    public Sigblock[] Sigblocks { get; set; }
            
                    [JsonProperty("blockheight")]
                    public long Blockheight { get; set; }
                
            
                    public class Put
                    {
                        [JsonProperty("amount")]
                        public long Amount { get; set; }
                
                        [JsonProperty("address")]
                        public string Address { get; set; }
                
                        [JsonProperty("useraddress")]
                        public string Useraddress { get; set; }
                    }
                
                    public class Sigblock
                    {
                        [JsonProperty("signatures")]
                        public string[] Signatures { get; set; }
                    }
                }
            }
        }
    }
}