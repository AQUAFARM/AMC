using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public class PictureQueueViewModel : ObservableObject
    {
        #region Properties

        /// <summary>
        /// Gets a value that determines if drag &amp; drop is supported on this picture queue.
        /// </summary>
        public bool IsDragDropSupported { get; private set; }

        /// <summary>
        /// Gets the commands that are available for this picture queue.
        /// </summary>
        public IEnumerable<ICommand> Commands { get; private set; }

        /// <summary>
        /// Gets the input bindings for the commands.
        /// </summary>
        public IEnumerable<InputBinding> InputBindings { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the pictures to be displayed.
        /// </summary>
        public ICollectionView Pictures
        {
            get { return this.GetValue(PicturesProperty); }
            set { this.SetValue(PicturesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Pictures"/> observable property.
        /// </summary>
        public static ObservableProperty<ICollectionView> PicturesProperty = new ObservableProperty<ICollectionView, PictureQueueViewModel>(o => o.Pictures);

        /// <summary>
        /// Gets or sets the scale of the pictures.
        /// </summary>
        public double Scale
        {
            get { return this.GetValue(ScaleProperty); }
            set { this.SetValue(ScaleProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Scale"/> observable property.
        /// </summary>
        public static ObservableProperty<double> ScaleProperty = new ObservableProperty<double, PictureQueueViewModel>(o => o.Scale, 1);

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public Account Account
        {
            get { return this.GetValue(AccountProperty); }
            set { this.SetValue(AccountProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Account"/> observable property.
        /// </summary>
        public static ObservableProperty<Account> AccountProperty = new ObservableProperty<Account, PictureQueueViewModel>(o => o.Account);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureQueueViewModel"/> class.
        /// </summary>
        /// <param name="isDragDropSupported">A value that determines if drag &amp; drop is supported on this picture queue.</param>
        /// <param name="commands">The commands that are available for this picture queue.</param>
        public PictureQueueViewModel(bool isDragDropSupported, IEnumerable<ICommand> commands)
        {
            this.IsDragDropSupported = isDragDropSupported;
            this.Commands = commands;
            this.InputBindings = this.Commands.OfType<RelayCommand>().SelectMany(r => r.InputGestures.Select(g => new InputBinding(r, g)));
        }

        #endregion
    }
}