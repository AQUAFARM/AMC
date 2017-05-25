using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.Delay
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DelayPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the number of seconds to Delay.
        /// </summary>
        [DataMember]
        public int DelaySeconds
        {
            get { return this.GetValue(DelaySecondsProperty); }
            set { this.SetValue(DelaySecondsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DelaySeconds"/> observable property.
        /// </summary>
        public static ObservableProperty<int> DelaySecondsProperty = new ObservableProperty<int, DelayPluginSettings>(o => o.DelaySeconds);

        #endregion
    }
}