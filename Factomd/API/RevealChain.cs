using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// reveal-chain https://docs.factom.com/api#reveal-chain
    ///
    /// Reveal the First Entry in a Chain to factomd after the Commit to complete the Chain creation. The reveal-chain hex encoded
    /// string is documented here: https://github.com/FactomProject/FactomDocs/blob/master/factomDataStructureDetails.md#entry
    ///
    /// The reveal-chain API takes a specifically formatted message encoded in hex that includes signatures. If you have a factom-walletd
    /// instance running, you can construct this reveal-chain API call with compose-chain which takes easier to construct arguments.
    ///
    /// The compose-chain api call has two api calls in its response: commit-chain and reveal-chain. To successfully create a chain, the reveal-chain must be called after the commit-chain.
    /// </summary>
    public class RevealChain
    {
        public RevealChainRequest Request   {get; private set;}
        public RevealChainResult  Result    {get; private set;}
        public FactomdRestClient            Client    {get; private set;}
        public string             JsonReply {get; private set;}
        
        public RevealChain(FactomdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(EntryData entry)
        {
            Request = new RevealChainRequest();
            Request.param.Entry = entry.MarshalBinary.ToHexString();
                        
            return Run(Request);
        }
    
    
        public bool Run(RevealChainRequest requestData)
        {
            var reply = Client.MakeRequest<RevealChainRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<RevealChainResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        
        public class RevealChainRequest
        {
            public RevealChainRequest()
            {
              param = new RevealChainRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "reveal-chain";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("entry")]
                public string Entry { get; set; }
            }
        }
        
        public class RevealChainResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
         
            public class Result
            {
                [JsonProperty("message")]
                public string Message { get; set; }
        
                [JsonProperty("entryhash")]
                public string Entryhash { get; set; }
        
                [JsonProperty("chainid")]
                public string Chainid { get; set; }
            }
             
        }
    } 
}