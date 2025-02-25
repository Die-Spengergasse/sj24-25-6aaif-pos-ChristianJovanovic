using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
        [MaxLength(255)]
        public string CarType { get; set; }
        
        public Manager(int registrationNumber, string firstName, string lastName, Address address, string carType) : base(registrationNumber, firstName, lastName, address)
        {
            CarType = carType;
        }
        
        #pragma warning disable CS8618
        protected Manager() { }
        #pragma warning restore CS8618
    }
}