using System.IO;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Provides extension methods for the <see cref="Stream"/> type.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Copies a stream to a file.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="fileName">The name of the file.</param>
        public static void CopyTo(this Stream source, string fileName)
        {
            using (var destination = File.Create(fileName))
            {
                source.CopyTo(destination);
                destination.Flush();
            }
        }

        /// <summary>
        /// Copies a stream to another stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        public static void CopyTo(this Stream source, Stream destination)
        {
            if (source == null)
            {
                throw new System.ArgumentNullException("source");
            }
            if (destination == null)
            {
                throw new System.ArgumentNullException("destination");
            }
            int readCount;
            var buffer = new byte[8192];
            while ((readCount = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, readCount);
            }
        }
    }
}