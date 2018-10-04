using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Details { get; set; }

    }
}