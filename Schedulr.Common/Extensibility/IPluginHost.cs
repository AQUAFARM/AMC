using System.Collections.Generic;
using JelleDruyts.Windows;
using JelleDruyts.Windows.Text;
using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents the host for plugins.
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets information about the application.
        /// </summary>
        ApplicationInfo GetApplicationInfo();

        /// <summary>
        /// Registers an application task that is executing.
        /// </summary>
        /// <param name="task">The task to register</param>
        void RegisterApplicationTask(ApplicationTask task);

        /// <summary>
        /// Adds the specified pictures to the upload queue asynchronously.
        /// </summary>
        /// <param name="account">The account for which to add the pictures.</param>
        /// <param name="fileNames">The file names of the pictures to load and add.</param>
        /// <param name="pictures">The pictures to add.</param>
        /// <param name="addToSingleBatch">A value that determines if all the pictures should be added to a single batch or if each should go in its own batch.</param>
        void AddPicturesToQueue(Account account, ICollection<string> fileNames, ICollection<Picture> pictures, bool addToSingleBatch);

        /// <summary>
        /// Uploads the specified pictures asynchronously.
        /// </summary>
        /// <param name="account">The account for which to upload the pictures.</param>
        /// <param name="pictures">The pictures to upload.</param>
        /// <returns><see langword="true"/> if the pictures are being uploaded in the background upon return, <see langword="false"/> otherwise.</returns>
        bool UploadPictures(Account account, ICollection<Picture> pictures);

        /// <summary>
        /// Uploads the specified batch of pictures asynchronously.
        /// </summary>
        /// <param name="account">The account for which to upload the batch.</param>
        /// <param name="batch">The batch to upload.</param>
        /// <returns><see langword="true"/> if the batch is being uploaded in the background upon return, <see langword="false"/> otherwise.</returns>
        bool UploadBatch(Account account, Batch batch);

        /// <summary>
        /// Allows the user to edit a text template.
        /// </summary>
        /// <param name="template">The text template to edit.</param>
        /// <param name="title">The dialog title.</param>
        /// <returns>The modified text template to use.</returns>
        string ShowTemplateEditorDialog(string template, string title);

        /// <summary>
        /// Allows the user to edit a text template.
        /// </summary>
        /// <param name="template">The text template to edit.</param>
        /// <param name="title">The dialog title.</param>
        /// <param name="additionalTokens">Defines any additional tokens that are supported apart from the built-in ones.</param>
        /// <returns>The modified text template to use.</returns>
        string ShowTemplateEditorDialog(string template, string title, IEnumerable<TemplateTokenInfo> additionalTokens);

        /// <summary>
        /// Processes a text template by replacing all tokens with their values.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <param name="tokenValues">The dictionary of token names with their values.</param>
        /// <returns>The template, with all tokens replaced by their token values.</returns>
        string ProcessTemplate(string template, IDictionary<string, object> tokenValues);
    }
}