using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.ApiDto
{

    /// <summary>
    /// Modelo de request de efetivação de match
    /// </summary>
    public class EfetivarMatchRequest
    {

        /// <summary>
        /// Id do match
        /// </summary>
        public Guid? Id { get; set; }

    }

}
