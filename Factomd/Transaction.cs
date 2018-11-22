using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace FactomSharp.Factomd
{
    public class Transaction
    {
    
        public List<FCTAddress> FCT_Input    = new List<FCTAddress>();
        public List<FCTAddress> FCT_Output   = new List<FCTAddress>();
        public List<ECAddress>  EC_Output    = new List<ECAddress>();
        
        public byte[] TXID                     { get; private set;}
            
        public Transaction()
        {
        }
        public Transaction(FCTAddress fctInput, FCTAddress fctOutput = null, ECAddress ecAddress = null)
        {
            FCT_Input.Add(fctInput);
            if (fctOutput!=null) FCT_Output.Add(fctOutput);
            if (ecAddress!=null) EC_Output.Add(ecAddress);
        }
        
        public void AddFCTInput(FCTAddress fct)
        {
            FCT_Input.Add(fct);
        }

        public void AddFCTOutput(FCTAddress fct)
        {
            FCT_Output.Add(fct);
        }

        public void AddECOutput(ECAddress ec)
        {
            EC_Output.Add(ec);
        }          
        
        public byte[] GetTransaction()
        {
                        
            var byteList = new List<byte>();

            //1 byte version
            byteList.Add(2);

            // 6 byte milliTimestamp (truncated unix time)
            byteList.AddRange(FactomUtils.MilliTime());


            // 1 byte Input Count. This is how many Factoid addresses are being spent from in this transaction.
            byteList.Add((byte)FCT_Input.Count);
            // 1 byte Factoid Output Count. This is how many Factoid addresses are being spent to in this transaction.
            byteList.Add((byte)FCT_Output.Count);
            // 1 byte Entry Credit Purchase Count. This is how many Entry Credit addresses are being spent to in this transaction.
            byteList.Add((byte)EC_Output.Count);
            
            //INPUTS
            
            // varInt_F (Input X) This is how much the Factoshi balance of Input X will be decreased by.
            // 32 bytes Factoid Address (Input X) This is an RCD hash which previously had value assigned to it.
            foreach (var fct in FCT_Input)
            {
                byteList.AddRange(fct.TransactionValue.ToFactoshi().EncodeVarInt_F());
                byteList.AddRange(fct.Public.FactomBase58ToBytes());
            }
            
            // Factoid Outputs
            // varInt_F (Output X) This is how much the Output X Factoshi balance will be increased by.
            // 32 bytes (Output X) This is an RCD hash which will have its balance increased.
            foreach (var fct in FCT_Output)
            {
                byteList.AddRange(fct.TransactionValue.ToFactoshi().EncodeVarInt_F());
                byteList.AddRange(fct.Public.FactomBase58ToBytes());
            }

            
            // Entry Credit Purchase
            // varInt_F (Purchase X) This many Factoshis worth of ECs will be credited to the Entry Credit public key X.
            // 32 bytes EC Pubkey (Purchase X) This is Entry Credit public key that will have its balance increased.
            foreach (var ec in EC_Output)
            {
                byteList.AddRange(ec.TransactionValue.ToFactoshi().EncodeVarInt_F());
                byteList.AddRange(ec.Public.FactomBase58ToBytes());
            }

            var marshalBinary = byteList.ToArray();
            TXID = SHA256.Create().ComputeHash(marshalBinary);
            
            
            // Redeem Condition Datastructure (RCD) Reveal / Signature Section
            // variable RCD 0 First RCD. It hashes to input 0. The length is dependent on the RCD type, which is in the first byte. There are as many RCDs as there are inputs.
            foreach (var fct in FCT_Input)
            {
                byteList.Add(1); //The RCD type. This specifies how the datastructure should be interpreted. Type 1 is the only currently valid type. Can safely be coded using 1 byte for the first 127 types.
                byteList.AddRange(fct.Public.FactomBase58ToBytes());
                
                // variable Signature 0 This is the data needed to satisfy RCD 0. It is a signature for type 1, but might be other types of data for later RCD types. Its length is dependent on the RCD type.
                byteList.AddRange(fct.Sign(marshalBinary));
            }
            
            var hash2 = SHA256.Create().ComputeHash(byteList.ToArray());
                     

            return byteList.ToArray();
        }


        
    }
}
