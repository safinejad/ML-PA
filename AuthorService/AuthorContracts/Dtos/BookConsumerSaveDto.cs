using Infrastructure;

namespace AuthorContracts.Dtos;

public class BookConsumerSaveDto : IConsumerDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int ExternalAuthorId { get; set; }
}

public class BookConsumerDeleteDto : IConsumerDto
{
    public long ExternalId { get; set; }
}