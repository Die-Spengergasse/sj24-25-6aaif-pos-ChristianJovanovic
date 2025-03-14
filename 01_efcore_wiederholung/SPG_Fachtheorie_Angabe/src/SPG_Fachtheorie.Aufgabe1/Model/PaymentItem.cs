using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class PaymentItem
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string ArticleName { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public Payment Payment { get; set; }

        public PaymentItem(string articleName, int amount, decimal price, Payment payment)
        {
            ArticleName = articleName;
            Amount = amount;
            Price = price;
            Payment = payment;
        }

#pragma warning disable CS8618
        protected PaymentItem()
        {
        }
#pragma warning restore CS8618
    }
}