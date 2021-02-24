using System.Threading.Tasks;

namespace TVProgViewer.Services.Common
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public partial interface IAddressAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        Task<string> FormatAttributesAsync(string attributesXml,
            string separator = "<br />",
            bool htmlEncode = true);
    }
}
