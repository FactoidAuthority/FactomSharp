using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// entry-credit-balance https://docs.factom.com/api#entry-credit-balance
    ///
    /// Return its current balance for a specific entry credit address.
    /// </summary>
    public class EntryCreditBalance
    {
        public EntryCreditBalanceRequest    Request   {get; private set;}
        public EntryCreditBalanceResult     Result    {get; private set;}
        public FactomdRestClient            Client    {get; private set;}
        public string                       JsonReply {get; private set;}
                             
        
        public EntryCreditBalance(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string ECAddress)
        {
            Request = new EntryCreditBalanceRequest();
            Request.param.address = ECAddress;
                        
            return Run(Request);
        }

    
        public bool Run(EntryCreditBalanceRequest requestData)
        {
            var reply = Client.MakeRequest<EntryCreditBalanceRequest>(requestData);
            
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<EntryCreditBalanceResult>(reply.Content);
                return true;
            }
            return false;
        }
    
        /// <summary>
        /// Gets the balance, if the result is available, or -1
        /// </summary>
        public long Balance
        {
            get
            {
                return Result?.result?.Balance ?? -1;
            }
        } 
    
    
    
        public class EntryCreditBalanceRequest
        {
            public EntryCreditBalanceRequest()
            {
              param = new EntryCreditBalanceRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "entry-credit-balance";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("address")]
                public string address { get; set; }
            }
        }
        
        
       public class EntryCreditBalanceResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
    
            public class Result
            {
                [JsonProperty("balance")]
                public long Balance { get; set; }
            }
        }
    }
}