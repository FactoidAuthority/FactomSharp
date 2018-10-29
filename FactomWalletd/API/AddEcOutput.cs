using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using FactomSharp.Factomd;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// add-ec-output https://docs.factom.com/api#add-ec-output
    /// 
    /// When adding entry credit outputs, the amount given is in factoshis, not entry credits. This means math is required to
    /// determine the correct amount of factoshis to pay to get X EC.
    /// 
    /// (ECRate * ECTotalOutput)
    /// 
    /// In our case, the rate is 1000, meaning 1000 entry credits per factoid. We added 10 entry credits, so we need
    ///  1,000 * 10 = 10,000 factoshis
    /// 
    /// To get the ECRate search in the search bar above for “entry-credit-rate”
    ///
    /// </summary>
    public class AddEcOutput
    {
        public AddEcOutputRequest       Request   {get; private set;}
        public AddEcOutputResult        Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public AddEcOutput(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(String txName, string address,long amount)
        {
            Request = new AddEcOutputRequest();
            Request.param.TxName = txName;
            Request.param.Address = address;
            Request.param.Amount = amount;

            return Run(Request);
        }
    
    
        public bool Run(AddEcOutputRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AddEcOutputResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class AddEcOutputRequest
        {
            protected internal AddEcOutputRequest()
            {
              param = new AddEcOutputRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "add-ec-output";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            
            public class Params
            {
                [JsonProperty("tx-name")]
                public string TxName { get; set; }
        
                [JsonProperty("address")]
                public string Address { get; set; }
        
                [JsonProperty("amount")]
                public long Amount { get; set; }
            }    
            
        }
        
        
        public class AddEcOutputResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
            
            public class Result
            {
                [JsonProperty("feesrequired")]
                public long Feesrequired { get; set; }
        
                [JsonProperty("signed")]
                public bool Signed { get; set; }
        
                [JsonProperty("name")]
                public string Name { get; set; }
        
                [JsonProperty("timestamp")]
                public long Timestamp { get; set; }
        
                [JsonProperty("totalecoutputs")]
                public long Totalecoutputs { get; set; }
        
                [JsonProperty("totalinputs")]
                public long Totalinputs { get; set; }
        
                [JsonProperty("totaloutputs")]
                public long Totaloutputs { get; set; }
        
                [JsonProperty("inputs")]
                public Put[] Inputs { get; set; }
        
                [JsonProperty("outputs")]
                public Put[] Outputs { get; set; }
        
                [JsonProperty("ecoutputs")]
                public Put[] Ecoutputs { get; set; }
        
                [JsonProperty("txid")]
                public string Txid { get; set; }
        
                public class Put
                {
                    [JsonProperty("address")]
                    public string Address { get; set; }
            
                    [JsonProperty("amount")]
                    public long Amount { get; set; }
                }
            }
        }
    }
}
