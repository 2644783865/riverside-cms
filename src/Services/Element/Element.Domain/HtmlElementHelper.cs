using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Riverside.Cms.Services.Element.Domain
{
    public class HtmlElementHelper
    {
        private readonly IDictionary<long, HtmlPreviewImage> _previewImagesByBlobSetId;

        public HtmlElementHelper(IDictionary<long, HtmlPreviewImage> previewImagesByBlobSetId)
        {
            _previewImagesByBlobSetId = previewImagesByBlobSetId;
        }

        /// <summary>
        /// Gets attribute value from an HTML element.
        /// </summary>
        /// <param name="html">Html, e.g. "<img src='/elements/775/uploads/397?t=636576020049621023' width='45' height='56' alt='Car photo' />"</param>
        /// <param name="attribute">Name of attribute to retrieve, e.g. "width".</param>
        /// <returns>Attribute value.</returns>
        private string GetAttribute(string html, string attribute)
        {
            // Find beginning of attribute
            string attributeStartText = $"{attribute}=\"";
            int attributeStartIndex = html.IndexOf(attributeStartText);
            if (attributeStartIndex == -1)
                return null;

            // Find end of attribute value
            string attributeStopText = "\"";
            int attributeStopIndex = html.IndexOf(attributeStopText, attributeStartIndex + attributeStartText.Length);
            if (attributeStopIndex == -1)
                return null;

            // Return attribute value that is between double quotes
            return html.Substring(attributeStartIndex + attributeStartText.Length, attributeStopIndex - attributeStartIndex - attributeStartText.Length);
        }

        /// <summary>
        /// Converts HTML image tag details into an JSON object.
        /// </summary>
        /// <param name="imageHtml">Html, e.g. "<img src='/elements/775/uploads/397?t=636576020049621023' width='45' height='56' alt='Car photo' />"</param>
        /// <returns>JSON representation of object within opening and closing double square brackets.</returns>
        public string Replace(string imageHtml)
        {
            // Get HTML image tag attributes
            string src = GetAttribute(imageHtml, "src");
            string width = GetAttribute(imageHtml, "width");
            string height = GetAttribute(imageHtml, "height");
            string alt = GetAttribute(imageHtml, "alt");

            // At a minimum we need src attribute to determine blob identifier
            if (src == null)
                return "[[{}]]";

            // Src attribute determines HTML blob identifier
            string[] srcParts = src.Split('/');
            if (srcParts.Length < 5)
                return "[[{}]]";
            string blobSetIdAndQueryString = srcParts[4];
            int indexOfQueryString = blobSetIdAndQueryString.IndexOf("?");
            if (indexOfQueryString == -1)
                return "[[{}]]";
            string blobSetIdText = blobSetIdAndQueryString.Substring(0, indexOfQueryString);
            long blobSetId;
            if (!Int64.TryParse(blobSetIdText, out blobSetId))
                return "[[{}]]";

            // Check that image with HTML blob identifier exists
            if (!_previewImagesByBlobSetId.ContainsKey(blobSetId) || _previewImagesByBlobSetId[blobSetId] == null)
                return "[[{}]]";

            // Finally, encode image HTML into JSON
            HtmlPreviewImageOverride image = new HtmlPreviewImageOverride
            {
                BlobSetId = blobSetId,
                Name = alt,
                Width = width,
                Height = height
            };
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };
            return $"[[{JsonConvert.SerializeObject(image, serializerSettings)}]]";
        }
    }
}
