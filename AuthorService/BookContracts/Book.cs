using System.ComponentModel.DataAnnotations;

namespace BookContracts
{
    public class Book
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SerialNumber { get; set; }
        public Author Author { get; set; }
    }
}