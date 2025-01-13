#if __IOS__
using Foundation;
using MauiApplication.Models;
#endif

namespace MauiApplication
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            if(App.Current.MainPage is AppShell appShell)
            {
                if(appShell.CurrentPage.BindingContext is MainPageViewModel mainPageViewModel)
                {
                    #if __IOS__

                    var FileManager = new NSFileManager();
                    var appGroupContainer = FileManager.GetContainerUrl("group.com.mauiapp");
                    var appGroupContainerPath = appGroupContainer.Path;
                    var files = Directory.GetFiles(appGroupContainerPath);
                    var query = uri.Query;

                    List<Item> items = new List<Item>();
                    if (query[0].Equals('?'))
                        query = query.Substring(1, (query.Length - 1));
                    string[] parameters = query.Split('&');
                    foreach (var item in parameters)
                    {
                        string decodedItem = Uri.UnescapeDataString(item);
                        FileInfo fileInfo = new FileInfo(decodedItem);
                        items.Add(new Item() { Path = decodedItem, Size = fileInfo.Length });
                    }

                    mainPageViewModel.Items = items;

                    #endif
                }
            }
        }
    }
}
