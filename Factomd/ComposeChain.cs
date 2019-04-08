using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace FactomSharp.Factomd
{
    public class ComposeChain
    {
        public byte[]    FirstEntry        { get; set; }
        public ECAddress ECaddress         { get; set; }
        public byte[][]  ExtIDs            { get; set; } = null;
        public string    ChainIdString     { get; set; } = null;
        public EntryData Entry             { get; private set; } 
    
        public ComposeChain(byte[] firstEntry, ECAddress ecAddress, byte [][] extIDs = null, string chainIdString = null)
        {
            FirstEntry      = firstEntry;
            ECaddress       = ecAddress;
            ExtIDs          = extIDs;
            ChainIdString   = chainIdString;
        }
        
        public string GetHexString()
        {
            
            if (ExtIDs==null) ExtIDs = FactomUtils.MakeExtIDs();
                  
            var chainHash = new List<byte>();
            foreach (var extId in ExtIDs) {
                var h = SHA256.Create().ComputeHash(extId);
                chainHash.AddRange(h);
            }
            
            byte[] ChainId;
            if (ChainIdString!=null)
            {   
                ChainId = ChainIdString.DecodeHexIntoBytes();
            }
            else
            {
                ChainId = SHA256.Create().ComputeHash(chainHash.ToArray());
            }
                        
            Entry = new EntryData(ChainId,FirstEntry,ExtIDs);
            
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
            
            //Sign
            var signature = ECaddress.SignFunction(byteList.ToArray());

            //Add in the EC Public key (strip off header and checksum)
            // byteList.AddRange(ECPublicBytes.ToArray());
            byteList.AddRange(ECaddress.Public.FactomBase58ToBytes().ToArray());
            
            //Add signature
            byteList.AddRange(signature);
                        
            return byteList.ToArray().ToHexString();
        }
    }
}
