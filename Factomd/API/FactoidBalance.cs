using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// entry-credit-balance https://docs.factom.com/api#factoid-balance
    ///
    /// This call returns the number of Factoshis (Factoids *10^-8) that are currently available at the address specified.
    /// </summary>
    public class FactoidBalance
    {
        public FactoidBalanceRequest    Request   {get; private set;}
        public FactoidBalanceResult     Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public FactoidBalance(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string FCTaddress)
        {
            Request = new FactoidBalanceRequest();
            Request.param.address = FCTaddress;
                        
            return Run(Request);
        }

    
        public bool Run(FactoidBalanceRequest requestData)
        {
            var reply = Client.MakeRequest<FactoidBalanceRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<FactoidBalanceResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the balance, if the result is available, or -1
        /// </summary>
        public decimal Balance
        {
            get
            {
                return Result?.result?.Balance / 0.0000000001m ?? -1;
            }
        } 
        
        public class FactoidBalanceRequest
        {
            public FactoidBalanceRequest()
            {
              param = new FactoidBalanceRequest.Params()
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
        
        
       public class FactoidBalanceResult
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