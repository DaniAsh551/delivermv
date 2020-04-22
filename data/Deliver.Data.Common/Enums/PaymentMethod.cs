using System;
using System.Collections.Generic;
using System.Text;

namespace Deliver.Data.Common
{
    [Flags]
    public enum PaymentMethod
    {
        CashOnDelivery = 0,
        BankTransferBml = 1,
        BankTransferMib = 2,
        CreditCardBml = 3,
        CreditCardMib = 4,
    }
}
