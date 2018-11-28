using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// factoid-submit https://docs.factom.com/api#factoid-submit
    ///
    /// Submit a factoid transaction. The transaction hex encoded string is documented here: https://github.com/FactomProject/FactomDocs/blob/master/factomDataStructureDetails.md#factoid-transaction
    /// The factoid-submit API takes a specifically formatted message encoded in hex that includes signatures. If you have
    /// a factom-walletd instance running, you can construct this factoid-submit API call with compose-transaction which takes
    /// easier to construct arguments.
    /// </summary>
    public class FactoidSubmit
    {
        public FactoidSubmitRequest     Request   {get; private set;}
        public FactoidSubmitResult      Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public FactoidSubmit(FactomdRestClient client)
        {
            Client = client;
        }
    
    
        public bool Run(byte[] transaction)
        {
            return Run(transaction.ToHexString());
        }
    
        public bool Run(string transaction)
        {
            Request = new FactoidSubmitRequest();
            Request.param.transaction = transaction;
                        
            return Run(Request);
        }

    
        public bool Run(FactoidSubmitRequest requestData)
        {
            var reply = Client.MakeRequest<FactoidSubmitRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<FactoidSubmitResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class FactoidSubmitRequest
        {
            public FactoidSubmitRequest()
            {
              param = new FactoidSubmitRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "factoid-submit";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("transaction")]
                public string transaction { get; set; }
            }
        }
        
        
       public class FactoidSubmitResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
            public class Result
            {
                [JsonProperty("message")]
                public string message { get; set; }
        
                [JsonProperty("txid")]
                public string txid { get; set; }
        
            }
        }
    }
}