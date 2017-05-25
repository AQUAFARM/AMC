using System;
using System.Windows;
using System.Windows.Documents;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// A base class for drop target adorners.
    /// </summary>
    public abstract class DropTargetAdorner : Adorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Raised when adornedElement is null.
        /// </exception>
        public DropTargetAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            m_AdornerLayer.Add(this);
            IsHitTestVisible = false;
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            m_AdornerLayer.Remove(this);
        }

        /// <summary>
        /// Gets or sets the drop info.
        /// </summary>
        public DropInfo DropInfo { get; set; }

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="adornedElement">The adorned element.</param>
        /// <returns></returns>
        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");
            }

            return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
                .Invoke(new[] { adornedElement });
        }

        private AdornerLayer m_AdornerLayer;
    }
}
