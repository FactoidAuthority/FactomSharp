using System;
using System.Collections.Generic;

namespace FactomSharp.Factomd
{
    public class ComposeEntry
    {

        public string       ChainID           { get; set; }
        public byte[]       DataEntry         { get; set; }
        public ECAddress    EcAddress         { get; set; }
        public byte[][]     ExtIDs            { get; set; } = null;
        public EntryData    Entry             { get; private set; } 
    
    
        public ComposeEntry(string chainID, byte[] dataEntry, ECAddress ecAddress, byte [][] extIDs = null)
        {
            ChainID = chainID;
            DataEntry = dataEntry;
            EcAddress = ecAddress;
            ExtIDs = extIDs;
        }

        public String GetHexString()
        {

            if (ChainID == null) new Exception("Chain ID not set");

            Entry = new EntryData(ChainID, DataEntry, ExtIDs);

            var byteList = new List<byte>();

            //1 byte version
            byteList.Add(0);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(FactomUtils.MilliTime());
            //32 byte Entry Hash
            byteList.AddRange(Entry.Hash);

            // 1 byte number of Entry Credits to pay
            byteList.Add(Entry.EntryCost);

            //Sign
            var signature = EcAddress.SignFunction(byteList.ToArray());
            //Add in the EC Public key (strip off header and checksum)
            byteList.AddRange(EcAddress.Public.FactomBase58ToBytes());

            //Add signature
            byteList.AddRange(signature);

            return byteList.ToArray().ToHexString();
        }
    }
}
