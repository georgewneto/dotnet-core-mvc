using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.RegrasDeNegocio.Models
{
    public class Cidade
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        // Relacionamento com Estado
        [Required(ErrorMessage = "O Estado é obrigatório")]
        public int EstadoId { get; set; }

        [ForeignKey("EstadoId")]
        public Estado? Estado { get; set; }
    }
}