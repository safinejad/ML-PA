namespace BookContracts.Dtos
{
    public class BookGetDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
    public class AuthorGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ExternalId { get; set; }

    }
}