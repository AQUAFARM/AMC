using System;
using System.Collections.Generic;
using System.Linq;
using Schedulr.Models;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// Provides extensions methods on various types.
    /// </summary>
    public static class SchedulrExtensions
    {
        #region Nullable Extensions

        /// <summary>
        /// Gets the value of the specified <see cref="Nullable{T}"/>, or the specified default value if it has no value.
        /// </summary>
        /// <typeparam name="T">The nullable type.</typeparam>
        /// <param name="value">The nullable value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value of the specified <see cref="Nullable{T}"/>, or the specified default value if it has no value.</returns>
        public static T ValueOr<T>(this Nullable<T> value, T defaultValue) where T : struct
        {
            if (value.HasValue)
            {
                return value.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        #endregion

        #region Picture Extensions

        /// <summary>
        /// Gets the batch for a specified picture.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="batches">The available batches.</param>
        /// <returns>The first batch that contains the specified picture.</returns>
        public static Batch GetBatch(this Picture picture, IEnumerable<Batch> batches)
        {
            if (batches == null)
            {
                return null;
            }
            else
            {
                return batches.Where(b => b.Pictures.Contains(picture)).FirstOrDefault();
            }
        }

        #endregion

        #region Account Extensions

        /// <summary>
        /// Gets the default account.
        /// </summary>
        /// <param name="accounts">The available accounts.</param>
        /// <returns>The first account that has the <see cref="Account.IsDefaultAccount"/> property set.</returns>
        public static Account GetDefaultAccount(this IList<Account> accounts)
        {
            return accounts.Where(a => a.IsDefaultAccount).FirstOrDefault();
        }

        #endregion
    }
}