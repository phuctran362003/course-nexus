using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class ConfirmPaymentDto
    {
        public int OrderId { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionId { get; set; }
    }
}
