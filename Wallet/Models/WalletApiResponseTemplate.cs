using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Models
{
    public class WalletApiResponseTemplate<T>: WalletApiResponse
    {
        public WalletApiResponseTemplate(bool result)
        {
            Result = result;
        }

        public WalletApiResponseTemplate(bool result, T data):this(result)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
