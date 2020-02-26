using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Models
{
    public class MovementRequest
    {
        public int WalletId { get; set; }
        public decimal Money { get; set; }
    }
}
