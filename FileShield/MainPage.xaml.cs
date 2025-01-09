using System.Runtime.InteropServices;
using static FileShield.Constants;

namespace FileShield
{
    public partial class MainPage : ContentPage
    {
        private AccountFileHandler accountFileHandler;
        public MainPage()
        {
            InitializeComponent();
            CreateFiles(new List<string>() { accountDataFilePath });
            //DeleteFiles(new List<string>() { accountDataFilePath });
        }

        private async void OnRegistrationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegPage());
        }

        private void CreateFiles(List<string> filePaths)
        {
            string folderPath = FileSystem.Current.AppDataDirectory;
            foreach (string filePath in filePaths)
            {
                string path = Path.Combine(folderPath, filePath);

                if (!File.Exists(path))
                {
                    using (var stream = new FileStream(path, FileMode.CreateNew))
                    {
                    }
                }
            }
        }

        private void DeleteFiles(List<string> filePaths)
        {
            string folderPath = FileSystem.Current.AppDataDirectory;
            foreach (string filePath in filePaths)
            {
                string path = Path.Combine(folderPath, filePath);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private async void onAuthClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
            {
                ShowErrorMessage(UsernameEntry, "Пожалуйста, введите имя пользователя.");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ShowErrorMessage(PasswordEntry, "Пожалуйста, введите пароль.");
                return;
            }

            string userName = UsernameEntry.Text.Trim();
            string password = PasswordEntry.Text.Trim();
            accountFileHandler = new AccountFileHandler(userName, password);

            if (password.Length < 8)
            {
                ShowErrorMessage(PasswordEntry, "Длина пароля должна быть больше 7 символов.");
                return;
            }

            if (!accountFileHandler.CheckAuthorizationData())
            {
                ShowErrorMessage(UsernameEntry, "Неверный логин или пароль.");
                return;
            }

            ErrorLabel.IsVisible = false;
            UsernameEntry.Text = "";
            PasswordEntry.Text = "";
            await Navigation.PushAsync(new FileField(userName, password));
        }

        private void HideErrorLabel(object sender, EventArgs e)
        {
            var obj = sender as Entry;
            if (obj != null)
            {
                obj.PlaceholderColor = Color.FromArgb("#AAAAAA");
            }
            ErrorLabel.IsVisible = false;
        }

        private void ShowErrorMessage(object sender, string errorMessage)
        {
            var obj = sender as Entry;
            if (obj != null)
            {
                obj.PlaceholderColor = Colors.Red;
                ErrorLabel.Text = errorMessage;
                ErrorLabel.IsVisible = true;
            }
        }
    }
}