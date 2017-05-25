using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Plugins.RunProgram;

namespace Schedulr.Plugins.RunRenderingProgram
{
    [Plugin("Run Program On Picture Or Video", "Run a program to modify the file to upload", "Runs a program to modify the picture or video to upload.", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedRendering(RenderingType.Picture)]
    [SupportedRendering(RenderingType.Video)]
    public class RunRenderingProgramPlugin : RenderingPlugin<RunRenderingProgramPluginSettings, RunRenderingProgramPluginSettingsControl>
    {
        #region Constants

        internal const string RenderingInputFileTokenName = "RenderingInputFile";
        internal const string RenderingOutputFileTokenName = "RenderingOutputFile";

        #endregion

        #region Fields

        private string tempInputFile;
        private string tempOutputFile;
        private Stream lastRenderedStreamToDispose;

        #endregion

        protected override RunRenderingProgramPluginSettingsControl GetSettingsControl(RunRenderingProgramPluginSettings settings)
        {
            return new RunRenderingProgramPluginSettingsControl(settings, this.Host);
        }

        public override Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender)
        {
            var renderedFile = fileToRender;
            if ((args.IsVideo && this.Settings.RunOnVideos) || (!args.IsVideo && this.Settings.RunOnPictures))
            {
                if (!string.IsNullOrEmpty(this.Settings.FileName))
                {
                    if (!File.Exists(this.Settings.FileName))
                    {
                        this.Host.Logger.Log("The configured program does not exist: " + this.Settings.FileName, TraceEventType.Warning);
                    }
                    else
                    {
                        // Prepare files to contain the current rendered stream to process (input) and the modified stream (output).
                        this.tempInputFile = FileSystem.GetTempFileName(GetFileExtension(args.Picture.FileName, this.Settings.InputFileExtension));
                        this.tempOutputFile = FileSystem.GetTempFileName(GetFileExtension(args.Picture.FileName, this.Settings.OutputFileExtension));
                        fileToRender.CopyTo(tempInputFile);

                        var tokenValues = new Dictionary<string, object>(args.TemplateTokenValues) { { RenderingInputFileTokenName, tempInputFile }, { RenderingOutputFileTokenName, tempOutputFile } };
                        var arguments = this.Host.ProcessTemplate(this.Settings.Arguments, tokenValues);

                        RunProgramPlugin.RunProgram(this.Settings.FileName, arguments, this.Settings.WorkingDirectory, true, Timeout.Infinite, this.Host.Logger);

                        this.lastRenderedStreamToDispose = File.OpenRead(tempOutputFile);
                        renderedFile = this.lastRenderedStreamToDispose;
                    }
                }
            }

            return renderedFile;
        }

        public override void OnRenderingFileCompleted(RenderingEventArgs args)
        {
            if (this.lastRenderedStreamToDispose != null)
            {
                this.lastRenderedStreamToDispose.Dispose();
                this.lastRenderedStreamToDispose = null;
            }
            if (!string.IsNullOrEmpty(this.tempInputFile))
            {
                File.Delete(this.tempInputFile);
                this.tempInputFile = null;
            }
            if (!string.IsNullOrEmpty(this.tempOutputFile))
            {
                File.Delete(this.tempOutputFile);
                this.tempOutputFile = null;
            }
        }

        internal static string GetFileExtension(string originalFileName, string settingsExtension)
        {
            if (!string.IsNullOrEmpty(settingsExtension))
            {
                return FileSystem.EnsureValidFileExtension(settingsExtension);
            }
            else
            {
                return Path.GetExtension(originalFileName);
            }
        }
    }
}