using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Interfaces;

namespace NeuToDo.Models
{
    /// <summary>
    /// Wrapper to allow change the dot color
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DayEventCollection<T> : List<T>, IPersonalizableDayEvent
    {
        public DayEventCollection() : base()
        {
        }

        /// <summary>
        /// Color constructor extends from base()
        /// </summary>
        /// <param name="eventIndicatorColor"></param>
        /// <param name="eventIndicatorSelectedColor"></param>
        public DayEventCollection(Color? eventIndicatorColor, Color? eventIndicatorSelectedColor) : base()
        {
            EventIndicatorColor = eventIndicatorColor;
            EventIndicatorSelectedColor = eventIndicatorSelectedColor;
        }

        /// <summary>
        /// IEnumerable constructor extends from base(IEnumerable collection)
        /// </summary>
        /// <param name="collection"></param>
        public DayEventCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Capacity constructor extends from base(int capacity)
        /// </summary>
        /// <param name="capacity"></param>
        public DayEventCollection(int capacity) : base(capacity)
        {
        }

        #region PersonalizableProperties

        public Color? EventIndicatorColor { get; set; }
        public Color? EventIndicatorSelectedColor { get; set; }
        public Color? EventIndicatorTextColor { get; set; }
        public Color? EventIndicatorSelectedTextColor { get; set; }

        #endregion
    }
}