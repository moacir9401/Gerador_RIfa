using System.ComponentModel.DataAnnotations;

namespace Rifa.Models
{
    public class Rifas
    {
        public string Img { get; set; }
        public int CodigoSorteio { get; set; }
        [Display(Name = "Título")]
        public string Titulo { get; set; }
        [Display(Name = "Prêmio")]
        public string Premio { get; set; }
        [Display(Name = "Rodapé")]
        public string Rodape { get; set; }
         
        public Decimal Valor { get; set; }

    }
}
