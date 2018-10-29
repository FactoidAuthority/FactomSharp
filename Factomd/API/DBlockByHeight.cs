using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// dblock-by-height https://docs.factom.com/api#dblock-by-height
    ///
    /// Retrieve a directory block given only its height.
    /// </summary>
    public class DBlockByHeight
    {
        public DBlockByHeightRequest    Request   {get; private set;}
        public DBlockByHeightResult     Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public DBlockByHeight(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(long height)
        {
            Request = new DBlockByHeightRequest();
            Request.param.Height = height;
                        
            return Run(Request);
        }
    
        public bool Run(DBlockByHeightRequest requestData)
        {
            var reply = Client.MakeRequest<DBlockByHeightRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<DBlockByHeightResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class DBlockByHeightRequest
        {
            public DBlockByHeightRequest()
            {
              param = new DBlockByHeightRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "dblock-by-height";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("height")]
                public long Height { get; set; }
            }
        }
        
        public class DBlockByHeightResult
        {
            [JsonProperty("jsonrpc")]
            public string jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
        
            public class Result
            {
                [JsonProperty("dblock")]
                public Dblock dblock { get; set; }
        
                [JsonProperty("rawdata")]
                public string rawdata { get; set; }
            
        
                public class Dblock
                {
                    [JsonProperty("header")]
                    public Header Header { get; set; }
            
                    [JsonProperty("dbentries")]
                    public Dbentry[] Dbentries { get; set; }
            
                    [JsonProperty("dbhash")]
                    public string Dbhash { get; set; }
            
                    [JsonProperty("keymr")]
                    public string Keymr { get; set; }
                }
            
                public class Dbentry
                {
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
            
                    [JsonProperty("keymr")]
                    public string Keymr { get; set; }
                }
            
                public class Header
                {
                    [JsonProperty("version")]
                    public long Version { get; set; }
            
                    [JsonProperty("networkid")]
                    public long Networkid { get; set; }
            
                    [JsonProperty("bodymr")]
                    public string Bodymr { get; set; }
            
                    [JsonProperty("prevkeymr")]
                    public string Prevkeymr { get; set; }
            
                    [JsonProperty("prevfullhash")]
                    public string Prevfullhash { get; set; }
            
                    [JsonProperty("timestamp")]
                    public long Timestamp { get; set; }
            
                    [JsonProperty("dbheight")]
                    public long Dbheight { get; set; }
            
                    [JsonProperty("blockcount")]
                    public long Blockcount { get; set; }
            
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
                }
            }
        }
    }   
}