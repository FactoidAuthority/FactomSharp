using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd
{
/// <summary>
/// ablock-by-height https://docs.factom.com/api#ablock-by-height
///
/// The administrative.
/// Retrieve administrative blocks for any given height.

/// The admin block contains data related to the identities within the factom system and the decisions the system
/// makes as it builds the block chain. The ‘abentries’ (admin block entries) in the JSON response can be of various types,
/// the most common is a directory block signature (DBSig). A majority of the federated servers sign every directory block,
/// meaning every block after m5 will contain 5 DBSigs in each admin block.

/// The ABEntries are detailed here: Github Link
/// </summary>    
    
    public class ABlockByHeight
    {
        public ABlockByHeightRequest    Request   {get; private set;}
        public ABlockByHeightResult     Result    {get; private set;}
        public FactomdRestClient        Client    {get; private set;}
        public string                   JsonReply {get; private set;}
        
        public ABlockByHeight(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(long height)
        {
            Request = new ABlockByHeightRequest();
            Request.param.Height = height;

            return Run(Request);
        }
    
        public bool Run(ABlockByHeightRequest requestData)
        {
            var restReply = Client.MakeRequest<ABlockByHeightRequest>(requestData);
            JsonReply = restReply.Content;            
            
            if (restReply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<ABlockByHeightResult>(restReply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class ABlockByHeightRequest
        {
            public ABlockByHeightRequest()
            {
              param = new ABlockByHeightRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "ablock-by-height";
            [JsonProperty("id")]
            public long Id { get; set; }
        
            [JsonProperty("params")]
            public Params param { get; set; }
        
            public class Params
            {
                [JsonProperty("height")]
                public long Height { get; set; }
            }
        }
        
        public class ABlockByHeightResult
        {
        
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }
    
            [JsonProperty("id")]
            public long Id { get; set; }
    
            [JsonProperty("result")]
            public Result result { get; set; }
         
            public class Result
            {
                [JsonProperty("ablock")]
                public Ablock ablock { get; set; }
        
                [JsonProperty("rawdata")]
                public string Rawdata { get; set; }
            
        
                public class Ablock
                {
                    [JsonProperty("header")]
                    public Header Header { get; set; }
            
                    [JsonProperty("abentries")]
                    public Abentry[] Abentries { get; set; }
            
                    [JsonProperty("backreferencehash")]
                    public string Backreferencehash { get; set; }
            
                    [JsonProperty("lookuphash")]
                    public string Lookuphash { get; set; }
                }
            
                public class Abentry
                {
                    [JsonProperty("identityadminchainid")]
                    public string Identityadminchainid { get; set; }
            
                    [JsonProperty("prevdbsig")]
                    public Prevdbsig Prevdbsig { get; set; }
                }
            
                public class Prevdbsig
                {
                    [JsonProperty("pub")]
                    public string Pub { get; set; }
            
                    [JsonProperty("sig")]
                    public string Sig { get; set; }
                }
            
                public class Header
                {
                    [JsonProperty("prevbackrefhash")]
                    public string Prevbackrefhash { get; set; }
            
                    [JsonProperty("dbheight")]
                    public long Dbheight { get; set; }
            
                    [JsonProperty("headerexpansionsize")]
                    public long Headerexpansionsize { get; set; }
            
                    [JsonProperty("headerexpansionarea")]
                    public string Headerexpansionarea { get; set; }
            
                    [JsonProperty("messagecount")]
                    public long Messagecount { get; set; }
            
                    [JsonProperty("bodysize")]
                    public long Bodysize { get; set; }
            
                    [JsonProperty("adminchainid")]
                    public string Adminchainid { get; set; }
            
                    [JsonProperty("chainid")]
                    public string Chainid { get; set; }
                }
             
            }
         
        }
    }   
}