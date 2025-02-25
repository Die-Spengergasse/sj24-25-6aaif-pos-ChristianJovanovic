using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public abstract class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RegistrationNumber { get; set; }
        [MaxLength(48)]
        public string FirstName { get; set; }
        [MaxLength(48)]
        public string LastName { get; set; }
        public Address? Address { get; set; }

        public Employee(int registrationNumber, string firstName, string lastName, Address address)
        {
            RegistrationNumber = registrationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }
        
        #pragma warning disable CS8618
        protected Employee() { }
        #pragma warning restore CS8618
    }
}