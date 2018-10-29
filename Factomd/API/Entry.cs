using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// entry https://docs.factom.com/api#entry
    ///
    /// Get an Entry from factomd specified by the Entry Hash.
    /// </summary>
    public class Entry
    {
        public EntryRequest         Request   {get; private set;}
        public EntryResult          Result    {get; private set;}
        public FactomdRestClient    Client    {get; private set;}
        public string               JsonReply {get; private set;}
        
        public Entry(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string hash)
        {
            Request = new EntryRequest();
            Request.param.hash = hash;
                        
            return Run(Request);
        }

    
        public bool Run(EntryRequest requestData)
        {
            var reply = Client.MakeRequest<EntryRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<EntryResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class EntryRequest
        {
            public EntryRequest()
            {
              param = new EntryRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "entry";
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
        
        
       public class EntryResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("chainid")]
                public string Chainid { get; set; }
        
                [JsonProperty("content")]
                public string Content { get; set; }
        
                [JsonProperty("extids")]
                public string[] Extids { get; set; }
    
                public EntryData EntryData
                {
                    get
                    {
                        byte[][] extidBytes = null;
                        if (Extids!=null)
                        {
                            extidBytes = new byte[Extids.Length][];
                            for (int i=0; i < Extids.Length; i++)
                            {
                                extidBytes[i] = Extids[i].DecodeHexIntoBytes();
                            }
                        }
                        
                        return new EntryData(Chainid,Content.DecodeHexIntoBytes(),extidBytes);
                    }
                }
            }
        }
    }
}