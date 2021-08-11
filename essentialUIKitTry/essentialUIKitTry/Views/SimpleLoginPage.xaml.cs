using Android.Content.Res;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to login with user name and password
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimpleLoginPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLoginPage" /> class.
        /// </summary>
        public SimpleLoginPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                // Look for existing account
                //var accounts = await App.AuthenticationClient.GetAccountsAsync();
                 IEnumerable<IAccount> accounts = await App.AuthenticationClient.GetAccountsAsync();
                if (accounts.Count() >= 1)
                    this.SignInButton.IsVisible = false;
                else
                {
                    this.SignInButton.IsVisible = true;
                }
                //if (accounts.Count() >= 1)
                //{
                    AuthenticationResult result = await App.AuthenticationClient
                    .AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();

                    await Navigation.PushAsync(new ChooseALocker(result));
                //}
            }
            catch
            {
                // Do nothing - the user isn't logged in
            }
            base.OnAppearing();
        }



        async void OnSignInClicked(object sender, EventArgs e)
        {
            AuthenticationResult result;

            try
            {
                result = await App.AuthenticationClient
                    .AcquireTokenInteractive(Constants.Scopes)
                    //.WithPrompt(Prompt.ForceLogin)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(App.UIParent)
                    .ExecuteAsync();

                await Navigation.PushAsync(new ChooseALocker(result));
            }
            catch (MsalClientException ex)
            {
                if (ex.Message != null && ex.Message.Contains("AADB2C90118"))
                {
                    result = await OnForgotPassword();      
                    await Navigation.PushAsync(new ChooseALocker(result));
                }
               // else if (ex.Message != null && ex.Message.Contains("AADB2C90091"))
               // {
                //    await Navigation.PopAsync();
                //}
                else if (ex.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert("An error has occurred", "Exception message: " + ex.Message, "Dismiss");
                }

            }
        }



        async Task<AuthenticationResult> OnForgotPassword()
        {

            try
            {
                return await App.AuthenticationClient
                    .AcquireTokenInteractive(Constants.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(App.UIParent)
                    .WithB2CAuthority(Constants.policyPassword)
                    .ExecuteAsync();
            }
            //catch (MsalException)
            catch (MsalException)
            {
                // Do nothing - ErrorCode will be displayed in OnLoginButtonClicked
                return null;
            }
        }

    }
}