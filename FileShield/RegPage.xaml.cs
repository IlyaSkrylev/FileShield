using System.Diagnostics.Metrics;

namespace FileShield;

public partial class RegPage : ContentPage
{
	public RegPage()
	{
		InitializeComponent();
	}
    private void onRegClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
        {
            ShowErrorMessage(UsernameEntry, "Пожалуйста, введите имя пользователя.");
            return;
        }

        if (isAccountExists(UsernameEntry.Text))
        {
            ShowErrorMessage(UsernameEntry, "Аккаунт уже существует.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ShowErrorMessage(PasswordEntry, "Пожалуйста, введите пароль.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordRepeat.Text)) {
            ShowErrorMessage(PasswordRepeat, "Пожалуйста, повторите пароль.");
            return;
        }

        if (PasswordEntry.Text.Length < 8)
        {
            ShowErrorMessage(PasswordEntry, "Длина пароля должна быть больше 7 символов.");
            return;
        }

        if (PasswordEntry.Text != PasswordRepeat.Text)
        {
            ShowErrorMessage(PasswordRepeat, "Пароли не совпадают.");
            return;
        }
    }

    private void HideErrorLabel(object sender, EventArgs e)
    {
        var obj = sender as Entry;
        if (obj != null) { 
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

    private bool isAccountExists(string login)
    {
        return false;
    }
}