﻿using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// multiple-fct-balances https://docs.factom.com/api#multiple-fct-balances
    /// 
    /// The multiple-fct-balances API is used to query the acknowledged and saved balances in factoshis (a factoshi is 10^8 factoids) not factoids(FCT) for a list of FCT addresses.
    /// 
    /// currentheight is the current height that factomd was loading.
    /// lastsavedheight is the height last saved to the database.
    /// In balances it returns "ack", "saved" and "err".
    /// 
    /// ack is the balance after processing any in-flight transactions known to the Factom node responding to the API call
    /// saved is the last saved to the database
    /// err is just used to display any error that might have happened during the request. If it is "" that means there was no error.
    /// If the syntax of the parameters is off e.g. missing a quote, a comma, or a square bracket, it will return: {"jsonrpc”:“2.0”,“id”:null,“error”:{“code”:-32600,“message”:“Invalid Request”}}
    /// 
    /// If the parameters are labeled incorrectly the call will return: “{“code”:-32602,“message”:“Invalid params”,“data”:“ERROR! Invalid params passed in, expected 'addresses’”}”
    /// 
    /// If factomd is not loaded up all the way to the last saved block it will return: {“currentheight”:0,“lastsavedheight”:0,“balances”:[{“ack”:0,“saved”:0,“err”:“Not fully booted”}]}
    /// 
    /// If the list of addresses contains an incorrectly formatted address the call will return: {“currentheight”:0,“lastsavedheight”:0,“balances”:[{“ack”:0,“saved”:0,“err”:“Error decoding address”}]}
    /// 
    /// If an address in the list is valid but has never been part of a transaction it will return: “balances”:[{“ack”:0,“saved”:0,“err”:“Address has not had a transaction”}]“
    /// 
    /// Referring to the example request: This Example is for simulation, these addresses may not work or have the same value for mainnet or testnet    
    /// </summary>    
    public class MultipleFCTBalances
    {
        public MultipleFCTBalancesRequest   Request   {get; private set;}
        public MultipleFCTBalancesResult    Result    {get; private set;}
        public FactomdRestClient            Client    {get; private set;}
        public string                       JsonReply {get; private set;}
        
        public MultipleFCTBalances(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string[] addresses)
        {
            Request = new MultipleFCTBalancesRequest();
            Request.param.addresses = addresses;
                        
            return Run(Request);
        }

    
        public bool Run(MultipleFCTBalancesRequest requestData)
        {
            var reply = Client.MakeRequest<MultipleFCTBalancesRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<MultipleFCTBalancesResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class MultipleFCTBalancesRequest
        {
            public MultipleFCTBalancesRequest()
            {
              param = new MultipleFCTBalancesRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "multiple-fct-balances";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("addresses")]
                public string[] addresses { get; set; }
            }
        }
        
        
        public class MultipleFCTBalancesResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
        
            public class Result
            {
                [JsonProperty("currentheight")]
                public long Currentheight { get; set; }
        
                [JsonProperty("lastsavedheight")]
                public long Lastsavedheight { get; set; }
        
                [JsonProperty("balances")]
                public Balance[] Balances { get; set; }
            
        
                public class Balance
                {
                    [JsonProperty("ack")]
                    public long Ack { get; set; }
            
                    [JsonProperty("saved")]
                    public long Saved { get; set; }
            
                    [JsonProperty("err")]
                    public string Err { get; set; }
                }
            }
        }
    }
}