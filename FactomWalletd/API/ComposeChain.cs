using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using FactomSharp.Factomd;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// https://docs.factom.com/api#compose-chain
    ///    
    /// <summary>
    /// This method, compose-chain, will return the appropriate API calls to create a chain in factom. You must first call the
    /// commit-chain, then the reveal-chain API calls. To be safe, wait a few seconds after calling commit.
    ///
    /// Notes:
    /// Ensure that all data given in the firstentry fields are encoded in hex. This includes the content section.
    /// </summary>
    public class ComposeChain
    {
        public ComposeChainRequest      Request   {get; private set;}
        public ComposeChainResult       Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public ComposeChain(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(byte[] Content, String ECpub, byte [][] ExtIDs = null)
        {
            if (ExtIDs==null) ExtIDs = FactomUtils.MakeExtIDs();
            
            
            var chainHash = new List<byte>();
            foreach (var extId in ExtIDs) {
                var h = SHA256.Create().ComputeHash(extId);
                chainHash.AddRange(h);
            }
            
            Request = new ComposeChainRequest();
            Request.param.Chain.firstentry.Content = Content.ToHexString();
            Request.param.Chain.firstentry.Extids = ExtIDs.ExtIDsToHexStrings();
            Request.param.Ecpub = ECpub;
            return Run(Request);
        }
    
        public bool Run(ComposeChainRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ComposeChainResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class ComposeChainRequest
        {
            protected internal ComposeChainRequest()
            {
              param = new ComposeChainRequest.Params()
              {
                Chain = new ComposeChainRequest.Chain()
                { 
                    firstentry = new ComposeChainRequest.Chain.Firstentry()
                },
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "compose-chain";
            [JsonProperty("id")]
            public long Id { get; set; }
        
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            public class Params
            {
                [JsonProperty("chain")]
                public Chain Chain { get; set; }
        
                [JsonProperty("ecpub")]
                public string Ecpub { get; set; }
            }
    
            public class Chain
            {
                [JsonProperty("firstentry")]
                public Firstentry firstentry { get; set; }
            
                public class Firstentry
                {
                    [JsonProperty("extids")]
                    public string[] Extids { get; set; }
            
                    [JsonProperty("content")]
                    public string Content { get; set; }
                }
            }
        }
        
        
        public class ComposeChainResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    
            public class Result
            {
                [JsonProperty("commit")]
                public Commit commit { get; set; }
        
                [JsonProperty("reveal")]
                public Reveal reveal { get; set; }
            
                public class Commit
                {
                    [JsonProperty("jsonrpc")]
                    public string Jsonrpc { get; set; }
            
                    [JsonProperty("id")]
                    public long Id { get; set; }
            
                    [JsonProperty("params")]
                    public CommitParams Params { get; set; }
            
                    [JsonProperty("method")]
                    public string Method { get; set; }
            
                    public class CommitParams
                    {
                        [JsonProperty("message")]
                        public string Message { get; set; }
                    }
                }
            
                public class Reveal
                {
                    [JsonProperty("jsonrpc")]
                    public string Jsonrpc { get; set; }
            
                    [JsonProperty("id")]
                    public long Id { get; set; }
            
                    [JsonProperty("params")]
                    public RevealParams Params { get; set; }
            
                    [JsonProperty("method")]
                    public string Method { get; set; }
            
                    public class RevealParams
                    {
                        [JsonProperty("entry")]
                        public string Entry { get; set; }
                    }
                }
            }
        }
    }
}
