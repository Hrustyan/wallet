using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Database
{
    public class Wallet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Currency { get; set; }

        [ConcurrencyCheck]
        public decimal CacheSum { get; set; }

        [ForeignKey(nameof(WalletOperation.WalletId))]
        public List<WalletOperation> Operations { get; set; }
    }
}
