using System;

namespace SantaHelena.ClickDoBem.BackOffice.Exceptions
{
    public class SessaoExpiradaException : Exception
    {

        public SessaoExpiradaException() : base("A sessão expirou") { }

    }
}
