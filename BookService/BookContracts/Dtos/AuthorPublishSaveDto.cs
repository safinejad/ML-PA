namespace BookContracts.Dtos
{
    public class BookConsumerSaveDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int ExternalAuthorId { get; set; }
    }
    public class BookConsumerDeleteDto
    {
        public long ExternalId { get; set; }
    }
}