using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// directory-block https://docs.factom.com/api#directory-block
    ///
    /// Every directory block has a KeyMR (Key Merkle Root), which can be used to retrieve it. The response will contain information
    /// that can be used to navigate through all transactions (entry and factoid) within that block. The header of the directory block
    /// will contain information regarding the previous directory block’s keyMR, directory block height, and the timestamp.
    /// </summary>
    public class DirectoryBlock
    {
        public DirectoryBlockRequest    Request   {get; private set;}
        public DirectoryBlockResult     Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public DirectoryBlock(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string keymr)
        {
            Request = new DirectoryBlockRequest();
            Request.param.keymr = keymr;
                        
            return Run(Request);
        }
    
        public bool Run(DirectoryBlockRequest requestData)
        {
            var reply = Client.MakeRequest<DirectoryBlockRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<DirectoryBlockResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class DirectoryBlockRequest
        {
            public DirectoryBlockRequest()
            {
              param = new DirectoryBlockRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "ablock-by-height";
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
        
        public class DirectoryBlockResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("result")]
            public Result result { get; set; }
        
            
            public class Result
            {
                [JsonProperty("header")]
                public Header header { get; set; }
        
                [JsonProperty("entryblocklist")]
                public Entryblocklist[] entryblocklist { get; set; }
            
                public class Entryblocklist
                {
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
            
                    [JsonProperty("keymr")]
                    public string Keymr { get; set; }
                }
            
                public class Header
                {
                    [JsonProperty("prevblockkeymr")]
                    public string Prevblockkeymr { get; set; }
            
                    [JsonProperty("sequencenumber")]
                    public long Sequencenumber { get; set; }
            
                    [JsonProperty("timestamp")]
                    public long Timestamp { get; set; }
                }
            }
        }
    }   
}