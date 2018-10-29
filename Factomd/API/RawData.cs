using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// raw-data https://docs.factom.com/api#raw-data
    ///
    /// Retrieve an entry or transaction in raw format, the data is a hex encoded string.
    /// </summary>
    public class RawData
    {
        public RawDataRequest         Request   {get; private set;}
        public RawDataResult          Result    {get; private set;}
        public FactomdRestClient      Client    {get; private set;}
        public string                 JsonReply {get; private set;}
        
        public RawData(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string hash)
        {
            Request = new RawDataRequest();
            Request.param.hash = hash;
                        
            return Run(Request);
        }

    
        public bool Run(RawDataRequest requestData)
        {
            var reply = Client.MakeRequest<RawDataRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<RawDataResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class RawDataRequest
        {
            public RawDataRequest()
            {
              param = new RawDataRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "raw-data";
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
        
        
       public class RawDataResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("data")]
                public string Data { get; set; }
        
            }
        }
    }
}