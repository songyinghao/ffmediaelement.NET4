﻿namespace Unosquare.FFME.Core
{
    using Primitives;
    using Shared;
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// A time measurement artifact.
    /// </summary>
    internal sealed class RealTimeClock : IDisposable
    {
        private readonly Stopwatch Chrono = new Stopwatch();
        private ISyncLocker Locker = SyncLockerFactory.CreateSlim();
        private double OffsetMilliseconds = 0;
        private double m_SpeedRatio = Constants.Controller.DefaultSpeedRatio;
        private bool IsDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeClock"/> class.
        /// The clock starts poaused and at the 0 position.
        /// </summary>
        public RealTimeClock()
        {
            Reset();
        }

        /// <summary>
        /// Gets or sets the clock position.
        /// </summary>
        public TimeSpan Position
        {
            get
            {
                using (Locker.AcquireReaderLock())
                {
                    return TimeSpan.FromTicks((long)Math.Round(
                        (OffsetMilliseconds + (Chrono.ElapsedMilliseconds * SpeedRatio)) * TimeSpan.TicksPerMillisecond, 0));
                }
            }
            set
            {
                using (Locker.AcquireWriterLock())
                {
                    var resume = Chrono.IsRunning;
                    Chrono.Reset();
                    OffsetMilliseconds = value.TotalMilliseconds;
                    if (resume) Chrono.Start();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the clock is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                using (Locker.AcquireReaderLock())
                {
                    return Chrono.IsRunning;
                }
            }
        }

        /// <summary>
        /// Gets or sets the speed ratio at which the clock runs.
        /// </summary>
        public double SpeedRatio
        {
            get
            {
                using (Locker.AcquireReaderLock())
                {
                    return m_SpeedRatio;
                }
            }
            set
            {
                using (Locker.AcquireWriterLock())
                {
                    if (value < 0d) value = 0d;

                    // Capture the initial position se we set it even after the speedratio has changed
                    // this ensures a smooth position transition
                    var initialPosition = Position;
                    m_SpeedRatio = value;
                    Position = initialPosition;
                }
            }
        }

        /// <summary>
        /// Starts or resumes the clock.
        /// </summary>
        public void Play()
        {
            using (Locker.AcquireWriterLock())
            {
                if (Chrono.IsRunning) return;
                Chrono.Start();
            }
        }

        /// <summary>
        /// Pauses the clock.
        /// </summary>
        public void Pause()
        {
            using (Locker.AcquireWriterLock())
            {
                Chrono.Stop();
            }
        }

        /// <summary>
        /// Sets the clock position to 0 and stops it.
        /// The speed ratio is not modified.
        /// </summary>
        public void Reset()
        {
            using (Locker.AcquireWriterLock())
            {
                OffsetMilliseconds = 0;
                Chrono.Reset();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="alsoManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool alsoManaged)
        {
            if (!IsDisposed)
            {
                if (alsoManaged)
                {
                    Locker?.Dispose();
                }

                Locker = null;
                IsDisposed = true;
            }
        }
    }
}
