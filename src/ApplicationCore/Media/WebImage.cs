using System;

namespace ApplicationCore.Media
{
    public class WebImage
    {
        public WebImage(string format, byte[] contents)
        {
            Format = format;
            Contents = contents;
        }

        /// <summary>
        /// The format to be used in &lt;img&gt; tags (e.g. 'png', 'jpeg')
        /// </summary>
        public string Format { get; }

        public byte[] Contents { get; }

        /// <summary>
        /// The full string to be used in &lt;img&gt; source attributes
        /// </summary>
        public string Source => $"data:image/{Format};base64,{Convert.ToBase64String(Contents)}";
    }
}
