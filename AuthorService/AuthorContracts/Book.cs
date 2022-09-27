using System.ComponentModel.DataAnnotations;

namespace AuthorContracts;

public class Book
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required] 
    public long ExternalId { get; set; }
    public Author Author { get; set; }
}