using Xamarin.Forms;
using TouchEffect;

namespace NeuToDo.Components
{
    public class RippleStackLayout : StackLayout
    {
        public RippleStackLayout()
        {
            SetValue(TouchEff.RegularBackgroundColorProperty, Color.Transparent);
            SetValue(TouchEff.PressedBackgroundColorProperty, Color.Gray);
            SetValue(TouchEff.RippleCountProperty, 1);
            SetValue(TouchEff.PressedAnimationDurationProperty, 100);
            SetValue(TouchEff.RegularAnimationDurationProperty, 100);
        }
    }
}