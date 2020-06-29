using Android.Content;
using NeuToDo.Components;
using NeuToDo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRender))]

namespace NeuToDo.Droid
{
    public class CustomPickerRender : PickerRenderer
    {
        public CustomPickerRender
            (Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}