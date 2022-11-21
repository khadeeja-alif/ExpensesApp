using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExpensesApp.Pages
{
    public partial class ExpensePage : ContentPage
    {
        public int FuelAmount;
        private int ParkingAmount;
        private int FoodAmount;

        public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();

        private string PhotoPath { get; set; }

        public ExpensePage()
        {
            InitializeComponent();
            ResultLabel.Text = string.Empty;
            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                UploadButton.IsVisible = false;
                ImageList.IsVisible = false;
                //MediaPicker not supported on MacOS.
            }
        }

        protected void FuelEntry_TextChanged(object sender, EventArgs e)
        {
            CalculateResult();
        }

        protected void ParkingEntry_TextChanged(object sender, EventArgs e)
        {
            CalculateResult();
        }

        protected void FoodEntry_TextChanged(object sender, EventArgs e)
        {
            CalculateResult();
        }

        private void CalculateResult()
        {
            if (CheckAmounts())
            {
                var finalAmount = FuelAmount + ParkingAmount + FoodAmount;
                ResultLabel.Text = string.Format("Total expense claim is £{0}. The highest category was {1}", finalAmount, GetGreaterExpense());
            }
            else
                ResultLabel.Text = "Amount entered are not numerical. Result can't be calculated.";
        }

        private string GetGreaterExpense()
        {
            string greaterExpense;
            if (FuelAmount > ParkingAmount)
            {
                if (FuelAmount > FoodAmount)
                    greaterExpense = "fuel";
                else
                    greaterExpense = "food";
            }
            else if (ParkingAmount > FoodAmount)
            {
                greaterExpense = "parking";
            }
            else
                greaterExpense = "food";

            return greaterExpense;
        }

        private bool CheckAmounts()
        {
            var checkFuel = int.TryParse(FuelEntry.Text, out FuelAmount);
            var checkParking = int.TryParse(ParkingEntry.Text, out ParkingAmount);
            var checkFood = int.TryParse(FoodEntry.Text, out FoodAmount);

            var isValidFuel = string.IsNullOrWhiteSpace(FuelEntry.Text) ? true : checkFuel ? true : false;
            var isValidParking = string.IsNullOrWhiteSpace(ParkingEntry.Text) ? true : checkParking ? true : false;
            var isValidFood = string.IsNullOrWhiteSpace(FoodEntry.Text) ? true : checkFood ? true : false;

            return isValidFuel && isValidParking && isValidFood;
        }

        async void UploadButton_Clicked(System.Object sender, System.EventArgs e)
        {
            var photo = await MediaPicker.PickPhotoAsync();
            await LoadPhotoAsync(photo);
            Images.Add(PhotoPath);
            OnPropertyChanged(nameof(Images));
            ImageList.ItemsSource = Images;
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
        }
    }
}
