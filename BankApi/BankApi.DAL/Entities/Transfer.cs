using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.DAL.Entities
{
    public class Transfer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferID { get; set; }
        public int SourceAccountId { get; set; }
        public int DestinationAccountId { get; set; }
        public decimal TransferAmount { get; set; }
    }
}
