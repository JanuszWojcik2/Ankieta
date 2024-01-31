using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Info.Models
{
    public class Text
    {
        [Key]
        [Display(Name = "Identyfikator:")]
        public int TextId { get; set; }

        [Required]
        [Display(Name = "Tytuł tekstu:")]
        [MaxLength(75, ErrorMessage = "Tytuł tekstu nie może przekraczać 75 znaków")]
        public string? Title { get; set; }

        [Required]
        [Display(Name = "Streszczenie tekstu:")]
        [MaxLength(255, ErrorMessage = "Streszczenie nie może przekraczać 255 znaków")]
        [DataType(DataType.MultilineText)]
        public string? Summary { get; set; }

        [Display(Name = "Słowa kluczowe:")]
        [MaxLength(255, ErrorMessage = "Słowa kluczowe nie mogą być dłuższe niż 255 znaków")]
        public string? Keywords { get; set; }

        [Required]
        [Display(Name = "Treść tekstu:")]
        public string? Content { get; set; }

        [Display(Name = "Grafika do tekstu:")]
        [MaxLength(128)]
        [FileExtensions(Extensions = ". jpg,. png,. gif", ErrorMessage = "Niepoprawne rozszerzenie pliku.")]
        public string? Graphic { get; set; }

        [Required]
        [Display(Name = "Czy wyświetlać?")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Data dodania:")]
        [DataType(DataType.Date, ErrorMessage = "Niepoprawny format daty")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public System.DateTime AddedDate { get; set; }

        [Display(Name = "Kategoria tekstu:")]
        public int CategoryId { get; set; }
        //Kategoria tekstu
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Autor tekstu:")]
        public string? Id { get; set; }
        //Autor tekstu
        [ForeignKey("Id")]
        public virtual AppUser? User { get; set; }

        //lista wszystkich komentarzy tekstu
        public virtual List<Opinion>? Opinions { get; set; }
    }
}
