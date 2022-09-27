using System.ComponentModel.DataAnnotations;

namespace BookContracts
{

    public class Author
    {
        [Key] [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int ExternalId { get; set; }

    }
}