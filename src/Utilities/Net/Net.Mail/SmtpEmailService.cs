using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Options;

namespace Riverside.Cms.Utilities.Net.Mail
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IOptions<EmailOptions> _options;

        public SmtpEmailService(IOptions<EmailOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// Converts a string like "Mike (testtesttest@example.com)" into a display name and email address ("Mike", "testtesttest@example.com").
        /// If string is just an email address like "testtesttest@example.com", then this is returned as the email address with display name null.
        /// </summary>
        /// <param name="address">A string like "Mike (testtesttest@example.com)".</param>
        /// <returns>An email address object populated with display name and email.</returns>
        private EmailAddress GetAddress(string address)
        {
            // Get rid of any leading or trailing white space
            address = address.Trim();

            // Find the positions of the opening and closing brackets
            int startIndex = address.IndexOf('(');
            int stopIndex = startIndex != -1 ? address.IndexOf(')', startIndex) : -1;
            bool bracketsFound = startIndex != -1 && stopIndex != -1;

            // Email address will be between brackets or the entire string
            string email = bracketsFound ? address.Substring(startIndex + 1, stopIndex - startIndex - 1).Trim() : address;

            // Display name is everything before the first opening bracket or null if no brackets found
            string displayName = bracketsFound ? address.Substring(0, startIndex).Trim() : null;

            // Return result
            return new EmailAddress { DisplayName = displayName, Email = email };
        }

        private IEnumerable<EmailAddress> ListAddresses(string addresses)
        {
            string[] addressesArray = addresses.Split(';');
            foreach (string address in addressesArray)
                yield return GetAddress(address);
        }

        public EmailAddress GetFromEmailRecipient()
        {
            if (string.IsNullOrWhiteSpace(_options.Value.EmailFromAddress))
                return null;
            return GetAddress(_options.Value.EmailFromAddress);
        }

        public EmailAddress GetReplyToEmailRecipient()
        {
            if (string.IsNullOrWhiteSpace(_options.Value.EmailReplyToAddress))
                return null;
            return GetAddress(_options.Value.EmailReplyToAddress);
        }

        public IEnumerable<EmailAddress> ListBccEmailAddresses()
        {
            if (string.IsNullOrWhiteSpace(_options.Value.EmailBccAddresses))
                return Enumerable.Empty<EmailAddress>();
            return ListAddresses(_options.Value.EmailBccAddresses);
        }

        public IEnumerable<EmailAddress> ListToEmailAddresses()
        {
            if (string.IsNullOrWhiteSpace(_options.Value.EmailToAddresses))
                return Enumerable.Empty<EmailAddress>();
            return ListAddresses(_options.Value.EmailToAddresses);
        }

        public void Send(Email email)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.Host = _options.Value.EmailHost;
                if (_options.Value.EmailPort.HasValue)
                    client.Port = _options.Value.EmailPort.Value;
                if (_options.Value.EmailUsername != null && _options.Value.EmailPassword != null)
                    client.Credentials = new NetworkCredential(_options.Value.EmailUsername, _options.Value.EmailPassword);
                using (MailMessage mailMessage = new MailMessage())
                {
                    if (email.ReplyToAddress != null)
                        mailMessage.ReplyToList.Add(new MailAddress(email.ReplyToAddress.Email, email.ReplyToAddress.DisplayName));
                    mailMessage.From = new MailAddress(email.FromAddress.Email, email.FromAddress.DisplayName);
                    mailMessage.Subject = email.Content.Subject;
                    mailMessage.Body = email.Content.PlainTextBody != null ? email.Content.PlainTextBody : email.Content.HtmlBody;
                    mailMessage.IsBodyHtml = email.Content.PlainTextBody == null;
                    if (email.Content.PlainTextBody != null && email.Content.HtmlBody != null)
                    {
                        ContentType mimeType = new ContentType("text/html");
                        AlternateView alternate = AlternateView.CreateAlternateViewFromString(email.Content.HtmlBody, mimeType);
                        mailMessage.AlternateViews.Add(alternate);
                    }
                    foreach (EmailAddress toAddress in email.ToAddresses)
                        mailMessage.To.Add(new MailAddress(toAddress.Email, toAddress.DisplayName));
                    if (email.BccAddresses != null)
                        foreach (EmailAddress bccAddress in email.BccAddresses)
                            mailMessage.Bcc.Add(new MailAddress(bccAddress.Email, bccAddress.DisplayName));
                    client.Send(mailMessage);
                }
            }
        }
    }
}
