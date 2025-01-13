using Foundation;
using UIKit;

namespace MauiApplication
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            Microsoft.Maui.Controls.Application.Current.SendOnAppLinkRequestReceived(new Uri(url.AbsoluteString));
            return true;

            //return base.OpenUrl(application, url, options);
        }
    }
}
