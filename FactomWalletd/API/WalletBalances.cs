using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// wallet-balances https://docs.factom.com/api#wallet-balances
    ///
    /// The wallet-balances API is used to query the acknowledged and saved balances for all addresses in the currently running
    /// factom-walletd. The saved balance is the last saved to the database and the acknowledged or “ack” balance is the balance
    /// after processing any in-flight transactions known to the Factom node responding to the API call. The factoid address balance
    /// will be returned in factoshis (a factoshi is 10^8 factoids) not factoids(FCT) and the entry credit balance will be returned
    /// in entry credits.
    /// 
    /// If walletd and factomd are not both running this call will not work.
    /// If factomd is not loaded up all the way to last saved block it will return: “result”:{“Factomd Error”:“Factomd is not fully
    /// booted, please wait and try again.”}
    /// If an address is not in the correct format the call will return: “result”:{“Factomd Error”:”There was an error decoding an address”}
    /// If an address does not have a public and private address known to the wallet it will not be included in the balance.
    /// "fctaccountbalances" are the total of all factoid account balances returned in factoshis.
    /// "ecaccountbalances" are the total of all entry credit account balances returned in entry credits.
    /// </summary>
    public class WalletBalances 
    {
        public WalletBalancesResult     Result    {get; private set;}
        public WalletBalancesRequest    Request   {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public WalletBalances(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new WalletBalancesRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<WalletBalancesResult>(reply.Content);
                return true;
            }
            return false;
        }
        
        public class WalletBalancesRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "wallet-balances";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        public class WalletBalancesResult
        {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    
            public class Result
            {
                [JsonProperty("fctaccountbalances")]
                public Accountbalances Fctaccountbalances { get; set; }
        
                [JsonProperty("ecaccountbalances")]
                public Accountbalances Ecaccountbalances { get; set; }
            
                public class Accountbalances
                {
                    [JsonProperty("ack")]
                    public long Ack { get; set; }
            
                    [JsonProperty("saved")]
                    public long Saved { get; set; }
                }
            }
        }
    }
}
