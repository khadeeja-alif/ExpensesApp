using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExpensesApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            CheckUser();
            LoginButton.IsEnabled = false;
        }

        private async void CheckUser()
        {
            try
            {
                var username = await SecureStorage.GetAsync("Username");
                if (!string.IsNullOrWhiteSpace(username))
                {
                    Username.Text = username;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Username couldn't be retrieved from Secure Storage. Device not supported");
            }
        }

        async void OnLogin(System.Object sender, System.EventArgs e)
        {
            if (Password.Text.ToLower() == "password")
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(await SecureStorage.GetAsync("Username")))
                    {
                        SecureStorage.Remove("Username");
                    }

                    await SecureStorage.SetAsync("Username", Username.Text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Username couldn't be stored on Secure Storage. Device not supported");
                }
                await Navigation.PushAsync(new ExpensePage());
            }
            else
                await DisplayAlert("Login Failed", "Sorry, your password is incorrect. Please Try Again", "OK");
        }

        protected void Username_TextChanged(object sender, EventArgs e)
        {
            CheckLogin();
        }

        protected void Password_TextChanged(object sender, EventArgs e)
        {
            CheckLogin();
        }

        void CheckLogin()
        {
            if (!string.IsNullOrWhiteSpace(Username.Text) &&
                !string.IsNullOrWhiteSpace(Password.Text))
            {
                LoginButton.IsEnabled = true;
            }
            else
                LoginButton.IsEnabled = false;
        }
    }
}
