using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneBookWPF
{
    public class Contact : ICloneable
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu")]
        [StringLength(20, ErrorMessage = "Numer telefonu nie może przekraczać 20 znaków")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        [StringLength(100, ErrorMessage = "Email nie może przekraczać 100 znaków")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Adres nie może przekraczać 100 znaków")]
        public string Address { get; set; }

        [StringLength(500, ErrorMessage = "Notatki nie mogą przekraczać 500 znaków")]
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Contact()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public object Clone()
        {
            return new Contact
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                PhoneNumber = this.PhoneNumber,
                Email = this.Email,
                Address = this.Address,
                Notes = this.Notes,
                CreatedDate = this.CreatedDate,
                ModifiedDate = this.ModifiedDate
            };
        }
        public override string ToString()
        {
            return $"{FirstName} {LastName} ({PhoneNumber})";
        }
    }
}