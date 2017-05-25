using System;
using System.Collections.Generic;
using Schedulr.Models;

namespace Schedulr
{
    /// <summary>
    /// Defines the possible results of an attempt to upload a picture.
    /// </summary>
    public class PictureUploadResult
    {
        #region Properties

        /// <summary>
        /// Gets the picture that was attempted to be uploaded.
        /// </summary>
        public Picture Picture { get; private set; }

        /// <summary>
        /// Gets the date the picture was attempted to be uploaded.
        /// </summary>
        public DateTime? Date { get; private set; }

        /// <summary>
        /// Gets the status of the upload.
        /// </summary>
        public PictureUploadStatus Status { get; private set; }

        /// <summary>
        /// Gets the optional upload steps that were performed and that succeeded.
        /// </summary>
        public IList<PictureUploadOptionalStep> SucceededOptionalSteps { get; private set; }

        /// <summary>
        /// Gets the optional upload steps that were performed but failed (without failing the entire upload).
        /// </summary>
        public IList<PictureUploadOptionalStep> FailedOptionalSteps { get; private set; }

        /// <summary>
        /// Gets a value indicating which error occurred if the upload didn't succeed.
        /// </summary>
        public Exception Error { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureUploadResult"/> class.
        /// </summary>
        /// <param name="picture">The picture that was attempted to be uploaded.</param>
        /// <param name="date">The date the picture was attempted to be uploaded.</param>
        /// <param name="status">The status of the upload.</param>
        /// <param name="succeededOptionalSteps">The optional upload steps that were performed and that succeeded.</param>
        /// <param name="failedOptionalSteps">The optional upload steps that were performed but failed (without failing the entire upload).</param>
        /// <param name="error">A value indicating which error occurred if the upload didn't succeed.</param>
        public PictureUploadResult(Picture picture, DateTime date, PictureUploadStatus status, IList<PictureUploadOptionalStep> succeededOptionalSteps, IList<PictureUploadOptionalStep> failedOptionalSteps, Exception error)
        {
            this.Picture = picture;
            this.Date = date;
            this.Status = status;
            this.SucceededOptionalSteps = succeededOptionalSteps ?? new PictureUploadOptionalStep[0];
            this.FailedOptionalSteps = failedOptionalSteps ?? new PictureUploadOptionalStep[0];
            this.Error = error;
        }

        #endregion
    }
}