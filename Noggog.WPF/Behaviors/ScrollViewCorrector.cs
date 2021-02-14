using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Noggog.WPF
{
    // https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
    public sealed class ScrollViewerBehavior
    {
        #region Constants

        public static readonly DependencyProperty FixScrollingProperty =
            DependencyProperty.RegisterAttached
            ("FixScrolling", typeof(bool), typeof(ScrollViewerBehavior),
            new FrameworkPropertyMetadata(false, OnFixScrollingPropertyChanged));

        #endregion

        #region Fields

        /// <summary>
        /// Field used to keep track of the original MouseWheelEvent that started the current sequence
        /// </summary>
        private static MouseWheelEventArgs? s_currentOriginalEventArg;

        /// <summary>
        /// Field used to keep all the information related to the current sequence of MouseWheelEvents
        /// </summary>
        private static ScrollViewerBehaviorSequence? s_sequence;

        #endregion

        #region Event Handlers

        public static void OnFixScrollingPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                var fixScrolling = (bool)e.NewValue;
                if (fixScrolling)
                {
                    scrollViewer.PreviewMouseWheel += OnScrollViewerPreviewMouseWheel;
                }
                else
                {
                    scrollViewer.PreviewMouseWheel -= OnScrollViewerPreviewMouseWheel;
                }
            }
            else
            {
                throw new ArgumentException("The dependency property can only be attached to a ScrollViewer", "sender");
            }
        }

        private static void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            // We only want to process the PreviewMouseWheel event in the following cases:
            // The event was not previously handled
            // The event is a fake event OR the first time we receive an original event
            // The current ScrollViewer was not already processed
            if (!e.Handled && !e.Equals(s_currentOriginalEventArg) && (scrollViewer != null) &&
                (s_sequence == null || (!s_sequence.ScrollViewers.Contains(scrollViewer) && s_sequence.FakeEvents.Contains(e))))
            {
                // Create our fake event that will be used to parse all the children ScrollViewers
                var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent,
                    Source = e.OriginalSource,
                };

                var originalSource = e.OriginalSource as IInputElement;
                if (originalSource != null)
                {
                    if (s_sequence == null)
                    {
                        // First time through the sequence, this only happen the first time we receive the original event
                        s_sequence = new ScrollViewerBehaviorSequence(e, scrollViewer, previewEventArg);
                        s_currentOriginalEventArg = e;
                    }
                    else
                    {
                        s_sequence.FakeEvents.Add(previewEventArg);
                        s_sequence.ScrollViewers.Add(scrollViewer);
                    }

                    // Raise the fake event on the original source, so that it is pushed all the way down the logical tree to all the 
                    // ScrollViewers (this is done recursively by each child)
                    originalSource.RaiseEvent(previewEventArg);

                    // Now that all the children ScrollViewers raised their own fake event, we can start going back up the chain
                    if (s_sequence.FakeEvents.Contains(e))
                    {
                        if (previewEventArg.Handled)
                        {
                            // If one of our children already handled our fake event, handle them all the way up
                            e.Handled = true;
                        }
                        else
                        {
                            // At this point if no one else handled the event in our children, we do our job
                            if (IsScrollSupported(scrollViewer))
                            {
                                // If this ScrollViewer is able to scroll, handle the fake events all the way up
                                e.Handled = true;
                            }

                            if (IsScrollingOutOfBounds(scrollViewer, e.Delta))
                            {
                                // If this ScrollViewer has reached its upper or lower bound, push a MouseWheelEvent on a parent
                                // ScrollViewer that can handle it
                                s_sequence.ForceMouseWheelEvent(scrollViewer, e);
                            }
                        }
                    }

                    // Remove the current instances of ScrollViewer and fake event from the sequence
                    s_sequence.FakeEvents.Remove(previewEventArg);
                    s_sequence.ScrollViewers.Remove(scrollViewer);
                    if (s_sequence.FakeEvents.Count == 0)
                    {
                        s_sequence = null;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public static bool GetFixScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(FixScrollingProperty);
        }

        public static void SetFixScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(FixScrollingProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the provided ScrollViewer can scroll, meaning if it supports scrolling and if the scrolling request is not
        /// out of bounds
        /// </summary>
        internal static bool CanScroll(ScrollViewer scrollViewer, int delta)
        {
            if (!IsScrollSupported(scrollViewer) || IsScrollingOutOfBounds(scrollViewer, delta))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the delta scroll request is out of bound for the provided ScrollViewer, meaning if we are attempting to scroll
        /// down when the scrollbar is all the way down, or up when it's all the way up.
        /// </summary>
        internal static bool IsScrollingOutOfBounds(ScrollViewer scrollViewer, int delta)
        {
            return ((delta > 0) && (scrollViewer.VerticalOffset <= 0)) ||
                   ((delta <= 0) && (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight));
        }

        /// <summary>
        /// Checks if the provided ScrollViewer supports scrolling, meaning if it has an actual scrollbar that can be used
        /// </summary>
        internal static bool IsScrollSupported(ScrollViewer scrollViewer)
        {
            return (scrollViewer.ScrollableHeight > 0) && scrollViewer.IsEnabled;
        }

        #endregion
    }

    internal sealed class ScrollViewerBehaviorSequence
    {
        #region Properties

        /// <summary>
        /// The list of all the fake events raised in the sequence
        /// </summary>
        public List<MouseWheelEventArgs> FakeEvents { get; private set; }

        /// <summary>
        /// The real original MouseWheelEvent that started the sequence
        /// </summary>
        public MouseWheelEventArgs OriginalEvent { get; private set; }

        /// <summary>
        /// The list of all the ScrollViewers involved in the sequence
        /// </summary>
        public List<ScrollViewer> ScrollViewers { get; private set; }

        #endregion

        #region Constructors

        public ScrollViewerBehaviorSequence(MouseWheelEventArgs originalEvent, ScrollViewer scrollViewer, MouseWheelEventArgs fakeEvent)
        {
            OriginalEvent = originalEvent;
            FakeEvents = new List<MouseWheelEventArgs> { fakeEvent };
            ScrollViewers = new List<ScrollViewer> { scrollViewer };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles all the events of the sequence, including the original one
        /// </summary>
        public void HandleEvents()
        {
            foreach (var fakeEvent in FakeEvents)
            {
                fakeEvent.Handled = true;
            }

            OriginalEvent.Handled = true;
        }

        public void ForceMouseWheelEvent(ScrollViewer scrollViewer, MouseWheelEventArgs e)
        {
            // Since the current ScrollViewer isn't able to scroll, push the event to the first parent who can
            int index = ScrollViewers.IndexOf(scrollViewer);
            if (index > -1)
            {
                for (int i = index - 1; i >= 0; --i)
                {
                    var parentScrollViewer = ScrollViewers[i];
                    if (ScrollViewerBehavior.CanScroll(parentScrollViewer, e.Delta))
                    {
                        e.Handled = true;

                        // Since we will trigger our own MouseWheelEvent on the right ScrollViewer,
                        // handle all the chain of preview events (fake and real)
                        HandleEvents();

                        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                        eventArg.Source = scrollViewer;
                        parentScrollViewer.RaiseEvent(eventArg);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
