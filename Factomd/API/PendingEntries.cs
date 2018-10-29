using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// pending-entries https://docs.factom.com/api#pending-entries
    ///
    /// Returns an array of the entries that have been submitted but have not been recorded into the blockchain.
    /// </summary>
    public class PendingEntries
    {
        public PendingEntriesRequest    Request   {get; private set;}
        public PendingEntriesResult     Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public PendingEntries(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run()
        {
            Request = new PendingEntriesRequest();
                        
            return Run(Request);
        }
    
        public bool Run(PendingEntriesRequest requestData)
        {
            var reply = Client.MakeRequest<PendingEntriesRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<PendingEntriesResult>(reply.Content);
                return true;
            }
            
            return false;
        }

        
        public class PendingEntriesRequest
        {
            public PendingEntriesRequest()
            {
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "pending-entries";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
       public class PendingEntriesResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result[] result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("entryhash")]
                public string Entryhash { get; set; }
        
                [JsonProperty("chainid")]
                public string Chainid { get; set; }
        
                [JsonProperty("status")]
                public string Status { get; set; }
            }
        }
    }
}