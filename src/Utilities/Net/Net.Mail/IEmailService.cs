using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Net.Mail
{
    public interface IEmailService
    {
        EmailAddress GetFromEmailRecipient();
        EmailAddress GetReplyToEmailRecipient();
        IEnumerable<EmailAddress> ListBccEmailAddresses();
        IEnumerable<EmailAddress> ListToEmailAddresses();
        void Send(Email email);
    }
}
