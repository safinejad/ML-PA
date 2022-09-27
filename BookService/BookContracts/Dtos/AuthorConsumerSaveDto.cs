using Infrastructure;

namespace BookContracts.Dtos;

public class AuthorConsumerSaveDto: IConsumerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}