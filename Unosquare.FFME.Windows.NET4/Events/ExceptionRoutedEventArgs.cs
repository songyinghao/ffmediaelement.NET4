using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Unosquare.FFME.Events
{
    /// <summary>
    /// Holds the error event arguments for media failed events from a MediaElement.
    /// Since other MediaFailed events from the MediaClock or MediaTimeline are not
    /// routed events, this is separated out into a different class.
    /// </summary>
    public sealed class ExceptionRoutedEventArgs : RoutedEventArgs
    {
        internal
        ExceptionRoutedEventArgs(
            RoutedEvent routedEvent,
            object sender,
            Exception errorException
            ) : base(routedEvent, sender)
        {
            if (errorException == null)
            {
                throw new ArgumentNullException("errorException");
            }

            _errorException = errorException;
        }

        /// <summary>
        /// The exception that describes the media failure.
        /// </summary>
        public Exception ErrorException
        {
            get
            {
                return _errorException;
            }
        }

        private Exception _errorException;
    };

}
