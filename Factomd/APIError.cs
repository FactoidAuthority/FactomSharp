using System;
using Newtonsoft.Json;

namespace FactomSharp.Factomd
{
    public class APIError
    {
        private Exception ex;

        public APIError()
        {
        }
        public APIError(Exception ex)
        {
            this.ex = ex;
            error = new Error();
            error.Message = ex.Message;
        }

        [JsonProperty("jsonrpc")]
        public string jsonrpc { get; set; }

        [JsonProperty("id")]
        public object id { get; set; }

        [JsonProperty("error")]
        public Error error { get; set; }
                
        public partial class Error
        {
            [JsonProperty("code")]
            public long Code { get; set; }
    
            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
