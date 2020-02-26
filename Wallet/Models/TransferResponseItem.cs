using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Models
{
    public class TransferResponseItem
    {
        public GetWalletsResponseItem SourceWallet { get; set; }
        public GetWalletsResponseItem TargetWallet { get; set; }
    }
}
