using People.Models;
using People.ViewModels;

namespace People;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel(App.PersonRepo);
    }
}
