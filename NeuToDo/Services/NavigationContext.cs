using Xamarin.Forms;

namespace NeuToDo.Services
{
    /// <summary>
    /// 导航上下文。
    /// </summary>
    public class NavigationContext
    {
        /// <summary>
        /// 导航参数属性。
        /// </summary>
        public static readonly BindableProperty NavigationParameterProperty =
            BindableProperty.CreateAttached("NavigationParameter", typeof(object), typeof(NavigationContext), null,
                BindingMode.OneWayToSource);

        /// <summary>
        /// 设置导航参数。
        /// </summary>
        /// <param name="bindableObject">需要设置导航参数的对象。</param>
        /// <param name="value">导航参数。</param>
        public static void SetParameter(BindableObject bindableObject, object value) =>
            bindableObject.SetValue(NavigationParameterProperty, value);
    }
}