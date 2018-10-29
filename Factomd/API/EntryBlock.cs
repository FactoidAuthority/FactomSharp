using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// entry-block https://docs.factom.com/api#entry-block
    ///
    /// Retrieve a specified entry block given its merkle root key. The entry block contains 0 to many entries
    /// </summary>
    public class EntryBlock
    {
        public EntryBlockRequest Request   {get; private set;}
        public EntryBlockResult  Result    {get; private set;}
        public FactomdRestClient           Client    {get; private set;}
        public string            JsonReply {get; private set;}
        
        public EntryBlock(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string keymr)
        {
            Request = new EntryBlockRequest();
            Request.param.keymr = keymr;
                        
            return Run(Request);
        }

    
        public bool Run(EntryBlockRequest requestData)
        {
            var reply = Client.MakeRequest<EntryBlockRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<EntryBlockResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class EntryBlockRequest
        {
            public EntryBlockRequest()
            {
              param = new EntryBlockRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "entry-block";
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
        
        public class EntryBlockResult
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
        
                [JsonProperty("entrylist")]
                public Entrylist[] entrylist { get; set; }
            
            
                public class Entrylist
                {
                    [JsonProperty("entryhash")]
                    public string Entryhash { get; set; }
            
                    [JsonProperty("timestamp")]
                    public long Timestamp { get; set; }
                }
            
                public class Header
                {
                    [JsonProperty("blocksequencenumber")]
                    public long Blocksequencenumber { get; set; }
            
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
            
                    [JsonProperty("prevkeymr")]
                    public string Prevkeymr { get; set; }
            
                    [JsonProperty("timestamp")]
                    public long Timestamp { get; set; }
            
                    [JsonProperty("dbheight")]
                    public long Dbheight { get; set; }
                }
            }
            
        }
    }
    
}