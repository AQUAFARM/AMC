using System.Collections.Generic;
using System.ComponentModel;

namespace Schedulr.Test
{
    internal class PropertyChangedListener
    {
        public IList<PropertyChangedEventArgs> RaisedPropertyChangedEventArgs { get; private set; }

        public PropertyChangedListener(INotifyPropertyChanged obj)
        {
            this.RaisedPropertyChangedEventArgs = new List<PropertyChangedEventArgs>();
            obj.PropertyChanged += (sender, e) => this.RaisedPropertyChangedEventArgs.Add(e);
        }
    }
}