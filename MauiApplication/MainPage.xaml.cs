namespace MauiApplication
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel viewModel;

        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
            BindingContext = viewModel;
        }
    }

}
