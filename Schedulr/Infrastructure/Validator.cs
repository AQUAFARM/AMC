using Schedulr.Models;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// Validates model instances.
    /// </summary>
    public static class Validator
    {
        #region Picture

        /// <summary>
        /// Determines whether the specified picture is valid.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="account">The account.</param>
        /// <returns><see langword="true"/> if the picture is valid, <see langword="false"/> otherwise.</returns>
        public static bool IsPictureValid(Picture picture, Account account)
        {
            return IsPictureFileSizeValid(picture, account);
        }

        /// <summary>
        /// Determines whether the specified picture file size is valid.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="account">The account.</param>
        /// <returns><see langword="true"/> if the picture file size is valid, <see langword="false"/> otherwise.</returns>
        public static bool IsPictureFileSizeValid(Picture picture, Account account)
        {
            if (account != null)
            {
                var maxFileSize = (App.IsVideoFile(picture.FileName) ? account.UserInfo.UploadLimitVideo : account.UserInfo.UploadLimit);
                if (picture.FileSize > maxFileSize)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}