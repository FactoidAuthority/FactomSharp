using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// chain-head https://docs.factom.com/api#chain-head
    /// Return the keymr of the head of the chain for a chain ID (the unique hash created when the chain was created).
    /// </summary>
    
    public class ChainHead
    {
        public ChainHeadRequest     Request   {get; private set;}
        public ChainHeadResult      Result    {get; private set;}
        public FactomdRestClient    Client    {get; private set;}
        public string               JsonReply {get; private set;}
        
        public ChainHead(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string chainID)
        {
            Request = new ChainHeadRequest();
            Request.param.ChainID = chainID;
                        
            return Run(Request);
        }
    
    
        public bool Run(ChainHeadRequest requestData)
        {
            var reply = Client.MakeRequest<ChainHeadRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ChainHeadResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class ChainHeadRequest
        {
            public ChainHeadRequest()
            {
              param = new ChainHeadRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "chain-head";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("chainid")]
                public string ChainID { get; set; }
            }
        }
        
        public class ChainHeadResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
         
            public class Result
            {
                [JsonProperty("chainhead")]
                public string chainHead { get; set; }
        
                [JsonProperty("chaininprocesslist")]
                public bool chaininProcessList { get; set; }
    
            }
             
        }
    }
    
}