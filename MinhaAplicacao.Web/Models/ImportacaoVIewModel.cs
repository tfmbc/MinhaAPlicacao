using System.ComponentModel.DataAnnotations;
namespace MinhaAplicacao.Web.Models
{
    public class ImportacaoVIewModel
    {
        [Required(ErrorMessage = "Favor Selecionar arquivos para importação.")]
        [Display(Name = "Escolher Arquivos")]
        public IFormFile[]? files { get; set; }
    }
}
