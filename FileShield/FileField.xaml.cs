using System.Security.Cryptography;
using System.Text;
using static FileShield.Constants;

namespace FileShield
{

    public partial class FileField : ContentPage
    {
        private string login;
        private string password;
        private List<string> directories;

        public FileField(string login, string password)
        {
            InitializeComponent();

            this.login = login;
            this.password = password;
            directories = new List<string>();
            directories.Add(login);

            ShowFolders();
        }

        private async void onAddFolderClicked(object sender, EventArgs e)
        {
            var modalPage = new AddFolderModal();
            modalPage.Unloaded += (_, _) =>
            {
                if (!modalPage.IsCancelled)
                {
                    string folderName = modalPage.FolderName;
                    string EncryptedFolderName = EncodeString(folderName, password);
                    if (FolderExists(EncryptedFolderName))
                        return;
                    Directory.CreateDirectory(PathToFile(EncryptedFolderName));
                }
            };
            await Navigation.PushModalAsync(modalPage);
        }

        private bool FolderExists(string folderName)
        {
            string pathToFolder = PathToFile(folderName);
            if (Directory.Exists(pathToFolder))
                return true;
            return false;
        }

        private void ShowFolders()
        {
            string[] directories = Directory.GetDirectories(PathToDirectory());
            foreach (string d in directories)
            {
                string directoryName = DecodeString(Path.GetFileName(d), password);
                DemonstrateFolder(directoryName);
            }
        }

        private void DemonstrateFolder(string folderName)
        {
            var imgField = this.FindByName<FlexLayout>("imageFlexLayout");

            var image = new Image
            {
                Source = imgFolderIcon,
                WidthRequest = 100,
                HeightRequest = 100,
                Margin = new Thickness(10)
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => OnFolderClicked(Path.GetFileName(folderName));
            image.GestureRecognizers.Add(tapGestureRecognizer);

            imgField.Children.Add(image);
        }

        private void OnFolderClicked(string folderName)
        {

        }

        private string PathToDirectory()
        {
            string directoryPath = FileSystem.Current.AppDataDirectory;
            foreach (string directory in directories)
            {
                directoryPath = Path.Combine(directoryPath, directory);
            }
            return directoryPath;
        }

        private string PathToFile(string fileName)
        {
            return Path.Combine(PathToDirectory(), fileName);
        }

        public static string EncodeString(string input, string key)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key))
                return string.Empty;

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encodedBytes = new byte[inputBytes.Length];

            for (int i = 0; i < inputBytes.Length; i++)
            {
                encodedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encodedBytes);
        }

        public static string DecodeString(string input, string key)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key))
                return string.Empty;

            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decodedBytes = new byte[inputBytes.Length];

            for (int i = 0; i < inputBytes.Length; i++)
            {
                decodedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}
