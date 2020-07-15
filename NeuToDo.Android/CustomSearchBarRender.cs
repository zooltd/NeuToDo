using Android.Content;
using Android.Widget;
using NeuToDo.Components;
using NeuToDo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomSearchBar), typeof(CustomSearchBarRender))]

namespace NeuToDo.Droid
{
    public class CustomSearchBarRender : SearchBarRenderer
    {
        public CustomSearchBarRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            if (Control == null) return;
            var searchView = Control;
            searchView.Iconified = true;
            searchView.SetIconifiedByDefault(false);
            int searchIconId = Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
            var icon = searchView.FindViewById(searchIconId);
            (icon as ImageView)?.SetImageResource(Resource.Drawable.search_white);
            
            //remove borderline
            var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
            var plate = Control.FindViewById(plateId);
            plate.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}