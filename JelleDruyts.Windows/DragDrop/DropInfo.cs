using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// Holds information about a the target of a drag drop operation.
    /// </summary>
    /// 
    /// <remarks>
    /// The <see cref="DropInfo"/> class holds all of the framework's information about the current 
    /// target of a drag. It is used by <see cref="IDropTarget.DragOver"/> method to determine whether 
    /// the current drop target is valid, and by <see cref="IDropTarget.Drop"/> to perform the drop.
    /// </remarks>
    public class DropInfo
    {
        /// <summary>
        /// Gets or sets the mouse position.
        /// </summary>
        public Point? MousePosition { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DropInfo class.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The sender of the drag event.
        /// </param>
        /// 
        /// <param name="e">
        /// The drag event.
        /// </param>
        /// 
        /// <param name="dragInfo">
        /// Information about the source of the drag, if the drag came from within the framework.
        /// </param>
        public DropInfo(object sender, DragEventArgs e, DragInfo dragInfo)
        {
            string dataFormat = DragDrop.DataFormat.Name;
            Data = (e.Data.GetDataPresent(dataFormat)) ? e.Data.GetData(dataFormat) : e.Data;
            DragInfo = dragInfo;

            VisualTarget = sender as UIElement;
            if (VisualTarget != null)
            {
                this.MousePosition = e.GetPosition(VisualTarget);
            }

            if (sender is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                Point position = e.GetPosition(itemsControl);
                bool isOnGroupHeader;
                HeaderedContentControl groupHeaderControl;
                TargetGroup = FindGroup(itemsControl, position, out isOnGroupHeader, out groupHeaderControl);
                UIElement item = itemsControl.GetItemContainerAt(position);
                bool directlyOverItem = item != null;

                // The code below attempts to search an item "close by" in a horizontal or vertical line scan depending on the panel orientation,
                // but this can interfere with certain layouts. E.g. if the group's container template is an expander that expands to the right,
                // it scans to the right of the expander's header and effectively finds the first actual item in the group there.

                //if (item == null)
                //{
                //    item = itemsControl.GetItemContainerAt(position, VisualTargetOrientation);
                //}

                if (item != null)
                {
                    VisualTargetOrientation = item.GetItemsPanelOrientation();
                    ItemsControl itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    TargetCollection = itemParent.ItemsSource ?? itemParent.Items;

                    if (directlyOverItem)
                    {
                        TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                        VisualTargetItem = item;
                    }

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (e.GetPosition(item).Y > item.RenderSize.Height / 2) InsertIndex++;
                    }
                    else
                    {
                        if (e.GetPosition(item).X > item.RenderSize.Width / 2) InsertIndex++;
                    }
                }
                else
                {
                    if (TargetGroup != null && TargetGroup.ItemCount > 0)
                    {
                        // Not being dropped directly on an item but somewhere on a group.
                        this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;

                        bool createNewGroupOnHeaderDrop = DragDrop.GetCreateNewGroupOnHeaderDrop(this.VisualTarget);
                        if (isOnGroupHeader && createNewGroupOnHeaderDrop)
                        {
                            // Insert before the beginning of that group.
                            var itemContainer = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromItem(TargetGroup.Items[0]);
                            this.InsertIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer);
                            this.VisualTargetOrientation = Orientation.Vertical;
                            this.TargetGroupControl = groupHeaderControl;
                            this.NewGroupRequestedBefore = true;
                        }
                        else
                        {
                            // Insert at the end of that group.
                            var itemContainer = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromItem(TargetGroup.Items[TargetGroup.ItemCount - 1]);
                            this.InsertIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) + 1;
                            this.VisualTargetOrientation = itemContainer.GetItemsPanelOrientation();
                        }
                    }
                    else
                    {
                        // Not being dropped directly on an item or somewhere on a group but outside any container.
                        var belowAllElements = true;
                        UIElement lowestUIElement = null;
                        double lowestUIElementY = double.MinValue;
                        for (var i = 0; i < itemsControl.Items.Count; i++)
                        {
                            var pictureElement = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                            var pictureElementY = pictureElement.TranslatePoint(new Point(), itemsControl).Y;
                            if (lowestUIElement == null || pictureElementY > lowestUIElementY)
                            {
                                lowestUIElement = pictureElement;
                                lowestUIElementY = pictureElementY;
                            }
                            if (e.GetPosition(pictureElement).Y < 0)
                            {
                                belowAllElements = false;
                                break;
                            }
                        }

                        if (belowAllElements)
                        {
                            var collectionView = itemsControl.ItemsSource as CollectionView;
                            if (collectionView != null)
                            {
                                this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                                if (collectionView.Groups.Count > 0)
                                {
                                    this.TargetGroup = collectionView.Groups[collectionView.Groups.Count - 1] as CollectionViewGroup;
                                }
                                this.InsertIndex = itemsControl.Items.Count;
                                this.VisualTargetOrientation = Orientation.Vertical;
                                if (lowestUIElement != null)
                                {
                                    this.TargetGroupControl = lowestUIElement.GetVisualAncestor<HeaderedContentControl>();
                                }
                                this.NewGroupRequestedAfter = true;
                            }
                        }
                    }
                }
            }
        }

        private CollectionViewGroup FindGroup(ItemsControl itemsControl, Point position, out bool isOnGroupHeader, out HeaderedContentControl groupHeaderControl)
        {
            isOnGroupHeader = false;
            groupHeaderControl = null;
            DependencyObject element = itemsControl.InputHitTest(position) as DependencyObject;

            if (element != null)
            {
                HeaderedContentControl headeredParent = element.GetVisualAncestor<HeaderedContentControl>();
                if (headeredParent != null)
                {
                    groupHeaderControl = headeredParent;
                    var header = headeredParent.Header as Visual;
                    if (header != null)
                    {
                        if (header.IsAncestorOf(element))
                        {
                            isOnGroupHeader = true;
                        }
                    }
                }

                GroupItem groupItem = element.GetVisualAncestor<GroupItem>();
                if (groupItem != null)
                {
                    return groupItem.Content as CollectionViewGroup;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// 
        /// <remarks>
        /// If the drag came from within the framework, this will hold:
        /// 
        /// - The dragged data if a single item was dragged.
        /// - A typed IEnumerable if multiple items were dragged.
        /// </remarks>
        public object Data { get; private set; }

        /// <summary>
        /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
        /// if the drag came from within the framework.
        /// </summary>
        public DragInfo DragInfo { get; private set; }

        /// <summary>
        /// Gets or sets the class of drop target to display.
        /// </summary>
        /// 
        /// <remarks>
        /// The standard drop target adorner classes are held in the <see cref="DropTargetAdorners"/>
        /// class.
        /// </remarks>
        public Type DropTargetAdorner { get; set; }

        /// <summary>
        /// Gets or sets the allowed effects for the drop.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drop handler in order 
        /// for a drop to be possible.
        /// </remarks>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets the current insert position within <see cref="TargetCollection"/>.
        /// </summary>
        public int InsertIndex { get; private set; }

        /// <summary>
        /// Gets the collection that the target ItemsControl is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable TargetCollection { get; private set; }

        /// <summary>
        /// Gets the object that the current drop target is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object TargetItem { get; private set; }

        /// <summary>
        /// Gets the current group target.
        /// </summary>
        /// 
        /// <remarks>
        /// If the drag is currently over an ItemsControl with groups, describes the group that
        /// the drag is currently over.
        /// </remarks>
        public CollectionViewGroup TargetGroup { get; private set; }

        /// <summary>
        /// Gets the control that is the current drop target.
        /// </summary>
        public UIElement VisualTarget { get; private set; }

        /// <summary>
        /// Gets the item in an ItemsControl that is the current drop target.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public UIElement VisualTargetItem { get; private set; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public Orientation VisualTargetOrientation { get; private set; }

        /// <summary>
        /// Gets or sets the target group control.
        /// </summary>
        public HeaderedContentControl TargetGroupControl { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether a new group is requested before the target item.
        /// </summary>
        public bool NewGroupRequestedBefore { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether a new group is requested after the target item.
        /// </summary>
        public bool NewGroupRequestedAfter { get; private set; }
    }
}
