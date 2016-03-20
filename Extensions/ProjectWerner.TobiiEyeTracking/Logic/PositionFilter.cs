using System.Collections.Generic;
using System.Windows;

namespace ProjectWerner.TobiiEyeTracking.Logic
{
    /// <summary>
    /// A simple filter over a number of recent gaze points to get more stable mouse movements
    /// </summary>
    class PositionFilter
    {
        private readonly Queue<Point> _queue = new Queue<Point>();

        /// <summary>
        /// FilterSize is the number of positions that are averaged to the filtered position
        /// </summary>
        private int FilterSize { get; set; }

        public PositionFilter()
        {
            // todo: experiment with timestamp based filtering to get more reliable results (filter size 10 worked fine, but Tobii makes no guarantee about the stream frequency)
            FilterSize = 10;
        }

        /// <summary>
        /// Adds a raw position point that will be used to calculate a filtered position.
        /// Note: this function automatically removes all unneeded positions.
        /// </summary>
        public void AddRawPosition(Point point)
        {
            lock (_queue)
            {
                _queue.Enqueue(point);

                // remove unneeded positions from the queue
                while (_queue.Count > FilterSize)
                    _queue.Dequeue();
            }
        }

        /// <summary>
        /// Calculates and returns a filtered position point
        /// </summary>
        /// <returns>The position point or null, if no points are available</returns>
        public Point? GetFilteredPosition()
        {
            // todo: experiment with eliminating statistical outliers (German "statistische Ausreißer")
            return GetMeanPoint();
        }

        /// <summary>
        /// A simple averaging filter (statistical mean value)
        /// </summary>
        private Point? GetMeanPoint()
        {
            Point point = new Point();
            lock (_queue)
            {
                if (_queue.Count == 0)
                    return null;
                foreach (var p in _queue)
                {
                    point.X += p.X;
                    point.Y += p.Y;
                }
                point.X /= _queue.Count;
                point.Y /= _queue.Count;
            }
            return point;
        }
    }
}
