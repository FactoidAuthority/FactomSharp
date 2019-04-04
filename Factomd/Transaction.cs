using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace FactomSharp.Factomd
{
    public class Transaction
    {
    
        public Dictionary<FCTAddress,decimal> FCT_Input    = new Dictionary<FCTAddress,decimal>();
        public Dictionary<FCTAddress,decimal> FCT_Output   = new Dictionary<FCTAddress,decimal>();
        public Dictionary<ECAddress,decimal>  EC_Output    = new Dictionary<ECAddress,decimal>();
        
        public decimal Fee { get; set;}
        
        public byte[] TXID                     { get; private set;}
            
        public Transaction()
        {
        }

        public void AddFCTInput(FCTAddress fct, decimal subtract_value)
        {
            FCT_Input.Add(fct,subtract_value);
        }

        public void AddFCTOutput(FCTAddress fct, decimal send_value)
        {
            FCT_Output.Add(fct,send_value);
        }

        public void AddECOutput(ECAddress ec, ulong send_value)
        {
            EC_Output.Add(ec,send_value);
        }
        
        public void AddRCDType1(byte[] publicKey, byte[] signature)
        {
            var byteList = new List<byte>();

        }
        

        
        public byte[] GetTransaction()
        {
            var byteList = new List<byte>();
            var marshalBinary = GetMarshalBinary();
            byteList.AddRange(marshalBinary);
            
            // Add RCDs Type 1
            foreach(var input in FCT_Input)
            {
                byteList.Add(1); //The RCD type. This specifies how the datastructure should be interpreted. Type 1 is the only currently valid type. Can safely be coded using 1 byte for the first 127 types.
                byteList.AddRange(input.Key.Public.FactomBase58ToBytes());
                
                // variable Signature 0 This is the data needed to satisfy RCD 0. It is a signature for type 1, but might be other types of data for later RCD types. Its length is dependent on the RCD type.
                byteList.AddRange(input.Key.Sign(marshalBinary));
            }
            
          //  var hash2 = SHA256.Create().ComputeHash(byteList.ToArray());
            return byteList.ToArray();
            
        }
        
        
        public byte[] GetMarshalBinary()
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
                if (fct.Key == FCT_Input.First().Key)
                {
                    byteList.AddRange((fct.Value + Fee).ToFactoshi().EncodeVarInt_F());
                    
                }
                else
                {
                    byteList.AddRange(fct.Value.ToFactoshi().EncodeVarInt_F());
                }
                byteList.AddRange(fct.Key.Public.FactomBase58ToBytes());
            }
            
            // Factoid Outputs
            // varInt_F (Output X) This is how much the Output X Factoshi balance will be increased by.
            // 32 bytes (Output X) This is an RCD hash which will have its balance increased.
            foreach (var fct in FCT_Output)
            {
                byteList.AddRange(fct.Value.ToFactoshi().EncodeVarInt_F());
                byteList.AddRange(fct.Key.Public.FactomBase58ToBytes());
            }

            
            // Entry Credit Purchase
            // varInt_F (Purchase X) This many Factoshis worth of ECs will be credited to the Entry Credit public key X.
            // 32 bytes EC Pubkey (Purchase X) This is Entry Credit public key that will have its balance increased.
            foreach (var ec in EC_Output)
            {
                byteList.AddRange(ec.Value.ToFactoshi().EncodeVarInt_F());
                byteList.AddRange(ec.Key.Public.FactomBase58ToBytes());
            }

            var marshalBinary = byteList.ToArray();
            TXID = SHA256.Create().ComputeHash(marshalBinary);
           
            return marshalBinary;
        }

        public int ComputeTransactionSize()
        {
            var len = 10; //Header
            
            foreach(var fct in FCT_Input)  //Input + RCD
            {
                len += fct.Value.ToFactoshi().EncodeVarInt_F().Length;
                len += (32+1+32+64);  //32 byte public key - 1 byte RCD type, 32 public key, 64 Sign
            }
            
            foreach (var fct in FCT_Output)
            {
                len += fct.Value.ToFactoshi().EncodeVarInt_F().Length;
                len += 32;  //public key
            }
            
            foreach (var ec in EC_Output)
            {
                len += ec.Value.ToFactoshi().EncodeVarInt_F().Length;
                len += 32;
            }
            
            return len;
        }
        
        public uint ComputeRequiredFeesEc()
        {
            var size = ComputeTransactionSize();
            var fee = Math.Floor((size + 1023) / 1024m);
            fee += 10 * (this.FCT_Output.Count + this.EC_Output.Count);
            fee += this.FCT_Input.Count; //RCDS
            return (uint)fee;
        }
        
        public decimal ComputeRequiredFeesFCT(decimal rate)
        {
            var ecs = ComputeRequiredFeesEc();
            return ecs * rate;
        }
        

        public bool SendTransaction(FactomdRestClient factomd)
        {
            var trans = new Factomd.API.FactoidSubmit(factomd);
            return trans.Run(GetTransaction().ToHexString());
        }
        
    }
}
