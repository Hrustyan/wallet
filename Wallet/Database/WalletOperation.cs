using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Database
{
    public class WalletOperation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        public decimal Mov { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
