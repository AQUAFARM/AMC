using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace JelleDruyts.Windows.Collections
{
    /// <summary>
    /// Provides attached properties to merge a collection into existing collections on a <see cref="UIElement"/>.
    /// </summary>
    public static class MergeCollections
    {
        #region Input Bindings

        /// <summary>
        /// Gets the input bindings to be added to a specified <see cref="UIElement"/>.
        /// </summary>
        /// <param name="target">The <see cref="UIElement"/> to add the input bindings to.</param>
        /// <returns>The input bindings to be added.</returns>
        public static IEnumerable<InputBinding> GetInputBindings(UIElement target)
        {
            return (IEnumerable<InputBinding>)target.GetValue(InputBindingsProperty);
        }

        /// <summary>
        /// Sets the input bindings to be added to a specified <see cref="UIElement"/>.
        /// </summary>
        /// <param name="target">The <see cref="UIElement"/> to add the input bindings to.</param>
        /// <param name="inputBindings">The input bindings to be added.</param>
        public static void SetInputBindings(UIElement target, IEnumerable<InputBinding> inputBindings)
        {
            target.SetValue(InputBindingsProperty, inputBindings);
        }

        /// <summary>
        /// Identifies the InputBindings attached property.
        /// </summary>
        public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached("InputBindings", typeof(IEnumerable<InputBinding>), typeof(MergeCollections), new UIPropertyMetadata(null, InputBindingsChanged));

        /// <summary>
        /// Called when the input bindings changed.
        /// </summary>
        /// <param name="d">The <see cref="UIElement"/> to add the input bindings to.</param>
        /// <param name="e">The event arguments.</param>
        private static void InputBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (UIElement)d;
            var oldBindings = (IEnumerable<InputBinding>)e.OldValue;
            var newBindings = (IEnumerable<InputBinding>)e.NewValue;
            Merge(target.InputBindings, oldBindings, newBindings);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Merges the specified elements into the specified target list.
        /// </summary>
        /// <param name="targetList">The target list.</param>
        /// <param name="oldMergedElements">The old merged elements to be removed from the target list.</param>
        /// <param name="newMergedElements">The new merged elements to be added to the target list.</param>
        private static void Merge(IList targetList, IEnumerable oldMergedElements, IEnumerable newMergedElements)
        {
            if (oldMergedElements != null)
            {
                // Remove any previously added elements.
                foreach (var oldMergedElement in oldMergedElements)
                {
                    targetList.Remove(oldMergedElement);
                }
            }
            if (newMergedElements != null)
            {
                // Add new elements.
                foreach (var newMergedElement in newMergedElements)
                {
                    targetList.Add(newMergedElement);
                }
            }
        }

        #endregion
    }
}