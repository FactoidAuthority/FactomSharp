using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    
    /// <summary>
    /// admin-block https://docs.factom.com/api#admin-block
    ///
    /// Retrieve a specified admin block given its merkle root key.
    /// </summary>
    
    public class AdminBlock
    {
        public AdminBlockRequest    Request   {get; private set;}
        public AdminBlockResult     Result    {get; private set;}
        public FactomdRestClient    Client    {get; private set;}
        public string               JsonReply {get; private set;}
        
        public AdminBlock(FactomdRestClient client)
        {
            Client = client;
        }
    
        public bool Run(string keymr)
        {
            Request = new AdminBlockRequest();
            Request.param.keymr = keymr;
                        
            return Run(Request);
        }
    
        public bool Run(AdminBlockRequest requestData)
        {
            var reply = Client.MakeRequest<AdminBlockRequest>(requestData);
            JsonReply = reply.Content;            
            
            if (reply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<AdminBlockResult>(reply.Content);
                return true;
            }
            
            return false;
        }
    
        
        public class AdminBlockRequest
        {
            public AdminBlockRequest()
            {
              param = new AdminBlockRequest.Params()
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
                [JsonProperty("keymr")]
                public string keymr { get; set; }
            }
        }
        
        public class AdminBlockResult
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
                    [JsonProperty("identityadminchainid", NullValueHandling = NullValueHandling.Ignore)]
                    public string Identityadminchainid { get; set; }
            
                    [JsonProperty("prevdbsig", NullValueHandling = NullValueHandling.Ignore)]
                    public Prevdbsig Prevdbsig { get; set; }
            
                    [JsonProperty("minutenumber", NullValueHandling = NullValueHandling.Ignore)]
                    public long? Minutenumber { get; set; }
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