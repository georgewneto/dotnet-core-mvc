using System.ComponentModel.DataAnnotations;

namespace Projeto.RegrasDeNegocio.Models
{
    public class Estado
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo Nome deve ter no máximo 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo UF é obrigatório")]
        [StringLength(2, ErrorMessage = "O campo UF deve ter 2 caracteres")]
        public string UF { get; set; } = string.Empty;

        // Relacionamento com Cidades
        public ICollection<Cidade>? Cidades { get; set; }
    }
}