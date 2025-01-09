using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;

namespace FileShield
{
    public partial class RegPage : ContentPage
    {
        private AccountFileHandler accountFileHandler;

        public RegPage()
        {
            InitializeComponent();
        }

        private async void onRegClicked(object sender, EventArgs e)
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

            if (string.IsNullOrWhiteSpace(PasswordRepeat.Text))
            {
                ShowErrorMessage(PasswordRepeat, "Пожалуйста, повторите пароль.");
                return;
            }

            string userName = UsernameEntry.Text.Trim();
            string password = PasswordEntry.Text.Trim();
            string repPassword = PasswordRepeat.Text.Trim();
            accountFileHandler = new AccountFileHandler(userName, password);

            if (accountFileHandler.isAccountExists())
            {
                ShowErrorMessage(UsernameEntry, "Аккаунт уже существует.");
                return;
            }

            if (password.Length < 8)
            {
                ShowErrorMessage(PasswordEntry, "Длина пароля должна быть больше 7 символов.");
                return;
            }

            if (password != repPassword)
            {
                ShowErrorMessage(PasswordRepeat, "Пароли не совпадают.");
                return;
            }

            accountFileHandler.CreateNewAccount();
            UsernameEntry.Text = "";
            PasswordEntry.Text = "";
            PasswordRepeat.Text = "";
            await Navigation.PopAsync();
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