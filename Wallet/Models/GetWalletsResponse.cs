using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletApp.Database;

namespace WalletApp.Models
{
    public class GetWalletsResponse : WalletApiResponseTemplate<IEnumerable<GetWalletsResponseItem>>
    {
        public GetWalletsResponse(IEnumerable<Wallet> wallets) : base(true)
        {
            Data = wallets.Select(wallet => new GetWalletsResponseItem(wallet));
        }
    }
}
