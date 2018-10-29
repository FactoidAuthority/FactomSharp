using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// heights https://docs.factom.com/api#heights
    ///
    /// Returns various heights that allows you to view the state of the blockchain. The heights returned provide a lot of
    /// information regarding the state of factomd, but not all are needed by most applications. The heights also indicate
    /// the most recent block, which could not be complete, and still being built. The heights mean as follows:
    ///
    /// directoryblockheight : The current directory block height of the local factomd node.
    /// leaderheight : The current block being worked on by the leaders in the network. This block is not yet complete, but all transactions submitted will go into this block (depending on network conditions, the transaction may be delayed into the next block)
    /// entryblockheight : The height at which the factomd node has all the entry blocks. Directory blocks are obtained first, entry blocks could be lagging behind the directory block when syncing.
    /// entryheight : The height at which the local factomd node has all the entries. If you added entries at a block height above this, they will not be able to be retrieved by the local factomd until it syncs further.
    ///
    /// A fully synced node should show the same number for all, (except between minute 0 and 1, when leaderheight will be 1 block ahead.)
    /// </summary>
    public class Heights
    {
        public HeightsRequest       Request   {get; private set;}
        public HeightsResult        Result    {get; private set;}
        public FactomdRestClient    Client    {get; private set;}
        public string               JsonReply {get; private set;}
        
        public Heights(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run()
        {
            Request = new HeightsRequest();
                        
            return Run(Request);
        }

    
        public bool Run(HeightsRequest requestData)
        {
            var reply = Client.MakeRequest<HeightsRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<HeightsResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        
        public class HeightsRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "heights";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
       public class HeightsResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("directoryblockheight")]
                public long Directoryblockheight { get; set; }
        
                [JsonProperty("leaderheight")]
                public long Leaderheight { get; set; }
        
                [JsonProperty("entryblockheight")]
                public long Entryblockheight { get; set; }
        
                [JsonProperty("entryheight")]
                public long Entryheight { get; set; }
            }
        }
    }
}