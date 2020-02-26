using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Models
{
    public class WalletApiErrorResponse : WalletApiResponse
    {
        public WalletApiErrorResponse(string error)
        {
            Error = error;
        }

        public WalletApiErrorResponse(Exception ex):this(ex.Message)
        {
        }

        public string Error { get; set; }
    }
}
