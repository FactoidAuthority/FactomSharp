using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// reveal-chain https://docs.factom.com/api#reveal-chain
    ///
    /// Reveal an Entry to factomd after the Commit to complete the Entry creation. The reveal-entry hex encoded string is documented
    /// here: https://github.com/FactomProject/FactomDocs/blob/master/factomDataStructureDetails.md#entry
    ///
    /// The reveal-entry API takes a specifically formatted message encoded in hex that includes signatures. If you have a factom-walletd
    /// instance running, you can construct this reveal-entry API call with compose-entry which takes easier to construct arguments.
    ///
    /// The compose-entry api call has two api calls in it’s response: commit-entry and reveal-entry. To successfully create an entry,
    /// the reveal-entry must be called after the commit-entry.
    /// </summary>    
    public class RevealEntry
    {
        public RevealEntryRequest Request   {get; private set;}
        public RevealEntryResult  Result    {get; private set;}
        public FactomdRestClient            Client    {get; private set;}
        public string             JsonReply {get; private set;}
        
        public RevealEntry(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(EntryData entry)
        {
            Request = new RevealEntryRequest();
            Request.param.Entry = entry.MarshalBinary.ToHexString();
                        
            return Run(Request);
        }
    
    
        public bool Run(RevealEntryRequest requestData)
        {
            var reply = Client.MakeRequest<RevealEntryRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<RevealEntryResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        
        public class RevealEntryRequest
        {
            public RevealEntryRequest()
            {
              param = new RevealEntryRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "reveal-entry";
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
        
        public class RevealEntryResult
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