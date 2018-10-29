using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using FactomSharp;

namespace FactomSharp.Factomd
{
    
    public class EntryData {

        // Hex Bytes
        public byte[] ChainId { get; private set; }
        // binary
        public byte[] Content { get; private set; }
        public byte[][] ExtIDs { get; private set; }
        
        
        public EntryData(byte[] chainId, byte[] content, byte[][] extIDs = null)
        {
            ChainId = chainId;
            Content = content;
            ExtIDs = extIDs;
        }
    
        public EntryData(string chainId, byte[] content, byte[][] extIDs = null)
        {
            ChainId = chainId.DecodeHexIntoBytes();
            Content = content;
            ExtIDs = extIDs;
        }

        public string ChainIdString
        {
          get
          {
            return ChainId.ToHexString();
          }
        }
        
        public string ContentHexString
        {
          get
          {
            return Content.ToHexString();
          }
        }
        
        public string[] ExtIDsHexStrings
        {
          get
          {
            return ExtIDs.ExtIDsToHexStrings();
          }
        }
        
        
        byte[] _MarshalBinary;
        /// <summary>
        ///     Marshals an entry into a byte[] to be sent to restAPI
        /// </summary>
        public byte[] MarshalBinary
        {
            
         get {   
            
            if (_MarshalBinary != null) return _MarshalBinary;
            
            var entryBStruct = new List<byte>();

            // Header 
            // 1 byte version
            byte version = 0;
            entryBStruct.Add(version);
            // 32 byte chainid
            entryBStruct.AddRange(ChainId);
            
            // Ext Ids Size
            if (ExtIDs == null) {
                entryBStruct.AddRange(new byte[2] {0,0});
            }
            else
            {
                var totalSize = 0;
                foreach (var extElement in ExtIDs) {
                    totalSize += extElement.Length + 2;
                }

                var extLen = Convert.ToInt16(totalSize);
                var bytes = BitConverter.GetBytes(extLen);
                entryBStruct.AddRange(bytes.SetEndian());
                // ExtIDS
                entryBStruct.AddRange(ExtIDsBinary);
            }

            // Content
            entryBStruct.AddRange(Content);

            _MarshalBinary = entryBStruct.ToArray();

            return _MarshalBinary;
            
            }
        }

        private byte[] ExtIDsBinary
        {
            get
            {
                var byteList = new List<byte>();
                foreach (var exId in ExtIDs) {
                    // 2 byte size of ExtID
                    var extLen = Convert.ToInt16(exId.Length);
                    var bytes = BitConverter.GetBytes(extLen);
                    bytes = bytes.SetEndian();
                    byteList.AddRange(bytes);
                    var extIdStr = exId;
                    byteList.AddRange(extIdStr);
                }
                return byteList.ToArray();
            }
        }


        
        /// <summary>
        /// Caculates the cost of an entry
        /// </summary>
        public byte EntryCost
        {
            get
            {
                var len = MarshalBinary.Length - 35;
                if (len > 10240) throw new ArgumentException("Parameter cannot exceed 10kb of content");
                
                int remain;
                var num = Math.DivRem(len,1024, out remain);
                
                if (remain > 0 || num == 0) num++;
                
                return (byte)num;
            }
        }
        
        public byte[] Hash
        {
            get
            {
                var data = MarshalBinary;
                var h1 = SHA512.Create().ComputeHash(data);
                var h2 = new byte[h1.Length + data.Length];
                h1.CopyTo(h2, 0);
                data.CopyTo(h2, h1.Length);
                var h3 = SHA256.Create().ComputeHash(h2);
                return h3;
            }
        }
        
    }
    
    
}
