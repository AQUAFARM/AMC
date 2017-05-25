using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace JelleDruyts.Windows.ObjectModel
{
    /// <summary>
    /// A base class for objects that provides property change notifications, mostly to be used in conjunction with <see cref="ObservableProperty{TProperty}"/> instances for properties.
    /// </summary>
    /// <remarks>
    /// One could argue that this is a lighterweight clone of WPF's DependencyObject and DependencyProperty infrastructure,
    /// and that would probably be correct. The reason this class and its associated classes exist is that DependencyProperties have
    /// a few drawbacks around threading and testability that are well explained by Kent Boogaart at http://kentb.blogspot.com/2009/03/view-models-pocos-versus.html.
    /// This implementation also removes the need for strings as property names by using lambda expressions so the properties can be safely refactored.
    /// </remarks>
    [DataContract]
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Properties

        private IDictionary<ObservableProperty, object> propertyValues;

        internal IDictionary<ObservableProperty, object> PropertyValues
        {
            get
            {
                if (this.propertyValues == null)
                {
                    this.propertyValues = new Dictionary<ObservableProperty, object>();
                }
                return this.propertyValues;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        protected ObservableObject()
        {
        }

        #endregion

        #region Get & Set Value

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>The current value of the property.</returns>
        public TProperty GetValue<TProperty>(ObservableProperty<TProperty> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            if (!this.PropertyValues.ContainsKey(property))
            {
                return property.DefaultValue;
            }
            return (TProperty)this.PropertyValues[property];
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><see langword="true"/> if the property value has effectively changed, <see langword="false"/> otherwise.</returns>
        public bool SetValue<TProperty>(ObservableProperty<TProperty> property, TProperty value)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            return property.SetValue(this, value);
        }

        #endregion

        #region ObservablePropertyChanged Event

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event EventHandler<ObservablePropertyChangedEventArgs> ObservablePropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnObservablePropertyChangedInternal(ObservablePropertyChangedEventArgs e)
        {
            OnObservablePropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnObservablePropertyChanged(ObservablePropertyChangedEventArgs e)
        {
            if (this.ObservablePropertyChanged != null)
            {
                this.ObservablePropertyChanged(this, e);
            }
        }

        #endregion

        #region PropertyChanged Event

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        internal void OnPropertyChangedInternal(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Clone & CopyFrom

        /// <summary>
        /// Returns a shallow clone of the current <see cref="ObservableObject" />.
        /// </summary>
        /// <typeparam name="T">The type of the object to clone.</typeparam>
        /// <returns>A clone of the current <see cref="ObservableObject"/> where each <see cref="ObservableProperty"/> value is the same reference as the original value.</returns>
        /// <remarks>Non-observable properties or fields are never copied.</remarks>
        public T Clone<T>() where T : ObservableObject, new()
        {
            var clone = new T();
            clone.PropertyValues.Clear();
            foreach (var property in this.PropertyValues)
            {
                property.Key.SetValue(clone, property.Value);
            }
            return clone;
        }

        /// <summary>
        /// Copies all the <see cref="ObservableProperty"/> values of the specified <paramref name="value"/> to the current <see cref="ObservableObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="value">The <see cref="ObservableObject"/> from which to copy all the observable properties.</param>
        /// <remarks>Non-observable properties or fields are never copied.</remarks>
        public void CopyFrom<T>(T value) where T : ObservableObject
        {
            // Reset all current properties that will not be set by the instance being copied from.
            foreach (var currentProperty in this.PropertyValues.Keys.Except(value.PropertyValues.Keys).ToArray())
            {
                currentProperty.ResetValue(this);
            }

            // Set all properties from the instance being copied from.
            foreach (var property in value.PropertyValues.ToArray())
            {
                property.Key.SetValue(this, property.Value);
            }
        }

        #endregion
    }
}