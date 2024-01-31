using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Info.Models
{
    public class Opinion
    {
        [Key]
        [Display(Name = "Identyfikator:")]
        public int OpinionId { get; set; }

        [Required(ErrorMessage = "Proszę podać treść komentarza.")]
        [Display(Name = "Treść komentarza:")]
        [DataType(DataType.MultilineText)]
        public string? Comment { get; set; }

        [Display(Name = "Data dodania:")]
        [DataType(DataType.Date, ErrorMessage = "Niepoprawny format daty")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public System.DateTime AddedDate { get; set; }

        [Display(Name = "Ocena tekstu:")]
        public TypeOfGrade? Rating { get; set; }

        [Required]
        [Display(Name = "Komentowany tekst:")]
        public int TextId { get; set; }
        //Komentowany tekst
        [ForeignKey("TextId")]
        public virtual Text? Text { get; set; }

        [Display(Name = "Autor komentarza:")]
        public string? Id { get; set; }
        //Autor komentarza
        [ForeignKey("Id")]
        public virtual AppUser? User { get; set; }
    }
   
    public enum TypeOfGrade
    {
        super = 5,
        dobry = 4,
        przeciętny = 3,
        słaby = 2,
        nieprzydatny = 1,
    }
}
