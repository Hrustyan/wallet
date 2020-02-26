using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletApp.Database;

namespace WalletApp.Models
{
    public class CreateWalletResponse : WalletApiResponseTemplate<GetWalletsResponseItem>
    {
        public CreateWalletResponse(Wallet wallet):base(true)
        {
            Data = new GetWalletsResponseItem(wallet);
        }
    }
}
