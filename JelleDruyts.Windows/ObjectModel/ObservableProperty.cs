using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace JelleDruyts.Windows.ObjectModel
{
    /// <summary>
    /// A property that can be observed for changed values.
    /// </summary>
    public abstract class ObservableProperty
    {
        #region Properties

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty"/> class.
        /// </summary>
        protected ObservableProperty()
        {
        }

        #endregion

        #region SetValue & ResetValue

        /// <summary>
        /// Sets the value of the current property.
        /// </summary>
        /// <param name="instance">The object instance on which to set the property.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><see langword="true"/> if the property value has effectively changed, <see langword="false"/> otherwise.</returns>
        internal abstract bool SetValue(ObservableObject instance, object value);

        /// <summary>
        /// Resets the value of the current property to its default value.
        /// </summary>
        /// <param name="instance">The object instance on which to reset the property to its default value.</param>
        /// <returns><see langword="true"/> if the property value has effectively changed, <see langword="false"/> otherwise.</returns>
        internal abstract bool ResetValue(ObservableObject instance);

        #endregion
    }

    /// <summary>
    /// A property that can be observed for changed values.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    public abstract class ObservableProperty<TProperty> : ObservableProperty
    {
        #region Properties

        /// <summary>
        /// Gets the default value of the property.
        /// </summary>
        public TProperty DefaultValue { get; protected set; }

        /// <summary>
        /// Gets the callback method to invoke when the property value has changed.
        /// </summary>
        public ObservablePropertyChangedCallback<TProperty> PropertyChangedCallback { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty&lt;TProperty&gt;"/> class.
        /// </summary>
        protected ObservableProperty()
        {
        }

        #endregion

        #region SetValue & ResetValue

        /// <summary>
        /// Sets the value of the current property.
        /// </summary>
        /// <param name="instance">The object instance on which to set the property.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><see langword="true"/> if the property value has effectively changed, <see langword="false"/> otherwise.</returns>
        internal override bool SetValue(ObservableObject instance, object value)
        {
            var typedValue = (TProperty)value;
            var hasPropertyChanged = false;
            TProperty oldValue = this.DefaultValue;
            if (instance.PropertyValues.ContainsKey(this))
            {
                oldValue = (TProperty)instance.PropertyValues[this];
                if (!object.Equals(oldValue, typedValue))
                {
                    instance.PropertyValues[this] = typedValue;
                    hasPropertyChanged = true;
                }
            }
            else
            {
                if (!object.Equals(this.DefaultValue, typedValue))
                {
                    instance.PropertyValues.Add(this, typedValue);
                    hasPropertyChanged = true;
                }
            }
            if (hasPropertyChanged)
            {
                if (this.PropertyChangedCallback != null)
                {
                    this.PropertyChangedCallback(instance, new ObservablePropertyChangedEventArgs<TProperty>(this, oldValue, typedValue));
                }
                instance.OnObservablePropertyChangedInternal(new ObservablePropertyChangedEventArgs<TProperty>(this, oldValue, typedValue));
                instance.OnPropertyChangedInternal(this.Name);
            }
            return hasPropertyChanged;
        }

        /// <summary>
        /// Resets the value of the current property to its default value.
        /// </summary>
        /// <param name="instance">The object instance on which to reset the property to its default value.</param>
        /// <returns><see langword="true"/> if the property value has effectively changed, <see langword="false"/> otherwise.</returns>
        internal override bool ResetValue(ObservableObject instance)
        {
            return SetValue(instance, this.DefaultValue);
        }

        #endregion
    }

    /// <summary>
    /// A property that can be observed for changed values.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TObservableObject">The type of the observable object that defines this property.</typeparam>
    public class ObservableProperty<TProperty, TObservableObject> : ObservableProperty<TProperty>
    {
        #region Properties

        /// <summary>
        /// Gets the information about the actual property encapsulated by this observable property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty&lt;TProperty, TObservableObject&gt;"/> class.
        /// </summary>
        /// <param name="property">An expression that defines the property.</param>
        public ObservableProperty(Expression<Func<TObservableObject, TProperty>> property)
            : this(property, default(TProperty), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty&lt;TProperty, TObservableObject&gt;"/> class.
        /// </summary>
        /// <param name="property">An expression that defines the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        public ObservableProperty(Expression<Func<TObservableObject, TProperty>> property, TProperty defaultValue)
            : this(property, defaultValue, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty&lt;TProperty, TObservableObject&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="propertyChangedCallback">The callback method to be invoked when the property value has changed.</param>
        public ObservableProperty(Expression<Func<TObservableObject, TProperty>> property, ObservablePropertyChangedCallback<TProperty> propertyChangedCallback)
            : this(property, default(TProperty), propertyChangedCallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty&lt;TProperty, TObservableObject&gt;"/> class.
        /// </summary>
        /// <param name="property">An expression that defines the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <param name="propertyChangedCallback">The callback method to be invoked when the property value has changed.</param>
        public ObservableProperty(Expression<Func<TObservableObject, TProperty>> property, TProperty defaultValue, ObservablePropertyChangedCallback<TProperty> propertyChangedCallback)
        {
            MemberExpression member = property.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Expression '{0}' refers to a method, not a property.", property.ToString()));
            }

            this.PropertyInfo = member.Member as PropertyInfo;
            if (this.PropertyInfo == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Expression '{0}' refers to a field, not a property.", property.ToString()));
            }

            Type type = typeof(TObservableObject);
            if (!this.PropertyInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Expression '{0}' refers to a property that is not from type {1}.", property.ToString(), type));
            }

            this.Name = this.PropertyInfo.Name;
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        #endregion
    }
}