using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
        [MaxLength(255)]
        public string JobSpezialisation { get; set; }

        public Cashier(int registrationNumber, string firstName, string lastName, Address address, string jobSpezialisation) : base(
            registrationNumber, firstName, lastName, address)
        {
            JobSpezialisation = jobSpezialisation;
        }
        
        
        #pragma warning disable CS8618
        protected Cashier()
        {
            
        }
        #pragma warning restore CS8618
    }
}