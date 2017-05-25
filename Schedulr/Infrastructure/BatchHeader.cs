using JelleDruyts.Windows.ObjectModel;
using Schedulr.Models;

namespace Schedulr.Infrastructure
{
    public class BatchHeader : ObservableObject
    {
        #region Properties

        /// <summary>
        /// Gets the batch.
        /// </summary>
        public Batch Batch { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the long name.
        /// </summary>
        public string LongName
        {
            get { return this.GetValue(LongNameProperty); }
            set { this.SetValue(LongNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="LongName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> LongNameProperty = new ObservableProperty<string, BatchHeader>(o => o.LongName);

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName
        {
            get { return this.GetValue(ShortNameProperty); }
            set { this.SetValue(ShortNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ShortName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ShortNameProperty = new ObservableProperty<string, BatchHeader>(o => o.ShortName);

        #endregion

        #region Constructors

        public BatchHeader(Batch batch, string longName, string shortName)
        {
            this.Batch = batch;
            this.LongName = longName;
            this.ShortName = shortName;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return this.LongName;
        }

        #endregion
    }
}