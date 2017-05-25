using System;
using System.IO;
using System.Runtime.InteropServices;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Provides helper methods for files.
    /// </summary>
    public static class FileSystem
    {
        #region Constants

        private const string BlockedFileAlternateDataStreamName = "Zone.Identifier";
        private const char AlternateDataStreamSeparator = ':';
        private const int MaxPathLength = 256;
        private const string LongPathPrefix = @"\\?\";

        #endregion

        #region GetTempFileName

        /// <summary>
        /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
        /// </summary>
        /// <returns>A <see cref="System.String"/> containing the full path of the temporary file.</returns>
        /// <exception cref="System.IO.IOException">An I/O error occurs, such as no unique temporary file name is available.-or -This method was unable to create a temporary file.</exception>
        public static string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        /// <summary>
        /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
        /// </summary>
        /// <returns>A <see cref="System.String"/> containing the full path of the temporary file.</returns>
        /// <param name="extension">The file extension for the temporary file.</param>
        /// <exception cref="System.IO.IOException">An I/O error occurs, such as no unique temporary file name is available.-or -This method was unable to create a temporary file.</exception>
        public static string GetTempFileName(string extension)
        {
            extension = EnsureValidFileExtension(extension);
            var tempPath = Path.GetTempPath();
            while (true)
            {
                var tempFileName = Path.Combine(tempPath, Guid.NewGuid().ToString() + extension);
                try
                {
                    using (var file = new FileStream(tempFileName, FileMode.CreateNew))
                    {
                        return tempFileName;
                    }
                }
                catch (IOException)
                {
                    // The file already exists, keep trying.
                }
            }
        }

        #endregion

        #region EnsureValidFileExtension

        /// <summary>
        /// Returns a valid file extension (including the leading dot) for the specified file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The file extension with a leading dot, or the original extension if it was <see langword="null"/> or empty.</returns>
        public static string EnsureValidFileExtension(string extension)
        {
            if (!string.IsNullOrEmpty(extension) && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return extension;
        }

        #endregion

        #region Blocked Files

        /// <summary>
        /// Determines if the specified file has been blocked by Windows because it comes from an untrusted source (e.g. it was downloaded).
        /// </summary>
        /// <param name="fileName">The full path of the file to verify.</param>
        /// <returns><see langword="true"/> if the file has been blocked by Windows, <see langword="false"/> otherwise.</returns>
        public static bool IsBlockedFile(string fileName)
        {
            var adsPath = BuildStreamPath(fileName, BlockedFileAlternateDataStreamName);
            bool exists = -1 != NativeMethods.GetFileAttributes(adsPath);
            return exists;
        }

        /// <summary>
        /// Unblocks the specified file if it has been blocked by Windows because it comes from an untrusted source (e.g. it was downloaded).
        /// </summary>
        /// <param name="fileName">The full path of the file to unblock.</param>
        /// <returns><see langword="true"/> if the file was unblocked, <see langword="false"/> otherwise (e.g. if the file did not exist or was not blocked).</returns>
        public static bool UnblockFile(string fileName)
        {
            var adsPath = BuildStreamPath(fileName, BlockedFileAlternateDataStreamName);
            return NativeMethods.DeleteFile(adsPath);
        }

        private static string BuildStreamPath(string fileName, string streamName)
        {
            string result = fileName;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (result.Length == 1)
                {
                    result = ".\\" + result;
                }
                result += AlternateDataStreamSeparator + streamName + AlternateDataStreamSeparator + "$DATA";
                if (result.Length >= MaxPathLength)
                {
                    result = LongPathPrefix + result;
                }
            }
            return result;
        }

        private sealed class NativeMethods
        {
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int GetFileAttributes(string fileName);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteFile(string name);
        }

        #endregion
    }
}