using System.ComponentModel.DataAnnotations;

namespace BookContracts.Dtos;

public class BookUpdateDto : BookSaveDto
{
    [Required]
    public int BookId { get; set; }
}