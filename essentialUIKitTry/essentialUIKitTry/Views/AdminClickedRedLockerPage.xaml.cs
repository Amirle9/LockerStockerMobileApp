using CounterFunctions;
using System.Drawing;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Java.Lang;
using Newtonsoft.Json;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show chat profile page
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminClickedRedLockerPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminClickedRedLockerPage" /> class.
        /// </summary>
        private Locker locker;
        HubConnection connection;
        public AdminClickedRedLockerPage(int lockerId)
        {
            this.InitializeComponent();
            InitializeLocker(lockerId);
            InitializePageComponent();
            SetLocker(lockerId);
        }

        private void InitializePageComponent()
        {

            LockUnlockBtn.Clicked += HandleLockUnlockBtn;
            ReleaseBtn.Text = "Release Locker";
            ReleaseBtn.Clicked += HandleReleaseBtn;
        }

        private void InitializeLocker(int lockerId)
        {
            locker = AzureApi.GetLocker(lockerId);
        }

        void SetLocker(int lockerId)
        {

            string status = "not set";

            LockerIdLbl.Text = "Locker Id: " + lockerId;

            this.mailOfUser.Text = $"Mail of Customer: {locker.user_key}";

            string timeRemainingStr = AzureApi.GetRemainingTime(locker);
            TimeRemainingLbl.Text = "Remaing time: " + timeRemainingStr;


            if (this.locker.locked) status = "Locked";
            else status = "Unlocked";
            StatusLbl.Text = "Status: " + status;

            LockUnlockBtn.Text = this.locker.locked ? "Unlock" : "Lock";

        }


        async void ConfigSignalR()
        {
            var results = await AzureApi.Negotiate();
            JObject json = JObject.Parse(results);
            var url = json["url"].ToString();
            var token = json["accessToken"].ToString();

            connection = new HubConnectionBuilder().WithUrl(url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
             .WithAutomaticReconnect().Build();



            connection.On<object>("unlock", (item) =>
            {
                string itemString = item.ToString();
                Locker lockerItem = JsonConvert.DeserializeObject<Locker>(itemString);
                if (locker.Id == lockerItem.Id)
                {
                    this.locker.locked = false;
                    SetLocker(locker.Id);

                }

            });

            connection.On<object>("lock", (item) =>
            {
                string itemString = item.ToString();
                Locker lockerItem = JsonConvert.DeserializeObject<Locker>(itemString);
                if (locker.Id == lockerItem.Id)
                {
                    this.locker.locked = true;
                    SetLocker(locker.Id);

                }

            });


            connection.On<object>("available", (item) =>
            {
                string itemString = item.ToString();
                Locker lockerItem = JsonConvert.DeserializeObject<Locker>(itemString);
                if (locker.Id == lockerItem.Id)
                {
                    locker.available = true;
                    locker.Id = 0;
                    displayLockerRealeasedAlert();
                    connection.StopAsync();
                    Navigation.PopAsync();
                }

            });

            try
            {
                await connection.StartAsync();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private async void displayLockerRealeasedAlert()
        {
            await DisplayAlert("This Locker has been released", "you will be sent back to previous page", "OK");

        }

        protected override void OnDisappearing()
        {

            connection.StopAsync();
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {

            ConfigSignalR();
            base.OnAppearing();
        }
        void HandleLockUnlockBtn(object sender, System.EventArgs e)
        {
            if (!this.locker.locked)
            {
                AzureApi.SetLock(this.locker);
                this.locker.locked = true;
            }
            else
            {
                AzureApi.SetUnlock(this.locker);
                this.locker.locked = false;
            }
            SetLocker(this.locker.Id);
        }
        async void HandleReleaseBtn(object sender, System.EventArgs e)
        {
            this.locker.available = true;
            AzureApi.SetAvailable(locker);
            await Navigation.PopAsync();

        }

    }
}