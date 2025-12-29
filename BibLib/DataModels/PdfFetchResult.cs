using BibLib.DataModels.BibDownload;

namespace BibLib.Utils
{
    public static partial class DoiUtils
    {
        /// <summary>
        /// Result for PDF fetch: bytes and best-effort filename.
        /// </summary>
        public sealed record PdfFetchResult
        {
            public byte[] Bytes { get; set; }
            public string FileName { get; set; }
            public BibElement Metadata { get; set; }

            public PdfFetchResult(byte[] bytes, string fileName, BibElement metadata = null)
            {
                Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
                FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
                Metadata = metadata;
            }
        }

    }
}