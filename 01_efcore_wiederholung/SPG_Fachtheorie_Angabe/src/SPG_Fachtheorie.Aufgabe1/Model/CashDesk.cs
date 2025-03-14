using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class CashDesk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Number { get; set; }

        public CashDesk(int number)
        {
            Number = number;
        }
        
        #pragma warning disable CS8618
        protected CashDesk() { }
        #pragma warning restore CS8618
    }
}