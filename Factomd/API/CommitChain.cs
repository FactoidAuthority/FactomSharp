using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace FactomSharp.Factomd.API
{
    /// <summary>
    /// commit-chain https://docs.factom.com/api#commit-chain
    ///
    /// Send a Chain Commit Message to factomd to create a new Chain. The commit chain hex encoded string is documented here:
    /// https://github.com/FactomProject/FactomDocs/blob/master/factomDataStructureDetails.md#chain-commit
    ///
    /// The commit-chain API takes a specifically formated message encoded in hex that includes signatures. If you have a
    /// factom-walletd instance running, you can construct this commit-chain API call with compose-chain which takes easier
    /// to construct arguments.
    ///
    /// The compose-chain api call has two api calls in it’s response: commit-chain and reveal-chain. To successfully create
    /// a chain, the reveal-chain must be called after the commit-chain.
    ///
    /// Notes:
    /// It is possible to be unable to send a commit, if the commit already exists (if you try to send it twice). This is a
    /// mechanism to prevent you from double spending. If you encounter this error, just skip to the reveal-chain. The error
    /// format can be found here: repeated-commit
    /// </summary>

    public class CommitChain
    {
        public CommitChainRequest    Request      {get; private set;}
        public CommitChainResult     Result       {get; private set;}
        public EntryData             Entry        {get; private set;}
        public FactomdRestClient     Client       {get; private set;}
        public string                JsonReply    {get; private set;}
        
        
        public CommitChain(FactomdRestClient client) 
        {
            Client = client;
        }
    
        public bool Run(byte[] firstEntry, string ECpublic, string ECprivate, byte [][] ExtIDs = null, string chainIdString = null)
        {
            
            if (ExtIDs==null) ExtIDs = FactomUtils.MakeExtIDs();
                  
            var chainHash = new List<byte>();
            foreach (var extId in ExtIDs) {
                var h = SHA256.Create().ComputeHash(extId);
                chainHash.AddRange(h);
            }
            
            byte[] ChainId;
            if (chainIdString!=null)
            {   
                ChainId = chainIdString.DecodeHexIntoBytes();
            }
            else
            {
                ChainId = SHA256.Create().ComputeHash(chainHash.ToArray());
            }
            
                        
            Entry = new EntryData(ChainId,firstEntry,ExtIDs);
            
            var byteList = new List<byte>();

            //1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(FactomUtils.MilliTime());

            // 32 Byte ChainID Hash
            //byte[] chainIDHash = Encoding.ASCII.GetBytes(c.ChainId);
            var chainIDHash = Entry.ChainId;
            chainIDHash = SHA256.Create().ComputeHash(chainIDHash);
            chainIDHash = SHA256.Create().ComputeHash(chainIDHash);
            byteList.AddRange(chainIDHash);

            // 32 byte Weld; sha256(sha256(EntryHash + ChainID))
            var cid = Entry.ChainId;
            var s = Entry.Hash;
            var weld = new byte[cid.Length + s.Length];
            s.CopyTo(weld, 0);
            cid.CopyTo(weld, s.Length);
            weld = SHA256.Create().ComputeHash(weld);
            weld = SHA256.Create().ComputeHash(weld);
            byteList.AddRange(weld);

            // 32 byte Entry Hash of the First Entry
            byteList.AddRange(Entry.Hash);

            // 1 byte number of Entry Credits to pay
            byteList.Add((byte)(Entry.EntryCost + 10));
            
            //Decode Factom addresses address to bytes
            var ECPublicBytes = ECpublic.FactomBase58ToBytes();
            var ECSecretBytes = ECprivate.FactomBase58ToBytes();
            var signature = Chaos.NaCl.Ed25519.Sign(byteList.ToArray(),FactomUtils.GetPrivateKey(ECSecretBytes,ECPublicBytes));
            
            //Add in the EC Public key (strip off header and checksum)
            byteList.AddRange(ECPublicBytes.ToArray());
            
            //Add signature
            byteList.AddRange(signature);
                        
            Request = new CommitChainRequest();
            Request.param.Message = byteList.ToArray().ToHexString();

            return Run(Request);
        }
    
    
        public bool Run(CommitChainRequest requestData)
        {
            var restReply = Client.MakeRequest<CommitChainRequest>(requestData);
            if (restReply.StatusCode == System.Net.HttpStatusCode.OK)
            {            
                Result = JsonConvert.DeserializeObject<CommitChainResult>(restReply.Content);
            }
            
            JsonReply = restReply.Content;
            
            return restReply.StatusCode == System.Net.HttpStatusCode.OK;
        }
    
        
        public class CommitChainRequest
        {
            public CommitChainRequest()
            {
              param = new CommitChainRequest.Params()
              {
              };
            }
        
            [JsonProperty("jsonrpc")]
            public readonly string Jsonrpc = "2.0";
            [JsonProperty("method")]
            public readonly string Method = "commit-chain";
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
        
        public class CommitChainResult
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
        
                [JsonProperty("chainidhash")]
                public string ChainIdHash { get; set; }
            }
        }
    }
}