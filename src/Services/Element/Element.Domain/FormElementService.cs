using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Utilities.Net.Mail;

namespace Riverside.Cms.Services.Element.Domain
{
    public enum FormFieldType
    {
        TextField,
        MultiLineTextField
    }

    public class FormField
    {
        public long FormFieldId { get; set; }
        public string Label { get; set; }
        public FormFieldType FieldType { get; set; }
        public bool Required { get; set; }
    }

    public class FormElementSettings : ElementSettings
    {
        public string RecipientEmail { get; set; }
        public string SubmitButtonLabel { get; set; }
        public string SubmittedMessage { get; set; }
        public IEnumerable<FormField> Fields { get; set; }
    }

    public class FormFieldValue
    {
        public long FormFieldId { get; set; }
        public string Value { get; set; }
    }

    public class FormElementActionRequest
    {
        public IEnumerable<FormFieldValue> Fields { get; set; }
    }

    public class FormFieldLabelValue
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class FormElementActionResponse
    {
        public string Message { get; set; }
    }

    public interface IFormElementService : IElementSettingsService<FormElementSettings>, IElementViewService<FormElementSettings, object>, IElementActionService<FormElementActionRequest, FormElementActionResponse>
    {
    }

    public class FormElementService : IFormElementService
    {
        private readonly IDomainService _domainService;
        private readonly IElementRepository<FormElementSettings> _elementRepository;
        private readonly IEmailService _emailService;
        private readonly IPageService _pageService;

        public FormElementService(IDomainService domainService, IElementRepository<FormElementSettings> elementRepository, IEmailService emailService, IPageService pageService)
        {
            _domainService = domainService;
            _elementRepository = elementRepository;
            _emailService = emailService;
            _pageService = pageService;
        }

        public Task<FormElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<FormElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            FormElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            return new ElementView<FormElementSettings, object>
            {
                Settings = settings,
                Content = null
            };
        }

        /// <summary>
        /// Converts email addresses in a text string into a list of strongly typed emails.
        /// Credit: http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net (see Guffa's answer).
        /// </summary>
        /// <param name="separators">Separators used to split text string of tags.</param>
        /// <param name="recipientEmail">Text string containing email addresses.</param>
        /// <returns>Collection of email addresses.</returns>
        private IEnumerable<EmailAddress> GetEmailAddresses(string[] separators, string recipientEmail)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
                return new List<EmailAddress>();
            return recipientEmail
                .Split(separators, StringSplitOptions.None)
                .Select(e => e.ToLower().Trim()).Distinct()
                .Where(e => e != string.Empty)
                .Select(e => new EmailAddress { Email = e });
        }

        private string GetHost(string url)
        {
            return url.Replace("http://", string.Empty).Replace("https://", string.Empty);
        }

        private Email GetEmail(WebDomain domain, Page page, FormElementSettings settings, EmailContent content)
        {
            // Get to recipients
            IEnumerable<EmailAddress> toAddresses = GetEmailAddresses(new string[] { "\r\n", "\n" }, settings.RecipientEmail);
            IEnumerable<EmailAddress> configurationToAddresses = _emailService.ListToEmailAddresses();
            if (configurationToAddresses != null)
                toAddresses.Concat(configurationToAddresses);

            // Get from (and reply to) email address
            string host = GetHost(domain.Url);
            EmailAddress fromEmailAddress = new EmailAddress
            {
                Email = $"donotreply@{host}",
                DisplayName = null
            };

            // Return email to send
            return new Email
            {
                BccAddresses = _emailService.ListBccEmailAddresses(),
                Content = content,
                FromAddress = fromEmailAddress,
                ReplyToAddress = fromEmailAddress,
                ToAddresses = toAddresses,
            };
        }

        private IEnumerable<FormFieldLabelValue> GetFormFieldLabelValues(FormElementSettings settings, FormElementActionRequest request)
        {
            foreach (FormField field in settings.Fields)
            {
                FormFieldValue value = request.Fields.Where(f => f.FormFieldId == field.FormFieldId).FirstOrDefault();
                if (value != null)
                {
                    yield return new FormFieldLabelValue
                    {
                        Label = field.Label,
                        Value = value.Value
                    };
                }
            }
        }

        private string FormatMultiLine(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br />");
        }

        private string GetHtmlEmailContent(Page page, IEnumerable<FormFieldLabelValue> labelValues)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("h1 { margin: 20px 0 10px 0; padding: 0; font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif; font-size: 36px; line-height: 1.1; color: #333333; }");
            sb.AppendLine("h2 { margin: 20px 0 10px 0; padding: 0; font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif; font-size: 30px; line-height: 1.1; color: #333333; }");
            sb.AppendLine("p { margin: 10px 0 10px 0; padding: 0; width: 100%; font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif; font-size: 14px; line-height: 1.4; color: #333333; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendFormat("<h1>{0}</h1>", WebUtility.HtmlEncode($"The following data has been submitted at {page.Name}"));
            foreach (FormFieldLabelValue labelValue in labelValues)
            {
                sb.AppendLine("<p>");
                sb.AppendLine("<strong>" + WebUtility.HtmlEncode(labelValue.Label) + ":</strong><br>");
                sb.AppendLine(FormatMultiLine(WebUtility.HtmlEncode(labelValue.Value ?? string.Empty)));
                sb.AppendLine("</p>");
            }
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        private string GetPlainTextEmailContent(Page page, IEnumerable<FormFieldLabelValue> labelValues)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"The following data has been submitted at {page.Name}");            
            foreach (FormFieldLabelValue labelValue in labelValues)
            {
                sb.AppendLine();
                sb.AppendLine(labelValue.Label + ":");
                sb.AppendLine(labelValue.Value ?? string.Empty);
            }
            return sb.ToString();
        }

        private EmailContent GetEmailContent(Page page, IEnumerable<FormFieldLabelValue> labelValues)
        {
            return new EmailContent
            {
                Subject = $"Form submission ({page.Name})",
                HtmlBody = GetHtmlEmailContent(page, labelValues),
                PlainTextBody = GetPlainTextEmailContent(page, labelValues)
            };
        }

        public async Task<FormElementActionResponse> PerformElementActionAsync(long tenantId, long elementId, FormElementActionRequest request, PageContext context)
        {
            // Get form settings, domain and current page details
            FormElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            Page page = await _pageService.ReadPageAsync(tenantId, context.PageId);
            WebDomain domain = await _domainService.ReadDomainByRedirectUrlAsync(tenantId, null);

            // Construct email
            IEnumerable<FormFieldLabelValue> labelValues = GetFormFieldLabelValues(settings, request);
            EmailContent content = GetEmailContent(page, labelValues);
            Email email = GetEmail(domain, page, settings, content);

            // Send finished email
            _emailService.Send(email);

            // Return response
            return new FormElementActionResponse
            {
                Message = settings.SubmittedMessage
            };
        }
    }
}
