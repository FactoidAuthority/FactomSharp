using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    
    /// <summary>
    /// ecblock-by-height https://docs.factom.com/api#ecblock-by-height
    ///
    /// Retrieve the entry credit block for any given height. These blocks contain entry credit transaction information.
    /// </summary>    
    public class ECBlockByHeight
    {
        public ECBlockByHeightRequest    Request   {get; private set;}
        public ECBlockByHeightResult     Result    {get; private set;}
        public FactomdRestClient         Client    {get; private set;}
        public string                    JsonReply {get; private set;}
        
        public ECBlockByHeight(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(long height)
        {
            Request = new ECBlockByHeightRequest();
            Request.param.Height = height;
                        
            return Run(Request);
        }
    
        public bool Run(ECBlockByHeightRequest requestData)
        {
            var reply = Client.MakeRequest<ECBlockByHeightRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ECBlockByHeightResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class ECBlockByHeightRequest
        {
            public ECBlockByHeightRequest()
            {
              param = new ECBlockByHeightRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "ecblock-by-height";
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
        
        public class ECBlockByHeightResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
         
            public class Result
            {
                [JsonProperty("ecblock")]
                public Ecblock ecblock { get; set; }
        
                [JsonProperty("rawdata")]
                public string rawdata { get; set; }
            
        
                public class Ecblock
                {
                    [JsonProperty("header")]
                    public Header Header { get; set; }
            
                    [JsonProperty("body")]
                    public Body Body { get; set; }
                }
            
                public class Body
                {
                    [JsonProperty("entries")]
                    public Entry[] Entries { get; set; }
                }
            
                public class Entry
                {
                    [JsonProperty("serverindexnumber", NullValueHandling = NullValueHandling.Ignore)]
                    public long? Serverindexnumber { get; set; }
            
                    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
                    public long? Version { get; set; }
            
                    [JsonProperty("millitime", NullValueHandling = NullValueHandling.Ignore)]
                    public string Millitime { get; set; }
            
                    [JsonProperty("entryhash", NullValueHandling = NullValueHandling.Ignore)]
                    public string Entryhash { get; set; }
            
                    [JsonProperty("credits", NullValueHandling = NullValueHandling.Ignore)]
                    public long? Credits { get; set; }
            
                    [JsonProperty("ecpubkey", NullValueHandling = NullValueHandling.Ignore)]
                    public string Ecpubkey { get; set; }
            
                    [JsonProperty("sig", NullValueHandling = NullValueHandling.Ignore)]
                    public string Sig { get; set; }
            
                    [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
                    public long? Number { get; set; }
                }
            
                public class Header
                {
                    [JsonProperty("bodyhash")]
                    public string Bodyhash { get; set; }
            
                    [JsonProperty("prevheaderhash")]
                    public string Prevheaderhash { get; set; }
            
                    [JsonProperty("prevfullhash")]
                    public string Prevfullhash { get; set; }
            
                    [JsonProperty("dbheight")]
                    public long Dbheight { get; set; }
            
                    [JsonProperty("headerexpansionarea")]
                    public string Headerexpansionarea { get; set; }
            
                    [JsonProperty("objectcount")]
                    public long Objectcount { get; set; }
            
                    [JsonProperty("bodysize")]
                    public long Bodysize { get; set; }
            
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
            
                    [JsonProperty("ecchainid")]
                    public string Ecchainid { get; set; }
                }
            }
        }
    }   
}