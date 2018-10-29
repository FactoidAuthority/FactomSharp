﻿using System;
using FactomSharp.Factomd;

namespace FactomSharp.Helper
{
    public class FCTAddress
    {
    
        public FactomdRestClient FactomD { get; private set;}
        public string            Public  { get; private set; }
        public string            Secret  { get; private set; }
    
        public FCTAddress(FactomdRestClient factomd, string secretAddress, string publicAddress)
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
        
    }
}