using System.ComponentModel.DataAnnotations;

namespace QuestionService.Entities
{
    public class Tag
    {
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public required string Slug { get; set; }

        [MaxLength(1000)]
        public required string Description { get; set; }

        public int UsageCount { get; set; }
    }
}
