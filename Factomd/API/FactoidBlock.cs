using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    
    
    /// <summary>
    /// factoid-block https://docs.factom.com/api#factoid-block
    ///
    /// Retrieve a specified factoid block given its merkle root key.
    /// </summary>
    public class FactoidBlock
    {
        public FactoidBlockRequest  Request   {get; private set;}
        public FactoidBlockResult   Result    {get; private set;}
        public FactomdRestClient    Client    {get; private set;}
        public string               JsonReply {get; private set;}
        
        public FactoidBlock(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string keymr)
        {
            Request = new FactoidBlockRequest();
            Request.param.keymr = keymr;
                        
            return Run(Request);
        }

    
        public bool Run(FactoidBlockRequest requestData)
        {
            var reply = Client.MakeRequest<FactoidBlockRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<FactoidBlockResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class FactoidBlockRequest
        {
            public FactoidBlockRequest()
            {
              param = new FactoidBlockRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "factoid-block";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("keymr")]
                public string keymr { get; set; }
            }
        }
        
        public class FactoidBlockResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        


            public class Result
            {
                [JsonProperty("fblock")]
                public Fblock fblock { get; set; }
        
                [JsonProperty("rawdata")]
                public string rawdata { get; set; }
            
        
                public class Fblock
                {
                    [JsonProperty("bodymr")]
                    public string Bodymr { get; set; }
            
                    [JsonProperty("prevkeymr")]
                    public string Prevkeymr { get; set; }
            
                    [JsonProperty("prevledgerkeymr")]
                    public string Prevledgerkeymr { get; set; }
            
                    [JsonProperty("exchrate")]
                    public long Exchrate { get; set; }
            
                    [JsonProperty("dbheight")]
                    public long Dbheight { get; set; }
            
                    [JsonProperty("transactions")]
                    public Transaction[] Transactions { get; set; }
            
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
            
                    [JsonProperty("keymr")]
                    public string Keymr { get; set; }
            
                    [JsonProperty("ledgerkeymr")]
                    public string Ledgerkeymr { get; set; }
                }
            
                public class Transaction
                {
                    [JsonProperty("txid")]
                    public string Txid { get; set; }
            
                    [JsonProperty("blockheight")]
                    public long Blockheight { get; set; }
            
                    [JsonProperty("millitimestamp")]
                    public long Millitimestamp { get; set; }
            
                    [JsonProperty("inputs")]
                    public Put[] Inputs { get; set; }
            
                    [JsonProperty("outputs")]
                    public Put[] Outputs { get; set; }
            
                    [JsonProperty("outecs")]
                    public object[] Outecs { get; set; }
            
                    [JsonProperty("rcds")]
                    public string[] Rcds { get; set; }
            
                    [JsonProperty("sigblocks")]
                    public Sigblock[] Sigblocks { get; set; }
                }
            
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