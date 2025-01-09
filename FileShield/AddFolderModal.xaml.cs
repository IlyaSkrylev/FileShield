using Microsoft.Maui.Controls;

namespace FileShield
{
    public partial class AddFolderModal : ContentPage
    {
        public string FolderName { get; private set; }
        public bool IsCancelled { get; private set; }

        public AddFolderModal()
        {
            InitializeComponent();
            IsCancelled = true;
        }

        private async void OnCreateClicked(object sender, System.EventArgs e)
        {
            FolderName = FolderNameEntry.Text;
            IsCancelled = false;
            await Navigation.PopModalAsync();
        }

        private void OnCancelClicked(object sender, System.EventArgs e)
        {
            IsCancelled = true;
            Navigation.PopModalAsync();
        }
        private void FolderNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FolderNameEntry.Text))
            {
                Create.IsEnabled = false;
                return;
            }
            Create.IsEnabled = true;  
        }
    }
}
