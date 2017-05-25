using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// The default drop handler.
    /// </summary>
    public class DefaultDropHandler : IDropTarget
    {
        /// <summary>
        /// Updates the current drag state.
        /// </summary>
        /// <param name="dropInfo">Information about the drag.</param>
        /// <remarks>
        /// To allow a drop at the current drag position, the <see cref="DropInfo.Effects"/> property on
        /// <paramref name="dropInfo"/> should be set to a value other than <see cref="DragDropEffects.None"/>
        /// and <see cref="DropInfo.Data"/> should be set to a non-null value.
        /// </remarks>
        public virtual void DragOver(DropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        /// <summary>
        /// Performs a drop.
        /// </summary>
        /// <param name="dropInfo">Information about the drop.</param>
        public virtual void Drop(DropInfo dropInfo)
        {
            int insertIndex = dropInfo.InsertIndex;
            IList destinationList = GetList(dropInfo.TargetCollection);
            IEnumerable data = ExtractData(dropInfo.Data);

            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                IList sourceList = GetList(dropInfo.DragInfo.SourceCollection);

                foreach (object o in data)
                {
                    int index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);

                        if (sourceList == destinationList && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }
            }

            if (insertIndex < 0)
            {
                insertIndex = 0;
            }

            foreach (object o in data)
            {
                destinationList.Insert(insertIndex++, o);
            }
        }

        /// <summary>
        /// Determines whether this instance can accept the data from the specified drop info.
        /// </summary>
        /// <param name="dropInfo">The drop info.</param>
        /// <returns><see langword="true"/> if this instance can accept the data from the specified drop info; otherwise, <see langword="false"/>.</returns>
        protected static bool CanAcceptData(DropInfo dropInfo)
        {
            if (dropInfo.TargetCollection == null || dropInfo.DragInfo == null)
            {
                return false;
            }
            else if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                return GetList(dropInfo.TargetCollection) != null;
            }
            else if (dropInfo.DragInfo.SourceCollection is ItemCollection)
            {
                return false;
            }
            else
            {
                if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
                {
                    return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Extracts the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }
            else
            {
                return Enumerable.Repeat(data, 1);
            }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        protected static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }
            else
            {
                return enumerable as IList;
            }
        }

        /// <summary>
        /// Determines whether the target item is a child of the source item.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        /// <param name="sourceItem">The source item.</param>
        /// <returns><see langword="true"/> if the target item is a child of the source item; otherwise, <see langword="false"/>.</returns>
        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (parent == sourceItem)
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        /// <summary>
        /// Tests the compatible types.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) =>
            {
                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            };

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

            if (enumerableTypes.Count() > 0)
            {
                Type dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            else
            {
                return target is IList;
            }
        }
    }
}
