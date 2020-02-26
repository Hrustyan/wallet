using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Services.Currency.EuropaEu
{
    public class EuropaEuCurrencyServiceException : Exception
    {
        public EuropaEuCurrencyServiceException(string message) : base(message)
        {
        }

        public EuropaEuCurrencyServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
