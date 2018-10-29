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
    /// https://docs.factom.com/api#sign-transaction
    ///
    /// Signs the transaction. It is now ready to be executed.
    /// </summary>
    public class SignTransaction
    {
        public SignTransactionRequest    Request   {get; private set;}
        public SignTransactionResult     Result    {get; private set;}
        public FactomWalletdRestClient   Client    {get; private set;}
        public string                    JsonReply {get; private set;}
        
        public SignTransaction(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string TxName)
        {
            Request = new SignTransactionRequest();
            Request.param.txname = TxName;

            return Run(Request);
        }
    
        public bool Run(SignTransactionRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<SignTransactionResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class SignTransactionRequest
        {
            protected internal SignTransactionRequest()
            {
              param = new SignTransactionRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "sign-transaction";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            public class Params
            {
                [JsonProperty("tx-name")]
                public string txname { get; set; }
            }    
        }
        
        public class SignTransactionResult
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
