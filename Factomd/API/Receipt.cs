using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// receipt https://docs.factom.com/api#receipt
    ///
    /// Retrieve a receipt providing cryptographically verifiable proof that information was recorded in the factom blockchain
    /// and that this was subsequently anchored in the bitcoin blockchain.
    /// </summary>
    public class Receipt
    {
        public ReceiptRequest Request   {get; private set;}
        public ReceiptResult  Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string         JsonReply {get; private set;}
        
        public Receipt(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string hash)
        {
            Request = new ReceiptRequest();
            Request.param.Hash = hash;
                        
            return Run(Request);
        }
    
        public bool Run(ReceiptRequest requestData)
        {
            var reply = Client.MakeRequest<ReceiptRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ReceiptResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class ReceiptRequest
        {
            public ReceiptRequest()
            {
              param = new ReceiptRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "receipt";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("hash")]
                public string Hash { get; set; }
            }
        }
        
        public class ReceiptResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
         
            public class Result
            {
                [JsonProperty("receipt")]
                public Receipt receipt { get; set; }
                
                
                public class Receipt
                {
                    [JsonProperty("entry")]
                    public Entry Entry { get; set; }
            
                    [JsonProperty("merklebranch")]
                    public Merklebranch[] Merklebranch { get; set; }
            
                    [JsonProperty("entryblockkeymr")]
                    public string Entryblockkeymr { get; set; }
            
                    [JsonProperty("directoryblockkeymr")]
                    public string Directoryblockkeymr { get; set; }
            
                    [JsonProperty("bitcointransactionhash")]
                    public string Bitcointransactionhash { get; set; }
            
                    [JsonProperty("bitcoinblockhash")]
                    public string Bitcoinblockhash { get; set; }
                }
            
                public class Entry
                {
                    [JsonProperty("entryhash")]
                    public string Entryhash { get; set; }
                }
            
                public class Merklebranch
                {
                    [JsonProperty("left")]
                    public string Left { get; set; }
            
                    [JsonProperty("right")]
                    public string Right { get; set; }
            
                    [JsonProperty("top")]
                    public string Top { get; set; }
                }
            }
        }
    }   
}