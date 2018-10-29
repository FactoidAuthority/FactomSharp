using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    
    
    /// <summary>
    /// directory-block-head https://docs.factom.com/api#directory-block-head
    ///
    /// The directory block head is the last known directory block by factom, or in other words, the most recently
    /// recorded block. This can be used to grab the latest block and the information required to traverse the entire blockchain.
    /// </summary>
    public class DirectoryBlockHead
    {
        public DirectoryBlockHeadRequest    Request   {get; private set;}
        public DirectoryBlockHeadResult     Result    {get; private set;}
        public FactomdRestClient            Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public DirectoryBlockHead(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run()
        {
            Request = new DirectoryBlockHeadRequest();
                        
            return Run(Request);
        }

    
        public bool Run(DirectoryBlockHeadRequest requestData)
        {
            var reply = Client.MakeRequest<DirectoryBlockHeadRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<DirectoryBlockHeadResult>(reply.Content);
                return true;
            }
            
            return false;
        }
        
        
        public class DirectoryBlockHeadRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "directory-block-head";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        
       public class DirectoryBlockHeadResult
        {
            [JsonProperty("jsonrpc")]
            public string jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    
            public class Result
            {
                [JsonProperty("keymr")]
                public string keymr { get; set; }
            }
        }
    }
}