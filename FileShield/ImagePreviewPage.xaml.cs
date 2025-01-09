using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static FileShield.Cryptography;

namespace FileShield
{
    public partial class ImagePreviewPage : ContentPage
    {
        private string filePath;
        private List<string> files;
        private int index;

        private string password;
        private double _currentScale = 1;
        private double _startScale = 1;
        private double _xOffset = 0;
        private double _yOffset = 0;
        private double _startX = 0;
        private double _startY = 0;
        public ImagePreviewPage(List<string> files, string filePath, string password)
        {
            InitializeComponent();
            this.filePath = filePath;
            this.password = password;
            this.files = files;
            this.index = TakeIndex();

            ShowImage();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => OnClose(); 
            mainGrid.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private int TakeIndex()
        {
            int i = 0;
            foreach (var file in files)
            {
                if (file == filePath)
                {
                    index = i;
                    break;
                }
                i++;
            }
            return index;
        }

        private void ShowImage()
        {
            byte[] encodedFile = File.ReadAllBytes(filePath);
            byte[] plainFile = DecodeFile(encodedFile, password);

            var img = this.FindByName<Image>("fullScreenImage");
            img.Source = ImageSource.FromStream(() => new MemoryStream(plainFile));
        }
        private async void OnClose()
        {
            await Navigation.PopModalAsync();
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                _startScale = _currentScale;
                fullScreenImage.AnchorX = e.ScaleOrigin.X;
                fullScreenImage.AnchorY = e.ScaleOrigin.Y;
            }
            if (e.Status == GestureStatus.Running)
            {
                _currentScale = _startScale * e.Scale;
                _currentScale = Math.Max(1, _currentScale); 
                fullScreenImage.Scale = _currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                _startScale = _currentScale;
            }
        }
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (_currentScale > 1)
            {

                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        _startX = e.TotalX;
                        _startY = e.TotalY;
                        break;

                    case GestureStatus.Running:
                        double x = (e.TotalX - _startX) + _xOffset;
                        double y = (e.TotalY - _startY) + _yOffset;
                        fullScreenImage.TranslationX = x;
                        fullScreenImage.TranslationY = y;
                        break;

                    case GestureStatus.Completed:
                        _xOffset = fullScreenImage.TranslationX;
                        _yOffset = fullScreenImage.TranslationY;
                        break;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is string imageUrl)
            {
                fullScreenImage.Source = ImageSource.FromFile(imageUrl);
            }
        }

        private void OnSwipedLeft(object sender, EventArgs e)
        {
            if (files.ElementAt(files.Count - 1) == filePath) 
                return;

            index++;
            filePath = files.ElementAt(index);
            ShowImage();
        }

        private void OnSwipedRight(object sender, EventArgs e)
        {
            if (files.ElementAt(0) == filePath)
                return;

            index--;
            filePath = files.ElementAt(index);
            ShowImage();
        }
    }
}

