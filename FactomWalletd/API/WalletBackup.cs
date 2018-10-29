using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.FactomWalletd.API
{
    /// <summary>
    /// wallet-backup https://docs.factom.com/api#wallet-backup
    ///     
    /// Return the wallet seed and all addresses in the wallet for backup and offline storage.
    /// </summary>
    public class WalletBackup 
    {
        public WalletBackupResult       Result    {get; private set;}
        public WalletBackupRequest      Request   {get; private set;}
        public FactomWalletdRestClient  Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public WalletBackup(FactomWalletdRestClient client)
        {
            Client = client;
        }
        
        public bool Run()
        {
            Request = new WalletBackupRequest();
         
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(Request);
                        
            var reply = Client.RestClient.Execute(request);
            JsonReply = reply.Content;
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<WalletBackupResult>(reply.Content);
                return true;
            }
            return false;
        }
    
        public class WalletBackupRequest
        {
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "wallet-backup";
            [JsonProperty("id")]
            public long Id { get; set; }
        }
        
        public class WalletBackupResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
    

            public class Result
            {
                [JsonProperty("transactions")]
                public Transaction[] Transactions { get; set; }
            
        
                public class Transaction
                {
                    [JsonProperty("blockheight")]
                    public long Blockheight { get; set; }
            
                    [JsonProperty("feespaid")]
                    public long Feespaid { get; set; }
            
                    [JsonProperty("signed")]
                    public bool Signed { get; set; }
            
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
                    public object Outputs { get; set; }
            
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
}
