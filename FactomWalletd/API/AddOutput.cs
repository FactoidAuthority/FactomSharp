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
    /// add-output https://docs.factom.com/api#add-output
    ///
    /// Adds a factoid address output to the transaction. Keep in mind the output is done in factoshis. 1 factoid is
    /// 1,000,000,000 factoshis.
    /// 
    /// So to send ten factoids, you must send 1,000,000,000 factoshis (no commas in JSON).
    /// </summary>
    public class AddOutput
    {
        public AddOutputRequest          Request   {get; private set;}
        public AddOutputResult           Result    {get; private set;}
        public FactomWalletdRestClient   Client    {get; private set;}
        public string                    JsonReply {get; private set;}
        
        public AddOutput(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(String txName, string address,long amount)
        {
            Request = new AddOutputRequest();
            Request.param.TxName = txName;
            Request.param.Address = address;
            Request.param.Amount = amount;

            return Run(Request);
        }
    
    
        public bool Run(AddOutputRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AddOutputResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class AddOutputRequest
        {
            protected internal AddOutputRequest()
            {
              param = new AddOutputRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "add-output";
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
        
        
        public class AddOutputResult
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
                public object Ecoutputs { get; set; }
        
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
