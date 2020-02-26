using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Models
{
    public class TransferRequest
    {
        public int SourceWalletId { get; set; }
        public int TargetWalletId { get; set; }
        public decimal Sum { get; set; }
    }
}
