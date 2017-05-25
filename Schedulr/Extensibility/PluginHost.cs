using System.Collections.Generic;
using JelleDruyts.Windows;
using JelleDruyts.Windows.Text;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;

namespace Schedulr.Extensibility
{
    public class PluginHost : IPluginHost
    {
        public PluginInstance Plugin { get; private set; }
        public ILogger Logger { get; private set; }

        public PluginHost(PluginInstance plugin)
        {
            this.Plugin = plugin;
            this.Logger = new PluginLogger(this.Plugin);
        }

        public ApplicationInfo GetApplicationInfo()
        {
            return App.Info;
        }

        public void RegisterApplicationTask(ApplicationTask task)
        {
            Messenger.Send<TaskStatusMessage>(new TaskStatusMessage(task));
        }

        public void AddPicturesToQueue(Account account, ICollection<string> fileNames, ICollection<Picture> pictures, bool addToSingleBatch)
        {
            Tasks.AddPicturesToQueue(account, fileNames, pictures, addToSingleBatch);
        }

        public bool UploadPictures(Account account, ICollection<Picture> pictures)
        {
            return Tasks.UploadPictures(account, pictures, null);
        }

        public bool UploadBatch(Account account, Batch batch)
        {
            return Tasks.UploadBatch(account, batch, null);
        }

        public string ShowTemplateEditorDialog(string template, string title)
        {
            return ShowTemplateEditorDialog(template, title, null);
        }

        public string ShowTemplateEditorDialog(string template, string title, IEnumerable<TemplateTokenInfo> additionalTokens)
        {
            var dialog = new TemplateEditorForm();
            dialog.Text = title;
            dialog.Template = template;
            var availableTokens = new List<TemplateTokenInfo>();
            if (additionalTokens != null)
            {
                availableTokens.AddRange(additionalTokens);
            }
            var collectionTokens = this.Plugin.Collection.GetTemplateTokens();
            if (collectionTokens != null)
            {
                availableTokens.AddRange(collectionTokens);
            }
            dialog.AvailableTokens = availableTokens;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                template = dialog.Template;
            }
            return template;
        }

        public string ProcessTemplate(string template, IDictionary<string, object> tokenValues)
        {
            return TemplateProcessor.Process(template, tokenValues);
        }
    }
}