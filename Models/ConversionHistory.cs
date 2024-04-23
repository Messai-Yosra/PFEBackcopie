using System.ComponentModel.DataAnnotations;

namespace stage_api.Models
{
    public class ConversionHistory
    {
        [Key]
        public int? Id { get; set; }
        public string? FileName { get; set; }
        public string? EntityName { get; set; }
        public List<string>? Attributes { get; set; }
        public DateTime? DateCreation { get; set; }
    }
}
