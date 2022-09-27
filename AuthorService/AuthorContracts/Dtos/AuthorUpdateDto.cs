using System.ComponentModel.DataAnnotations;

namespace AuthorContracts.Dtos;

public class AuthorUpdateDto: AuthorSaveDto
{
    [Required]
    public int Id { get; set; }
}