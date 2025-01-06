namespace FileShield
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnRegistrationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegPage());

        }
    }

}
