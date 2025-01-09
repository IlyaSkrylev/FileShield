using Microsoft.Maui.Layouts;
using System.IO.Enumeration;
using System.Security.Cryptography;
using System.Text;
using static FileShield.Constants;
using static FileShield.Cryptography;

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
            ShowFiles();
        }

        private async void OnAddFolderClicked(object sender, EventArgs e)
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
                    DemonstrateFolder(folderName);
                }
            };
            await Navigation.PushModalAsync(modalPage);
        }

        private async void OnAddFileClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickMultipleAsync();

            if (result != null)
            {
                List<string> selectedFiles = result.Select(file => file.FullPath).ToList();

                foreach (string filePath in selectedFiles)
                {
                    CreateFile(filePath);
                    DemonstrateFile(Path.GetFileName(filePath));
                }
            }
        }

        private void OnBackArrowClicked(object sender, EventArgs e)
        {
            directories.RemoveAt(directories.Count - 1);

            var imgField = this.FindByName<FlexLayout>("imageFlexLayout");
            imgField.Clear();

            ShowFolders();
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

            var stackLayout = new StackLayout
            {
                WidthRequest = 100,
                HeightRequest = 120,
                Margin = new Thickness(10),
                
            };

            var folder = new Image
            {
                Source = imgFolderIcon,
                WidthRequest = 100,
                HeightRequest = 100,
            };

            var text = new Label
            {
                Text = folderName,
                HorizontalOptions = LayoutOptions.Center,
                FontFamily = "Courier New",
                FontSize = 14
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => OnFolderClicked(Path.GetFileName(folderName));
            folder.GestureRecognizers.Add(tapGestureRecognizer);

            imgField.Children.Add(stackLayout);
            stackLayout.Children.Add(folder);
            stackLayout.Children.Add(text);
        }

        private void OnFolderClicked(string folderName)
        {
            directories.Add(EncodeString(folderName,password));

            var imgField = this.FindByName<FlexLayout>("imageFlexLayout");
            imgField.Clear();
            ShowFolders();
            ShowFiles();
        }

        private async void CreateFile(string path)
        {
            string encryptedFileCaption = EncodeString(Path.GetFileNameWithoutExtension(path), password);
            string newFilePath = PathToFile(encryptedFileCaption + Path.GetExtension(path));

            byte[]? data = EncodeFile(File.ReadAllBytes(path), password);
            if (data == null) 
                return;

            await File.WriteAllBytesAsync(newFilePath, data);
        }

        private void DemonstrateFile(string fileName)
        {
            var imgField = this.FindByName<FlexLayout>("imageFlexLayout");

            var stackLayout = new StackLayout
            {
                WidthRequest = 100,
                HeightRequest = 120,
                Margin = new Thickness(10),
            };

            var file = new Image
            {
                Source = imgFolderIcon,
                WidthRequest = 100,
                HeightRequest = 100,
            };

            string encryptedFileName = EncodeString(Path.GetFileNameWithoutExtension(fileName), password) + Path.GetExtension(fileName);
            string filePath = PathToFile(encryptedFileName);
            byte[] encodedFile = File.ReadAllBytes(filePath);
            byte[] plainFile = DecodeFile(encodedFile, password);
            file.Source = ImageSource.FromStream(() => new MemoryStream(plainFile));

            var text = new Label
            {
                Text = Path.GetFileNameWithoutExtension(fileName),
                HorizontalOptions = LayoutOptions.Center,
                FontFamily = "Courier New",
                FontSize = 14
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => OnFileClicked(filePath);
            file.GestureRecognizers.Add(tapGestureRecognizer);

            imgField.Children.Add(stackLayout);
            stackLayout.Children.Add(file);
            stackLayout.Children.Add(text);
        }

        private async void OnFileClicked(string filePath)
        {
            string[] files = Directory.GetFiles(PathToDirectory());
            var modalPage = new ImagePreviewPage(files.ToList(), filePath, password);
            await Navigation.PushModalAsync(modalPage);
        }

        private void ShowFiles()
        {
            string[] files = Directory.GetFiles(PathToDirectory());
            foreach (string f in files)
            {
                string fileName = DecodeString(Path.GetFileNameWithoutExtension(f), password) + Path.GetExtension(f);
                DemonstrateFile(fileName);
            }
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
    }
}
