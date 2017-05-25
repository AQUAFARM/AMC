using System.Linq;
using System.Windows;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.DragDrop;
using Schedulr.Messages;
using Schedulr.ViewModels;

namespace Schedulr.Infrastructure
{
    public class SchedulrDragDropHandler : DefaultDropHandler
    {
        public override void DragOver(DropInfo dropInfo)
        {
            base.DragOver(dropInfo);

            // Check if we have a file drop first. If so, indicate a copy operation and skip the "internal" drag & drop.
            var dataObject = dropInfo.Data as IDataObject;
            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = dataObject.GetData(DataFormats.FileDrop) as string[];
                var dropCount = fileNames.Length;

                // If ALT is pressed, indicate that all pictures will be added to one single batch by using a "Link" icon.
                var message = dropCount.ToCountString("file", "Adding ");
                if (dropCount > 1 && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                {
                    dropInfo.Effects = DragDropEffects.Link;
                    message += " as a single batch";
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                    if (dropCount > 1)
                    {
                        message += " as individual batches (hold ALT to add as a single batch)";
                    }
                }
                DropTargetToolTipAdorner.ToolTipMessage = message;
                dropInfo.DropTargetAdorner = DropTargetAdorners.ToolTip;
            }
            else
            {
                if (CanAcceptData(dropInfo))
                {
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
        }

        public override void Drop(DropInfo dropInfo)
        {
            // Check if we have a file drop first. If so, import the files and skip the "internal" drag & drop.
            var dataObject = dropInfo.Data as IDataObject;
            if (dataObject != null)
            {
                var fileNames = dataObject.GetData(DataFormats.FileDrop) as string[];
                if (fileNames != null && fileNames.Length > 0)
                {
                    // If ALT is pressed, add all pictures to one single batch.
                    var addToSingleBatch = false;
                    if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                    {
                        addToSingleBatch = true;
                    }

                    Messenger.Send<AddPicturesRequestedMessage>(new AddPicturesRequestedMessage(fileNames, addToSingleBatch));
                }
            }
            else
            {
                base.Drop(dropInfo);

                if (dropInfo.TargetGroup != null)
                {
                    var selectedPictures = ExtractData(dropInfo.Data).Cast<PictureViewModel>().Select(p => p.Picture);
                    var pictureList = dropInfo.TargetCollection.Cast<PictureViewModel>().First().PictureList;
                    var batch = ((BatchHeader)dropInfo.TargetGroup.Name).Batch;
                    Tasks.ChangeBatch(selectedPictures, pictureList, batch, dropInfo.NewGroupRequestedBefore, dropInfo.NewGroupRequestedAfter);
                }
            }
        }
    }
}