using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// Provides extension methods for visual trees.
    /// </summary>
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// Gets the visual ancestor of a visual.
        /// </summary>
        /// <typeparam name="T">The type of ancestor to retrieve.</typeparam>
        /// <param name="target">The target visual.</param>
        /// <returns>The visual ancestor of the target object of the specified type.</returns>
        public static T GetVisualAncestor<T>(this DependencyObject target) where T : class
        {
            DependencyObject item;
            try
            {
                item = VisualTreeHelper.GetParent(target);
            }
            catch (InvalidOperationException)
            {
                // Thrown when the dependency object is not a visual.
                return null;
            }
            while (item != null)
            {
                T itemAsT = item as T;
                if (itemAsT != null) return itemAsT;
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        /// <summary>
        /// Gets the visual ancestor of a visual.
        /// </summary>
        /// <param name="target">The target visual.</param>
        /// <param name="type">The type of ancestor to retrieve.</param>
        /// <returns>The visual ancestor of the target object of the specified type.</returns>
        public static DependencyObject GetVisualAncestor(this DependencyObject target, Type type)
        {
            DependencyObject item = VisualTreeHelper.GetParent(target);

            while (item != null)
            {
                if (item.GetType() == type) return item;
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        /// <summary>
        /// Gets the visual descendent of a visual.
        /// </summary>
        /// <typeparam name="T">The type of descendent to retrieve.</typeparam>
        /// <param name="target">The target visual.</param>
        /// <returns>The visual descendent of the target object of the specified type.</returns>
        public static T GetVisualDescendent<T>(this DependencyObject target) where T : DependencyObject
        {
            return target.GetVisualDescendents<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the visual descendents of a visual.
        /// </summary>
        /// <typeparam name="T">The type of descendent to retrieve.</typeparam>
        /// <param name="target">The target visual.</param>
        /// <returns>The visual descendents of the target object of the specified type.</returns>
        public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject target) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(target);

            for (int n = 0; n < childCount; n++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(target, n);

                if (child is T)
                {
                    yield return (T)child;
                }

                foreach (T match in GetVisualDescendents<T>(child))
                {
                    yield return match;
                }
            }

            yield break;
        }
    }
}
