using System.ComponentModel.DataAnnotations;

namespace QuestionService.Entities
{
    public class Question
    {
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        [MaxLength(500)]
        public required string Title { get; set; }
        [MaxLength(5000)]

        public required string Context { get; set; }
        public required IList<string> TagSlugs { get; set; }
        [MaxLength(100)]
        public required string AskerId { get; set; }
        [MaxLength(100)]
        public required string AskerName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public int Views { get; set; }
        public bool HasAcceptedAnswer { get; set; }
        public int Votes { get; set; }
    }
}
