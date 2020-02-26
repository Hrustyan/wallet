using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletApp.Database;

namespace WalletApp.Models
{
    public class TransferResponse : WalletApiResponseTemplate<TransferResponseItem>
    {
        public TransferResponse(Wallet sourceWallet, Wallet targetWallet) : base(true)
        {
            Data = new TransferResponseItem()
            {
                SourceWallet = new GetWalletsResponseItem(sourceWallet),
                TargetWallet = new GetWalletsResponseItem(targetWallet)
            };
        }
    }
}
