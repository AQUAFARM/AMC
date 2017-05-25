using System.Collections.Generic;
using System.ComponentModel;
using JelleDruyts.Windows.ObjectModel;

namespace JelleDruyts.Windows.Test
{
    internal class PropertyChangedListener
    {
        public IList<PropertyChangedEventArgs> RaisedPropertyChangedEventArgs { get; private set; }
        public IList<ObservablePropertyChangedEventArgs> RaisedObservablePropertyChangedEventArgs { get; private set; }

        public PropertyChangedListener(ObservableObject obj)
        {
            this.RaisedPropertyChangedEventArgs = new List<PropertyChangedEventArgs>();
            obj.PropertyChanged += (sender, e) => this.RaisedPropertyChangedEventArgs.Add(e);
            this.RaisedObservablePropertyChangedEventArgs = new List<ObservablePropertyChangedEventArgs>();
            obj.ObservablePropertyChanged += (sender, e) => this.RaisedObservablePropertyChangedEventArgs.Add(e);
        }
    }
}