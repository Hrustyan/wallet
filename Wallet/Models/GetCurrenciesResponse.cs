using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Models
{
    public class GetCurrenciesResponse : WalletApiResponseTemplate<IEnumerable<string>>
    {
        public GetCurrenciesResponse(IEnumerable<string> data) : base(true, data)
        {
        }
    }
}
