using System.ComponentModel.DataAnnotations;

namespace ArticleService.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Artikel code is verplicht.")]
        [MaxLength(50, ErrorMessage = "Artikel code mag maximaal 50 karakters lang zijn.")]
        public string ArticleCode { get; set; }
        public string Description { get; set; }
    }
}
