using System;
using System.Collections.Generic;
using FactomSharp.Factomd;
using FactomSharp.Factomd.API;

namespace FactomSharp
{
    public class FCTAddress
    {
    
        public FactomdRestClient FactomD            { get; private set; }
        public string            Public             { get; private set; }
        public string            Secret             { get; private set; }
        public decimal           LastBalance        { get; set; }
        
        /// <summary>
        /// Func used for the Sign method.
        /// </summary>
        /// <value>Data to sign.</value>
        public Func<byte[],byte[]>   SignFunction { get; set; }
        

        public FCTAddress(FactomdRestClient factomd, string publicAddress, string secretAddress = null)
        {
            Secret  = secretAddress;
            Public  = publicAddress;
            FactomD = factomd;
            
            SignFunction = (data) =>
            {
                return Chaos.NaCl.Ed25519.Sign(data,FactomUtils.GetCombinedKey(Secret.FactomBase58ToBytes(),Public.FactomBase58ToBytes()));
            };
        }
        
        public decimal GetBalance()
        {
            var balance = new FactoidBalance(FactomD);
            balance.Run(Public);
            return LastBalance = balance.Balance;
        }
       
       /// <summary>
       /// Sign the specified data, using the SignFunction
       /// </summary>
       /// <returns>signature.</returns>
       /// <param name="data">Data to sign.</param>
        public byte[] Sign (byte[] data)
        {
            return SignFunction.Invoke(data);
        }
    }
}
