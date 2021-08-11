using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CounterFunctions;
using Xamarin.Forms;
using Plugin.LocalNotification;
using System;
using Plugin.LocalNotifications;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Java.Lang;
using Newtonsoft.Json.Linq;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show chat profile page
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LockerProfilePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockerProfilePage" /> class.
        /// </summary>

        Locker locker;
        HubConnection connection;
        public LockerProfilePage(int lockerId)
        {
            this.InitializeComponent();

            InitializeLocker(lockerId);
            //ConfigSignalR();
            initializePageComponent();
            SetLocker(lockerId);
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


            connection.On<object>("photoReady", (item) =>
            {
                if (locker.Id == Integer.ParseInt(item.ToString()))
                {
                    AzureApi.TakeLockerCameraPhoto(locker.Id + "");
                    locker.photo_taken = true;
                    AzureApi.setPhotoTaken(locker);
                    SetLocker(locker.Id);
                }

            });
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
            await DisplayAlert("This Locker has been released", "cannot complete action", "OK");

        }

        private void initializePageComponent()
        {
            int btnTakephotoFontSize = 12;
            int btn_width = 160;
            int btn_height = 45;
            LockUnlockBtn.Text = this.locker.locked ? "Unlock" : "Lock";
            LockUnlockBtn.WidthRequest = btn_width;
            LockUnlockBtn.HeightRequest = btn_height;
            LockUnlockBtn.BackgroundColor = Color.LightSteelBlue;
            LockUnlockBtn.Padding = new Xamarin.Forms.Thickness(5, 2);
            LockUnlockBtn.FontSize = btnTakephotoFontSize;
            LockUnlockBtn.Clicked += HandleLockUnlockBtn;

            TakeAPhotoBtn.Text = "Take Photo :)";
            TakeAPhotoBtn.WidthRequest = btn_width;
            TakeAPhotoBtn.HeightRequest = btn_height;
            TakeAPhotoBtn.BackgroundColor = Color.LightSteelBlue;
            TakeAPhotoBtn.Padding = new Xamarin.Forms.Thickness(5, 2);
            TakeAPhotoBtn.FontSize = btnTakephotoFontSize;
            TakeAPhotoBtn.Clicked += HandleTakeAPhotoBtn;

            ReleaseBtn.Text = "Release Locker";
            ReleaseBtn.WidthRequest = btn_width;
            ReleaseBtn.HeightRequest = btn_height;
            ReleaseBtn.BackgroundColor = Color.LightSteelBlue;
            ReleaseBtn.Padding = new Xamarin.Forms.Thickness(5, 2);
            ReleaseBtn.FontSize = btnTakephotoFontSize;
            ReleaseBtn.Clicked += HandleReleaseBtn;


        }

        private void InitializeLocker(int lockerId)
        {
            this.locker = AzureApi.GetLocker(lockerId);
        }

        private void SetLocker(int lockerId)
        {

            LastPhotoLinkLabel.Text = "No Photo Available";
            if (locker.photo_taken)
            {
                LastPhotoLinkLabel.Text = "Last photo of the locker";
            }


            string status = "not set";

            LockerIdLbl.Text = "Locker Id: " + lockerId;


            string timeRemainingStr = AzureApi.GetRemainingTime(locker);
            TimeRemainingLbl.Text = timeRemainingStr;


            if (this.locker.locked) status = "Locked";
            else status = "Unlocked";
            StatusLbl.Text = "Status: " + status;


            LockUnlockBtn.Text = locker.locked ? "Unlock" : "Lock";

        }
        async void HandleTakeAPhotoBtn(object sender, System.EventArgs e)
        {
            if (!locker.available)
            {
                try
                {
                    await AzureApi.sendSignalToTakePhoto(locker.Id);
                }
                catch
                {
                    //  await DisplayAlert("Failed to Take photo", "check with Admin", "OK");

                }

            }


        }
        void HandleLockUnlockBtn(object sender, System.EventArgs e)
        {
            if (!locker.available)
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
        }
        async void HandleReleaseBtn(object sender, System.EventArgs e)
        {
            if (!locker.available)
            {
                bool answer = true;
                if (!App.m_adminMode)
                    answer = await DisplayAlert("You will not be refunded for the remaining time on the clock", "are you sure you want to release the locker", "Yes", "No");
                if (answer)
                {
                    AzureApi.SetAvailable(this.locker);
                    this.locker.available = true;
                    this.locker.Id = 0;
                    await connection.StopAsync();
                    await Navigation.PopAsync();
                }
            }

        }

        async void Navigate_To_Photo(object sender, System.EventArgs e)
        {
            if (!locker.available && locker.photo_taken)
            {

                await connection.StopAsync();
                await Navigation.PushAsync(new InsideALockerImage(this.locker.Id + ""));

            }
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


    }
}