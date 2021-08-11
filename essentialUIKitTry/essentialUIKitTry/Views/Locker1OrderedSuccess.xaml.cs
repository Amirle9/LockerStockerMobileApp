using essentialUIKitTry.ViewModels;
using Microsoft.Identity.Client;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show the payment success.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Locker1OrderedSuccess : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Locker1OrderedSuccess" /> class.
        /// </summary>
        int lockerId;
        AuthenticationResult authenticationResult;
        public Locker1OrderedSuccess(int lockerId, AuthenticationResult authenticationResult)
        {
            if (authenticationResult is null)
            {
                throw new ArgumentNullException(nameof(authenticationResult));
            }
            else
            {
                this.authenticationResult = authenticationResult;
            }

            this.InitializeComponent();
            this.BindingContext = PaymentViewModel.BindingContext;
            this.lockerId = lockerId;
            lbl1.Text = "LOCKER ID: " + lockerId;
            lbl2.Text = "You have successfully ordered locker " +  lockerId + ". Enjoy!";
        }
        async void OrderProfileClicked(object sender, System.EventArgs e)
        {
            if (!await AzureApi.IsAvailableAsync(lockerId))
            {
                await Navigation.PushAsync(new LockerProfilePage(this.lockerId));
            }
            else
            {
                await DisplayAlert("This Locker has been released", "cannot complete action", "OK");
                await Navigation.PopAsync(); 

            }
        }
        async void Back_To_Menu_Clicked(object sender, EventArgs e)
        {
 
            try
            {
               await Navigation.PopAsync();
            }
            catch (MsalClientException)
            {

            }
        }
    }
}