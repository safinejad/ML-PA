namespace AuthorContracts.Dtos;

public class BookGetDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long ExternalId { get; set; }
    public string AuthorName { get; set; }
    public int AuthorId { get; set; }

}