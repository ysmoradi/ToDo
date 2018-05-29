using FormsPlugin.Iconize.iOS;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Plugin.Iconize;
using Plugin.Iconize.Fonts;
using Prism;
using Prism.Ioc;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace ToDo.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            SQLitePCL.Batteries.Init();

            Forms.Init();

            IconControls.Init();

            Iconize.With(new FontAwesomeModule());

            ImageCircleRenderer.Init();

            LoadApplication(new App(new ToDoInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class ToDoInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
