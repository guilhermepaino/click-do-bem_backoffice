using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Campanha
{
    public class CampanhaModel
    {

        [Key]
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "A descrição deve ser informada")]
        [MaxLength(length: 150, ErrorMessage = "A descrição deve conter no máximo 150 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A data de início deve ser informada")]
        public DateTime? DataInicial { get; set; }

        [Required(ErrorMessage = "A data de término deve ser informada")]
        public DateTime? DataFinal { get; set; }

        [Required(ErrorMessage = "A prioridade deve ser informada")]
        public int? Prioridade { get; set; }

        public string Imagem { get; set; }

    }
}
