using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// entry-credit-rate https://docs.factom.com/api#entry-credit-rate
    ///
    /// Returns the number of Factoshis (Factoids *10^-8) that purchase a single Entry Credit. The minimum factoid fees are also
    /// determined by this rate, along with how complex the factoid transaction is.
    /// </summary>
    public class EntryCreditRate
    {
        public EntryCreditRateRequest    Request   {get; private set;}
        public EntryCreditRateResult     Result    {get; private set;}
        public FactomdRestClient         Client    {get; private set;}
        public string                    JsonReply {get; private set;}
        
        public EntryCreditRate(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run()
        {
            Request = new EntryCreditRateRequest();
            return Run(Request);
        }

    
        public bool Run(EntryCreditRateRequest requestData)
        {
            var reply = Client.MakeRequest<EntryCreditRateRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<EntryCreditRateResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class EntryCreditRateRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "entry-credit-rate";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
       public class EntryCreditRateResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("rate")]
                public long Rate { get; set; }
            }
        }
    }
}