using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// entrycredit-block https://docs.factom.com/api#entrycredit-block
    ///
    /// Retrieve a specified entrycredit block given its merkle root key. The numbers are minute markers.
    /// </summary>
    public class EntryCreditBlock
    {
        public EntryCreditBlockRequest    Request   {get; private set;}
        public EntryCreditBlockResult     Result    {get; private set;}
        public FactomdRestClient          Client    {get; private set;}
        public string                     JsonReply {get; private set;}
        
        public EntryCreditBlock(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string keymr)
        {
            Request = new EntryCreditBlockRequest();
            Request.param.keymr = keymr;
                        
            return Run(Request);
        }
    
        public bool Run(EntryCreditBlockRequest requestData)
        {
            var reply = Client.MakeRequest<EntryCreditBlockRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<EntryCreditBlockResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class EntryCreditBlockRequest
        {
            public EntryCreditBlockRequest()
            {
              param = new EntryCreditBlockRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "entrycredit-block";
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
        
        public class EntryCreditBlockResult
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
            
                    [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
                    public long? Number { get; set; }
            
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