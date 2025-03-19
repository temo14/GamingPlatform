namespace Domain.Entities
{
    public class Prize
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Hour { get; set; }
        public int Rank { get; set; }
    }
}
