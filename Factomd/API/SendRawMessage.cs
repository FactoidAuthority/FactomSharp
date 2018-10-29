using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    /// <summary>
    /// send-raw-message https://docs.factom.com/api#send-raw-message
    /// 
    /// Send a raw hex encoded binary message to the Factom network. This is mostly just for debugging and testing.
    /// To Check Commit Chain Example
    /// Entry Hash : 23af5f7c05a89c0097eed7378c60b8bcc89a284094a81da85fb8faab7b297247
    /// </summary>
    
    public class SendRawMessage
    {
        public SendRawMessageRequest    Request   {get; private set;}
        public SendRawMessageResult     Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public SendRawMessage(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string message)
        {
            Request = new SendRawMessageRequest();
            Request.param.message = message;
                        
            return Run(Request);
        }

    
        public bool Run(SendRawMessageRequest requestData)
        {
            var reply = Client.MakeRequest<SendRawMessageRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<SendRawMessageResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class SendRawMessageRequest
        {
            public SendRawMessageRequest()
            {
              param = new SendRawMessageRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "send-raw-message";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("message")]
                public string message { get; set; }
            }
        }
        
        
        public class SendRawMessageResult
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
                public string Message { get; set; }
        
            }
        }
    }
}