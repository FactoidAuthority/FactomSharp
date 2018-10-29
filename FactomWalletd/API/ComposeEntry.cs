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
    /// compose-entry https://docs.factom.com/api#compose-entry
    ///
    /// This method, compose-entry, will return the appropriate API calls to create an entry in factom. You must first call
    /// the commit-entry, then the reveal-entry API calls. To be safe, wait a few seconds after calling commit.
    ///
    /// Notes:
    /// Ensure all data given in the entry fields are encoded in hex. This includes the content section.
    /// </summary>
    public class ComposeEntry
    {
        public ComposeEntryRequest      Request   {get; private set;}
        public ComposeEntryResult       Result    {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public ComposeEntry(FactomWalletdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(string ChainID, byte[] Content, String ECpub, byte [][] ExtIDs = null)
        {
           
            Request = new ComposeEntryRequest();
            Request.param.Ecpub = ECpub;
            Request.param.entry = new ComposeEntryRequest.Params.Entry();
            
            Request.param.entry.Chainid = ChainID;
            Request.param.entry.Content = Content.ToHexString();
            Request.param.entry.Extids = ExtIDs.ExtIDsToHexStrings();
            
            return Run(Request);
        }
    
    
        public bool Run(ComposeEntryRequest requestData)
        {
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(requestData);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ComposeEntryResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        public class ComposeEntryRequest
        {
            protected internal ComposeEntryRequest()
            {
              param = new ComposeEntryRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "compose-entry";
            [JsonProperty("id")]
            public long Id { get; set; }
        
        
            [JsonProperty("params")]
            public Params param { get; set; }
            
            public class Params
            {
                [JsonProperty("entry")]
                public Entry entry { get; set; }

                [JsonProperty("ecpub")]
                public string Ecpub { get; set; }

                public class Entry
                {
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
            
                    [JsonProperty("extids")]
                    public string[] Extids { get; set; }
            
                    [JsonProperty("content")]
                    public string Content { get; set; }
                }
            }
        }
        
        
        public class ComposeEntryResult
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
                }
            
                public class CommitParams
                {
                    [JsonProperty("message")]
                    public string Message { get; set; }
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
                }
            
                public class RevealParams
                {
                    [JsonProperty("entry")]
                    public string Entry { get; set; }
                }    
            }
        }
    }
}
