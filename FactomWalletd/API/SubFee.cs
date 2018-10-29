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
    /// sub-fee https://docs.factom.com/api#sub-fee
    ///
    /// When paying from a transaction, you can also make the receiving transaction pay for it. Using sub fee, you can use
    /// the receiving address in the parameters, and the fee will be deducted from their output amount.
    ///
    /// This allows a wallet to send all it’s factoids, by making the input and output the remaining balance, then using sub fee
    /// on the output address.
    /// </summary>
    public class SubFee
    {
        public SubFeeRequest            Request   {get; private set;}
        public SubFeeResult             Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public SubFee(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(String txName, string address,long amount)
        {
            Request = new SubFeeRequest();
            Request.param.TxName = txName;
            Request.param.Address = address;

            return Run(Request);
        }
    
    
        public bool Run(SubFeeRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<SubFeeResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class SubFeeRequest
        {
            protected internal SubFeeRequest()
            {
              param = new SubFeeRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "sub-fee";
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
            }    
        }
        
        
        public class SubFeeResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
            
            public class Result
            {
                [JsonProperty("feespaid")]
                public long Feespaid { get; set; }
        
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
