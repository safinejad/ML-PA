using System.ComponentModel.DataAnnotations;

namespace BookContracts.Dtos
{

    public class BookSaveDto
    {
        [Required] public string Name { get; set; }
        [Required] public string SerialNumber { get; set; }
        [Required] public int ExternalAuthorId { get; set; }
    }
}