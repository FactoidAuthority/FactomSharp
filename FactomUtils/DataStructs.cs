using System.Collections.Generic;

namespace FactomSharp
{
    public class DataStructs {
        public static EntryBlockData ConvertStringFormatToByteFormat(EntryBlockDataStringFormat blockStringFormat) {
            var block = new EntryBlockData {Header = new EntryBlockData.HeaderData()};

            var len = blockStringFormat.EntryList.Length;
            block.EntryList = new EntryBlockData.EntryData[len];

            for (var i = 0; i < len; i++) {
                block.EntryList[i] = new EntryBlockData.EntryData {
                    EntryHash = blockStringFormat.EntryList[i].EntryHash.DecodeHexIntoBytes(),
                    Timestamp = blockStringFormat.EntryList[i].Timestamp
                };
            }

            block.Header.BlockSequenceNumber = blockStringFormat.Header.BlockSequenceNumber;
            block.Header.ChainId = blockStringFormat.Header.ChainId.DecodeHexIntoBytes();
            block.Header.PrevKeyMr = blockStringFormat.Header.PrevKeyMr.DecodeHexIntoBytes();
            block.Header.Timestamp = blockStringFormat.Header.Timestamp;

            return block;
        }

        public static ChainHeadData ConvertStringFormatToByteFormat(ChainHeadDataStringFormat chainStringFormat) {
            var chain = new ChainHeadData {ChainHead = chainStringFormat.ChainHead.DecodeHexIntoBytes()};
            return chain;
        }

        public static EntryBlockDataStringFormat ConvertByteFormatToStringFormat(EntryBlockData block) {
            var blockStringFormat = new EntryBlockDataStringFormat();
            var len = block.EntryList.Length;
            blockStringFormat.EntryList = new EntryBlockDataStringFormat.EntryData[len];

            for (var i = 0; i < len; i++) {
                blockStringFormat.EntryList[i] = new EntryBlockDataStringFormat.EntryData {
                    EntryHash = block.EntryList[i].EntryHash.ToHexString() ,
                    Timestamp = block.EntryList[i].Timestamp
                };
            }

            blockStringFormat.Header.BlockSequenceNumber = block.Header.BlockSequenceNumber;
            blockStringFormat.Header.ChainId = block.Header.ChainId.ToHexString();
            blockStringFormat.Header.PrevKeyMr = block.Header.PrevKeyMr.ToHexString();
            blockStringFormat.Header.Timestamp = block.Header.Timestamp;

            return blockStringFormat;
        }

        


        public class EntryBlockData {
            public HeaderData Header { get; set; }
            public EntryData[] EntryList { get; set; }

            public class HeaderData {
                public int BlockSequenceNumber { get; set; }
                // Hex Bytes
                public byte[] ChainId { get; set; }
                // Hex Bytes
                public byte[] PrevKeyMr { get; set; }
                public int Timestamp { get; set; }
            }

            public class EntryData {
                // Hex Bytes
                public byte[] EntryHash { get; set; }
                public int Timestamp { get; set; }
            }
        }

        public class ChainHeadData {
            // KeyMR of first Entry in Entry chain
            // Hex Bytes
            public byte[] ChainHead { get; set; }
        }

        //                             \\
        //  Conversion and Json Tools  \\
        //                             \\

        public class EntryDataStringFormat {
            public string ChainId { get; set; }
            public string Content { get; set; }
            public string[] ExtIDs { get; set; }
        }

        public class EntryBlockDataStringFormat {
            public HeaderData Header { get; set; }
            public EntryData[] EntryList { get; set; }

            public class HeaderData {
                public int BlockSequenceNumber { get; set; }
                public string ChainId { get; set; }
                public string PrevKeyMr { get; set; }
                public int Timestamp { get; set; }
            }

            public class EntryData {
                public string EntryHash { get; set; }
                public int Timestamp { get; set; }
            }
        }

        public class ChainHeadDataStringFormat {
            // KeyMR of first Entry in Entry chain
            // Hex Bytes
            public string ChainHead { get; set; }
        }
    }
}