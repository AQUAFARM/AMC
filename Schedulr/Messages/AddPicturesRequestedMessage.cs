using System.Collections.Generic;
using Schedulr.Models;

namespace Schedulr.Messages
{
    public class AddPicturesRequestedMessage
    {
        public ICollection<string> FileNames { get; private set; }
        public ICollection<Picture> Pictures { get; private set; }
        public bool AddToSingleBatch { get; private set; }

        public AddPicturesRequestedMessage(ICollection<string> fileNames, bool addToSingleBatch)
            : this(fileNames, null, addToSingleBatch)
        {
        }

        public AddPicturesRequestedMessage(ICollection<Picture> pictures, bool addToSingleBatch)
            : this(null, pictures, addToSingleBatch)
        {
        }

        private AddPicturesRequestedMessage(ICollection<string> fileNames, ICollection<Picture> pictures, bool addToSingleBatch)
        {
            this.FileNames = fileNames ?? new string[0];
            this.Pictures = pictures ?? new Picture[0];
            this.AddToSingleBatch = addToSingleBatch;
        }
    }
}