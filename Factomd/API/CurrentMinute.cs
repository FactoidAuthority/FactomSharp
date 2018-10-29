using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// current-minute https://docs.factom.com/api#current-minute
    ///
    /// The current-minute API call returns:
    /// leaderheight returns the current block height.
    /// directoryblockheight returns the last saved height.
    /// minute returns the current minute number for the open entry block.
    /// currentblockstarttime returns the start time for the current block.
    /// currentminutestarttime returns the start time for the current minute.
    /// currenttime returns the current nodes understanding of current time.
    /// directoryblockinseconds returns the number of seconds per block.
    /// stalldetected returns if factomd thinks it has stalled.
    /// faulttimeout returns the number of seconds before leader node is faulted for failing to provide a necessary message.
    /// roundtimeout returns the number of seconds between rounds of an election during a fault.
    /// </summary>
    public class CurrentMinute
    {
        public CurrentMinuteRequest     Request   {get; private set;}
        public CurrentMinuteResult      Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public CurrentMinute(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run()
        {
            Request = new CurrentMinuteRequest();
            return Run(Request);
        }

    
        public bool Run(CurrentMinuteRequest requestData)
        {
            var reply = Client.MakeRequest<CurrentMinuteRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<CurrentMinuteResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        
        public class CurrentMinuteRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "current-minute";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
       public class CurrentMinuteResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("leaderheight")]
                public long Leaderheight { get; set; }
        
                [JsonProperty("directoryblockheight")]
                public long Directoryblockheight { get; set; }
        
                [JsonProperty("minute")]
                public long Minute { get; set; }
        
                [JsonProperty("currentblockstarttime")]
                public long Currentblockstarttime { get; set; }
        
                [JsonProperty("currentminutestarttime")]
                public long Currentminutestarttime { get; set; }
        
                [JsonProperty("currenttime")]
                public long Currenttime { get; set; }
        
                [JsonProperty("directoryblockinseconds")]
                public long Directoryblockinseconds { get; set; }
        
                [JsonProperty("stalldetected")]
                public bool Stalldetected { get; set; }
        
                [JsonProperty("faulttimeout")]
                public long Faulttimeout { get; set; }
        
                [JsonProperty("roundtimeout")]
                public long Roundtimeout { get; set; }
            }
        }
    }
}