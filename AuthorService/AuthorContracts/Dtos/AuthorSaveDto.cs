using System.ComponentModel.DataAnnotations;

namespace AuthorContracts.Dtos;

public class AuthorSaveDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string AuthorGuid { get; set; }
}