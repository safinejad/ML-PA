using System.ComponentModel.DataAnnotations;

namespace AuthorContracts
{
    public class Author
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string AuthorGuid { get; set; }
    }
}