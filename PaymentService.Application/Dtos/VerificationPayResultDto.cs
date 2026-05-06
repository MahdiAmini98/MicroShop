using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Dtos
{
    public class VerificationPayResultDto
    {
        public int Status { get; set; }
        public long RefID { get; set; }
    }
}
