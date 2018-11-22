using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography;
using RestSharp;

namespace FactomSharp {

    public static class FactomUtils
    {
        
    /// <summary>
    ///     Convience function to emulate Java's CopyOfRange
    /// </summary>
    /// <param name="src">The byte array to copfrom</param>
    /// <param name="start">The index to cut from</param>
    /// <param name="end">The index to cut to</param>
    /// <returns></returns>
    public static byte[] CopyOfRange(this byte[] src, int start, int end) {
        var len = end - start + 1;
        var dest = new byte[len];
        Array.Copy(src, start, dest, 0, len);
        return dest;
    }

    /// <summary>
    ///     Converts byte[] to hex string
    /// </summary>
    /// <param name="ba"></param>
    /// <returns></returns>
    public static string ToHexString(this byte[] ba) {
        var hex = BitConverter.ToString(ba);
        return hex.Replace("-", "").ToLower();
    }

    /// <summary>
    ///     Will correct a little endian byte[]
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] SetEndian(this byte[] bytes)
    {
        if (BitConverter.IsLittleEndian) return bytes.Reverse().ToArray();
        return bytes;
    }

  

    public static byte[] MilliTime() {
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // 6 Byte millisec unix time
        var unixMilliLong = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;
        var unixBytes = BitConverter.GetBytes(unixMilliLong).SetEndian();
        unixBytes = unixBytes.CopyOfRange(2, unixBytes.Length-1);
        return unixBytes;
    }

   

    private static readonly byte[,] ByteLookup = {
        // low nibble
        {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f},
        // high nibble
        {0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0}
    };

    /// <summary>
    ///     Converts string hex into byte[]
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] DecodeHexIntoBytes(this string input) {
        var result = new byte[(input.Length + 1) >> 1];
        var lastcell = result.Length - 1;
        var lastchar = input.Length - 1;
        // count up in characters, but inside the loop will
        // reference from the end of the input/output.
        for (var i = 0; i < input.Length; i++) {
            // i >> 1    -  (i / 2) gives the result byte offset from the end
            // i & 1     -  1 if it is high-nibble, 0 for low-nibble.
            result[lastcell - (i >> 1)] |= ByteLookup[i & 1, HexToInt(input[lastchar - i])];
        }
        return result;
    }

    public static byte[] ToHexBytes(this byte[] data)
    {
        byte[] bytesOut = new byte[data.Length * 2];
        int b;
        for (int i = 0; i < data.Length; i++)
        {
            b = data[i] >> 4;
            bytesOut[i * 2] = (byte)(87 + b + (((b - 10) >> 31) & -39));
            b = data[i] & 0xF;
            bytesOut[i * 2 + 1] = (byte)(87 + b + (((b - 10) >> 31) & -39));
        }
        return bytesOut;
    }


   
    /// <summary>
    ///     Helper function of Hex functions
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static int HexToInt(char c)
    {
        switch (c) {
            case '0':
                return 0;
            case '1':
                return 1;
            case '2':
                return 2;
            case '3':
                return 3;
            case '4':
                return 4;
            case '5':
                return 5;
            case '6':
                return 6;
            case '7':
                return 7;
            case '8':
                return 8;
            case '9':
                return 9;
            case 'a':
            case 'A':
                return 10;
            case 'b':
            case 'B':
                return 11;
            case 'c':
            case 'C':
                return 12;
            case 'd':
            case 'D':
                return 13;
            case 'e':
            case 'E':
                return 14;
            case 'f':
            case 'F':
                return 15;
            default:
                throw new FormatException("Unrecognized hex char " + c);
            }   
        }
        
        private const string Base58Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"; 
        public static byte[] FactomBase58ToBytes(this string addessString)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (int i = 0; i < addessString.Length; i++)
            {
                int digit = Base58Digits.IndexOf(addessString[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", addessString[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            var bytesout = intData.ToByteArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(bytesout);
                
           // var result = bytesout.SkipWhile(b => b == 0);//strip sign byte
       
           var result = new byte[32];
           Array.Copy(bytesout,2,result,0,32);
       
            return result;
        }
        
        //public static string Encode(byte[] data)
        //{
        //    Contract.Requires<ArgumentNullException>(data != null);
        //    Contract.Ensures(Contract.Result<string>() != null);

        //    // Decode byte[] to BigInteger
        //    BigInteger intData = 0;
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        intData = intData * 256 + data[i];
        //    }

        //    // Encode BigInteger to Base58 string
        //    string result = "";
        //    while (intData > 0)
        //    {
        //        int remainder = (int)(intData % 58);
        //        intData /= 58;
        //        result = Base58Digits[remainder] + result;
        //    }

        //    // Append `1` for each leading 0 byte
        //    for (int i = 0; i < data.Length && data[i] == 0; i++)
        //    {
        //        result = '1' + result;
        //    }
        //    return result;
        //}
        
        
        static public string[] MakeExtidsHex()
        {
            string[] extIDs = new string[2];
            extIDs[0] = BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", string.Empty);
            extIDs[1] = BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", string.Empty);
            return extIDs;
        }
        
        static public byte[][] MakeExtIDs()
        {
            byte[][] extIDs = new byte[2][];
            extIDs[0] = Guid.NewGuid().ToByteArray();
            extIDs[1] = Guid.NewGuid().ToByteArray();
            return extIDs;
        }
        
        
        public static string[] ExtIDsToHexStrings(this byte[][] ExtIDs)
        {
            var exid = new string[ExtIDs.Length];
            for (int i=0; i < ExtIDs.Length; i++)
            {
                exid[i]=ExtIDs[i].ToHexString();
            }
            return exid;
        }
        
        
        
        static public byte[] GetCombinedKey(byte[] KeySecret, byte[] KeyPublic)
        {
            var PrivateKey = new byte[64];           
            Array.Copy(KeySecret,PrivateKey,32);
            Array.Copy(KeyPublic,0,PrivateKey,32,32);
            return PrivateKey;
        }
        
        static public ulong ToFactoshi(this decimal value)
        {
            return (ulong)(value / 0.00000001m);
        }

        static public decimal FromFactoshi(this ulong value)
        {
            return value * 0.00000001m;
        }
        
        
        public static byte[] EncodeVarInt_F(this ulong value)
        {
            int outmax = 10;
            int o;
            byte[] output = new byte[outmax];

                for (o=outmax-1; o > 0 ;o--)
                {
                    byte lower7bits = (byte)(value & 0x7f);
                    if (o<outmax-1) lower7bits |= 128;
                    output[o] = lower7bits;
                    value >>= 7;
                    if (value==0) break;
                }
                return output.CopyOfRange(o,outmax-1);
        }
    
    
        public static ulong DecodeVarInt_FInt32(this byte[] input)
        {
            long value = 0;
            foreach(var lower7bits in input)
            {
                value |= (lower7bits & 0x7f);
                if ((lower7bits & 128) != 0) value <<= 7;
            }
            return (ulong)value;
        }

        
    }
}