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
    /// add-input https://docs.factom.com/api#add-input
    ///
    /// Adds an input to the transaction from the given address. The public address is given, and the wallet must have the
    /// private key associated with the address to successfully sign the transaction.
    ///
    /// The input is measured in factoshis, so to send ten factoids, you must input 1,000,000,000 factoshis (without commas in JSON)   
    /// </summary>    
    public class AddInput
    {
        public AddInputRequest          Request   {get; private set;}
        public AddInputResult           Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public AddInput(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(String txName, string address,long amount)
        {
            Request = new AddInputRequest();
            Request.param.TxName = txName;
            Request.param.Address = address;
            Request.param.Amount = amount;

            return Run(Request);
        }
    
    
        public bool Run(AddInputRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AddInputResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class AddInputRequest
        {
            protected internal AddInputRequest()
            {
              param = new AddInputRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "add-input";
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
        
        
        public class AddInputResult
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
                public Input[] Inputs { get; set; }
        
                [JsonProperty("outputs")]
                public object Outputs { get; set; }
        
                [JsonProperty("ecoutputs")]
                public object Ecoutputs { get; set; }
        
                [JsonProperty("txid")]
                public string Txid { get; set; }
        
                public class Input
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
