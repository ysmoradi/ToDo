using Android.App;
using Android.Content.PM;
using Android.OS;
using FormsPlugin.Iconize.Droid;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.Iconize;
using Plugin.Iconize.Fonts;
using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace ToDo.Droid
{
    [Activity(Label = "ToDo", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.tabs;
            ToolbarResource = Resource.Layout.toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            IconControls.Init(Resource.Id.toolbar);

            Iconize.With(new FontAwesomeModule());

            ImageCircleRenderer.Init();

            LoadApplication(new App(new ToDoInitializer()));

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
    }

    public class ToDoInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}

