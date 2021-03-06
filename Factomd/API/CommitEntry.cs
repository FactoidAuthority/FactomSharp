﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// https://docs.factom.com/api#commit-entry
    ///
    /// <summary>
    /// Send an Entry Commit Message to factom to create a new Entry. The entry commit hex encoded string is documented here:
    /// https://github.com/FactomProject/FactomDocs/blob/master/factomDataStructureDetails.md#entry-commit
    ///
    /// The commit-entry API takes a specifically formated message encoded in hex that includes signatures. If you have a factom-walletd
    /// instance running, you can construct this commit-entry API call with compose-entry which takes easier to construct arguments.
    ///
    /// The compose-entry api call has two api calls in it’s response: commit-entry and reveal-entry. To successfully create an entry,
    /// the reveal-entry must be called after the commit-entry.
    ///
    /// Notes:
    /// It is possible to be unable to send a commit, if the commit already exists (if you try to send it twice). This is a mechanism
    /// to prevent you from double spending. If you encounter this error, just skip to the reveal-entry. The error format can be found
    /// here: https://docs.factom.com/api#repeated-commit
    /// </summary>
    public class CommitEntry
    {
        public CommitEntryRequest    Request      {get; private set;}
        public CommitEntryResult     Result       {get; private set;}
        public EntryData             Entry        {get; private set;}
        public FactomdRestClient     Client       {get; private set;}
        public string                JsonReply    {get; private set;}
        
        
        public CommitEntry(FactomdRestClient client) 
        {
            Client = client;
        }
    
        [Obsolete()]
        public bool Run(string chainID, byte[] dataEntry, string ECpublic, string ECprivate, byte [][] ExtIDs = null)
        {
            var ecaddress = new ECAddress(Client,ECpublic, ECprivate);
            return Run(chainID, dataEntry, ecaddress, ExtIDs);
        }
    
        public bool Run(string chainID, byte[] dataEntry, ECAddress ecAddress, byte [][] ExtIDs = null)
        {
            var compose = new ComposeEntry(chainID, dataEntry, ecAddress, ExtIDs);
            
            Request = new CommitEntryRequest();
            Request.param.Message = compose.GetHexString();

            return Run(Request);
        }
    
    
        public bool Run(CommitEntryRequest requestData)
        {
            var restReply = Client.MakeRequest<CommitEntryRequest>(requestData);

            if (restReply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<CommitEntryResult>(restReply.Content);
            }
            
            JsonReply = restReply.Content;
            
            return restReply.StatusCode == System.Net.HttpStatusCode.OK;
        }
    
        
        public class CommitEntryRequest
        {
            public CommitEntryRequest()
            {
              param = new CommitEntryRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "commit-entry";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("message")]
                public string Message { get; set; }
            }
        }
        
        public class CommitEntryResult
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
        
                [JsonProperty("txid")]
                public string Txid { get; set; }
        
                [JsonProperty("entryhash")]
                public string Entryhash { get; set; }
            }
        }
    }
}