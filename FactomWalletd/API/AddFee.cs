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
    /// add-fee https://docs.factom.com/api#add-fee
    ///
    /// Addfee is a shortcut and safeguard for adding the required additional factoshis to covert the fee. The fee is displayed
    /// in the returned transaction after each step, but addfee should be used instead of manually adding the additional input.
    /// This will help to prevent overpaying.
    ///
    /// Addfee will complain if your inputs and outputs do not match up. For example, in the steps above we added the inputs
    /// first. This was done intentionally to show a case of overpaying. Obviously, no one wants to overpay for a transaction,
    /// so addfee has returned an error and the message: ‘Inputs and outputs don’t add up’. This is because we have 2,000,000,000
    /// factoshis as input and only 1,000,000,000 + 10,000 as output. Let’s correct the input by doing 'add-input’, and putting
    /// 1000010000 as the amount for the address. It will overwrite the previous input.
    /// </summary>
    public class AddFee
    {
        public AddFeeRequest            Request   {get; private set;}
        public AddFeeResult             Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public AddFee(FactomWalletdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(String txName, string address)
        {
            Request = new AddFeeRequest();
            Request.param.TxName = txName;
            Request.param.Address = address;

            return Run(Request);
        }
    
        public bool Run(AddFeeRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AddFeeResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class AddFeeRequest
        {
            protected internal AddFeeRequest()
            {
              param = new AddFeeRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "add-fee";
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
        
        
        public class AddFeeResult
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
