﻿using System;
using FactomSharp.Factomd;
using FactomSharp.Factomd.API;

namespace FactomSharp
{
    public class FCTAddress
    {
    
        public FactomdRestClient FactomD            { get; private set;}
        public string            Public             { get; private set; }
        public string            Secret             { get; private set; }
        public decimal           TransactionValue   { get; set; }
        

        public FCTAddress(FactomdRestClient factomd, string publicAddress, string secretAddress = null)
        {
            Secret  = secretAddress;
            Public  = publicAddress;
            FactomD = factomd;
        }
        
        public decimal GetBalance()
        {
            var balance = new FactoidBalance(FactomD);
            balance.Run(Public);
            
            return balance.Balance;
        }
        
        
        public byte[] Sign (byte[] data)
        {
            return Chaos.NaCl.Ed25519.Sign(data,FactomUtils.GetCombinedKey(Secret.FactomBase58ToBytes(),Public.FactomBase58ToBytes()));
        }
        
        
    }
}
