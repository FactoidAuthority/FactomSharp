using System;
using FactomSharp.Factomd;
using FactomSharp.Factomd.API;

namespace FactomSharp
{
    public class ECAddress
    {
    
        public FactomdRestClient FactomD            { get; private set;}
        public string            Public             { get; private set; }
        public string            Secret             { get; private set; }
        public long              LastBalance        { get; set; }
    
        public ECAddress(FactomdRestClient factomd, string publicAddress, string secretAddress=null)
        {
            Secret  = secretAddress;
            Public  = publicAddress;
            FactomD = factomd;
        }
        
        public long GetBalance()
        {
            var balance = new EntryCreditBalance(FactomD);
            balance.Run(Public);
            
            return LastBalance = balance.Balance;
        }
        
        public decimal? GetEntryCreditRate()
        {
            var ecrate = new EntryCreditRate(FactomD);
            return ecrate.Run();
        }
    }
}
