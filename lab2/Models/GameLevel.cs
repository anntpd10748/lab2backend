using System.ComponentModel.DataAnnotations;

namespace ServerGame106.Models
{
    public class GameLevel
    {
        [Key]
        public int LevelID {  get; set; }
        public string title { get; set; }
        public string? Description { get; set; }

    }
}
