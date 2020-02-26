using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WalletApp.Services.Currency
{
    public interface ICurrencyService
    {
        IEnumerable<string> GetCurrencies();
        decimal Convert(decimal inValue, string inCode, string outCode);

        bool HasCurrencyRate(string code);
    }
}
