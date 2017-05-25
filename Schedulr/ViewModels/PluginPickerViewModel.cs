using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Extensibility;

namespace Schedulr.ViewModels
{
    public class PluginPickerViewModel : ObservableObject
    {
        #region Properties

        public ICommand AddPluginCommand { get; private set; }
        public ICommand RemovePluginCommand { get; private set; }
        public ICommand MovePluginUpCommand { get; private set; }
        public ICommand MovePluginDownCommand { get; private set; }
        public PluginCategory Category { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the available plugin types.
        /// </summary>
        public IEnumerable<PluginType> PluginTypes
        {
            get { return this.GetValue(PluginTypesProperty); }
            set { this.SetValue(PluginTypesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PluginTypes"/> observable property.
        /// </summary>
        public static ObservableProperty<IEnumerable<PluginType>> PluginTypesProperty = new ObservableProperty<IEnumerable<PluginType>, PluginPickerViewModel>(o => o.PluginTypes);

        /// <summary>
        /// Gets or sets the selected plugin type.
        /// </summary>
        public PluginType SelectedPluginType
        {
            get { return this.GetValue(SelectedPluginTypeProperty); }
            set { this.SetValue(SelectedPluginTypeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SelectedPluginType"/> observable property.
        /// </summary>
        public static ObservableProperty<PluginType> SelectedPluginTypeProperty = new ObservableProperty<PluginType, PluginPickerViewModel>(o => o.SelectedPluginType, null, OnSelectedPluginTypeChanged);

        /// <summary>
        /// Gets or sets all available plugin collections.
        /// </summary>
        public IEnumerable<PluginCollection> AllPluginCollections
        {
            get { return this.GetValue(AllPluginCollectionsProperty); }
            set { this.SetValue(AllPluginCollectionsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AllPluginCollections"/> observable property.
        /// </summary>
        public static ObservableProperty<IEnumerable<PluginCollection>> AllPluginCollectionsProperty = new ObservableProperty<IEnumerable<PluginCollection>, PluginPickerViewModel>(o => o.AllPluginCollections);

        /// <summary>
        /// Gets or sets the available plugin collections for the selected plugin type.
        /// </summary>
        public IEnumerable<PluginCollection> PluginCollections
        {
            get { return this.GetValue(PluginCollectionsProperty); }
            set { this.SetValue(PluginCollectionsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PluginCollections"/> observable property.
        /// </summary>
        public static ObservableProperty<IEnumerable<PluginCollection>> PluginCollectionsProperty = new ObservableProperty<IEnumerable<PluginCollection>, PluginPickerViewModel>(o => o.PluginCollections);

        /// <summary>
        /// Gets or sets the selected plugin collection.
        /// </summary>
        public PluginCollection SelectedPluginCollection
        {
            get { return this.GetValue(SelectedPluginCollectionProperty); }
            set { this.SetValue(SelectedPluginCollectionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SelectedPluginCollection"/> observable property.
        /// </summary>
        public static ObservableProperty<PluginCollection> SelectedPluginCollectionProperty = new ObservableProperty<PluginCollection, PluginPickerViewModel>(o => o.SelectedPluginCollection);

        /// <summary>
        /// Gets or sets a value that determines if plugin collection selection is enabled.
        /// </summary>
        public bool IsPluginCollectionSelectionEnabled
        {
            get { return this.GetValue(IsPluginCollectionSelectionEnabledProperty); }
            set { this.SetValue(IsPluginCollectionSelectionEnabledProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsPluginCollectionSelectionEnabled"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsPluginCollectionSelectionEnabledProperty = new ObservableProperty<bool, PluginPickerViewModel>(o => o.IsPluginCollectionSelectionEnabled);

        /// <summary>
        /// Gets or sets the visibility of the additional plugin details.
        /// </summary>
        public Visibility AdditionalPluginDetailsVisibility
        {
            get { return this.GetValue(AdditionalPluginDetailsVisibilityProperty); }
            set { this.SetValue(AdditionalPluginDetailsVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AdditionalPluginDetailsVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> AdditionalPluginDetailsVisibilityProperty = new ObservableProperty<Visibility, PluginPickerViewModel>(o => o.AdditionalPluginDetailsVisibility);

        /// <summary>
        /// Gets or sets the visibility of the plugin author details.
        /// </summary>
        public Visibility AdditionalPluginDetailsAuthorVisibility
        {
            get { return this.GetValue(AdditionalPluginDetailsAuthorVisibilityProperty); }
            set { this.SetValue(AdditionalPluginDetailsAuthorVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AdditionalPluginDetailsAuthorVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> AdditionalPluginDetailsAuthorVisibilityProperty = new ObservableProperty<Visibility, PluginPickerViewModel>(o => o.AdditionalPluginDetailsAuthorVisibility);

        /// <summary>
        /// Gets or sets the visibility of the plugin version details.
        /// </summary>
        public Visibility AdditionalPluginDetailsVersionVisibility
        {
            get { return this.GetValue(AdditionalPluginDetailsVersionVisibilityProperty); }
            set { this.SetValue(AdditionalPluginDetailsVersionVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AdditionalPluginDetailsVersionVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> AdditionalPluginDetailsVersionVisibilityProperty = new ObservableProperty<Visibility, PluginPickerViewModel>(o => o.AdditionalPluginDetailsVersionVisibility);

        /// <summary>
        /// Gets or sets the visibility of the plugin URL details.
        /// </summary>
        public Visibility AdditionalPluginDetailsUrlVisibility
        {
            get { return this.GetValue(AdditionalPluginDetailsUrlVisibilityProperty); }
            set { this.SetValue(AdditionalPluginDetailsUrlVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AdditionalPluginDetailsUrlVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> AdditionalPluginDetailsUrlVisibilityProperty = new ObservableProperty<Visibility, PluginPickerViewModel>(o => o.AdditionalPluginDetailsUrlVisibility);

        #endregion

        #region Constructors

        public PluginPickerViewModel(PluginCategory category)
        {
            this.AddPluginCommand = new RelayCommand(AddPlugin, CanAddPlugin, "Add Action", "Adds the action to the list.");
            this.RemovePluginCommand = new RelayCommand(RemovePlugin, CanRemovePlugin, "Remove Action", "Removes the action from the list.");
            this.MovePluginUpCommand = new RelayCommand(MovePluginUp, CanMovePluginUp, "Move Up", "Moves the action up in the list.");
            this.MovePluginDownCommand = new RelayCommand(MovePluginDown, CanMovePluginDown, "Move Down", "Moves the action down in the list.");
            this.Category = category;
            this.PluginTypes = PluginManager.GetPluginTypes(this.Category).Where(t => t.AvailablePluginCollections.Count > 0).OrderBy(p => p.Name);
            this.SelectedPluginType = this.PluginTypes.FirstOrDefault();
            this.AllPluginCollections = PluginManager.GetPluginCollections(this.Category);
        }

        #endregion

        #region Event Handlers

        private static void OnSelectedPluginTypeChanged(ObservableObject sender, ObservablePropertyChangedEventArgs<PluginType> e)
        {
            var viewModel = (PluginPickerViewModel)sender;
            viewModel.PluginCollections = viewModel.SelectedPluginType.AvailablePluginCollections.Where(collection => collection.Category == viewModel.Category);
            viewModel.SelectedPluginCollection = viewModel.PluginCollections.FirstOrDefault();
            viewModel.IsPluginCollectionSelectionEnabled = viewModel.PluginCollections.Count() > 1;
            var hasAuthor = (viewModel.SelectedPluginType != null && !string.IsNullOrEmpty(viewModel.SelectedPluginType.Author));
            var hasUrl = (viewModel.SelectedPluginType != null && !string.IsNullOrEmpty(viewModel.SelectedPluginType.Url));
            var hasVersion = (viewModel.SelectedPluginType != null && !string.IsNullOrEmpty(viewModel.SelectedPluginType.Version));
            viewModel.AdditionalPluginDetailsVisibility = ((hasAuthor || hasUrl || hasVersion) ? Visibility.Visible : Visibility.Collapsed);
            viewModel.AdditionalPluginDetailsAuthorVisibility = (hasAuthor ? Visibility.Visible : Visibility.Collapsed);
            viewModel.AdditionalPluginDetailsUrlVisibility = (hasUrl ? Visibility.Visible : Visibility.Collapsed);
            viewModel.AdditionalPluginDetailsVersionVisibility = (hasVersion ? Visibility.Visible : Visibility.Collapsed);
        }

        #endregion

        #region Commands

        private bool CanAddPlugin(object parameter)
        {
            return (this.SelectedPluginCollection != null && this.SelectedPluginType != null && this.SelectedPluginCollection.CanAddPluginInstance(this.SelectedPluginType));
        }

        private void AddPlugin(object parameter)
        {
            try
            {
                this.SelectedPluginCollection.AddPluginInstance(this.SelectedPluginType);
            }
            catch (Exception exc)
            {
                MessageBox.Show("The plugin could not be added, please see the log file for more information. " + exc.Message);
            }
        }

        private bool CanRemovePlugin(object parameter)
        {
            return parameter is PluginInstance;
        }

        private void RemovePlugin(object parameter)
        {
            var plugin = (PluginInstance)parameter;
            plugin.Collection.RemovePlugin(plugin);
        }

        private bool CanMovePluginUp(object parameter)
        {
            var plugin = (PluginInstance)parameter;
            return plugin.Collection.CanMovePluginInstanceUp(plugin);
        }

        private void MovePluginUp(object parameter)
        {
            var plugin = (PluginInstance)parameter;
            plugin.Collection.MovePluginInstanceUp(plugin);
        }

        private bool CanMovePluginDown(object parameter)
        {
            var plugin = (PluginInstance)parameter;
            return plugin.Collection.CanMovePluginInstanceDown(plugin);
        }

        private void MovePluginDown(object parameter)
        {
            var plugin = (PluginInstance)parameter;
            plugin.Collection.MovePluginInstanceDown(plugin);
        }

        #endregion
    }
}