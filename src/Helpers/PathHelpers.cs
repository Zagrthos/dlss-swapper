using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLSS_Swapper.Helpers
{
    internal static class PathHelpers
    {
        public static char[] InvalidFileNamePathChars { get; private set; }

        static PathHelpers()
        {
            var invalidChars = new List<char>();
            invalidChars.AddRange(Path.GetInvalidFileNameChars());
            invalidChars.AddRange(Path.GetInvalidPathChars());
            invalidChars.Add('.');
            InvalidFileNamePathChars = invalidChars.Distinct().ToArray();
        }

        /// <summary>
        /// Checks if a path is read-only
        /// </summary>
        /// <param name="path"></param>
        /// <returns><see langword="true"/> if the directory is read-only, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the path is null or whitespace.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not even exist.</exception>
        internal static bool CheckIfPathIsReadOnly(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory does not exist: {path}");
            }

            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Attributes.HasFlag(FileAttributes.ReadOnly))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a user has write permissions to a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns><see langword="true"/> if the user is able to write, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the path is null or whitespace.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not even exist.</exception>
        internal static bool CheckIfUserHasWritePermissions(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory does not exist: {path}");
            }

            try
            {
                string tempFilePath = Path.Combine(path, Path.GetRandomFileName());
                _ = File.Create(tempFilePath, 1, FileOptions.DeleteOnClose);
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to format path on disk so any and all paths will match after they have gone through this method.  
        /// </summary>
        /// <param name="path">Path on local disk</param>
        /// <returns>Formatted path on disk</returns>
        internal static string NormalizePath(string path)
        {
            // Via https://stackoverflow.com/a/21058152
            //new Uri(path).LocalPath
            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

    }
}
