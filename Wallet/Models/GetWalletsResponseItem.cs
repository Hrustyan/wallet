using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletApp.Database;

namespace WalletApp.Models
{
    public class GetWalletsResponseItem
    {
        public GetWalletsResponseItem(Wallet wallet)
        {
            Id = wallet.Id;
            Currency = wallet.Currency;
            Sum = wallet.CacheSum;
        }
        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal Sum { get; set; }
    }
}
