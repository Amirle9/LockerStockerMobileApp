using essentialUIKitTry.Views;
using essentialUIKitTry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterFunctions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;
using Newtonsoft.Json.Linq;
using Plugin.LocalNotifications;
using Newtonsoft.Json;

namespace essentialUIKitTry
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseALocker : ContentPage
    {

        public List<Locker>[] lockerRows = { new List<Locker>(), new List<Locker>(), new List<Locker>(), new List<Locker>() };
        private HubConnection connection;
        private int notifacationNum = 0;
        private AuthenticationResult authenticationResult;

        UserBalance userBalance;
        public ChooseALocker(AuthenticationResult authResult)
        {
            authenticationResult = authResult;
            InitializeComponent();
            ConfigSignalR();
            GetClaims();
            
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

            connection.On<object>("Occupy", (item) =>
            {
                SetLockerList();
            });

            connection.On<object>("available", (item) =>
            {
                SetLockerList();
            });


            connection.On<object>("unlock", (item) =>
             {
                 string itemString = item.ToString();
                 Locker lockerItem = JsonConvert.DeserializeObject<Locker>(itemString);
                 if (userBalance.user_key.Equals(lockerItem.user_key))
                 {
                     CrossLocalNotifications.Current.Show("Locker Stocker", "locker " + lockerItem.Id + " got unlocked !", notifacationNum++, DateTime.Now);
                 }
             });

            connection.On<object>("setCosts", (item) =>
            {
                SetLockerList();
            });

            connection.Closed += async (exception) =>
            {
                ConfigSignalR();
            };

            try
            {
                await connection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        protected override void OnAppearing()
        {

            SetLockerList();
            base.OnAppearing();
        }

        private void GetClaims()
        {
            userBalance = new UserBalance();
            var token = authenticationResult.IdToken;
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler(); //designed for creating and validating Json Web Tokens
                var data = handler.ReadJwtToken(authenticationResult.IdToken); //Converts a string into instance of JwtSecurityToken
                var claims = data.Claims.ToList();
               
                App.m_adminMode = data.Claims.FirstOrDefault(x => x.Type.Equals("surname")).Value == "ADMIN";
                 userBalance.user_key = data.Claims.FirstOrDefault(x => x.Type.Equals("email")).Value;
                userBalance.balance = AzureApi.GetBalance(userBalance.user_key);
                if (data != null)
                {
                    if (!App.m_adminMode)
                    {
                        this.name.Text = $"Hi {data.Claims.FirstOrDefault(x => x.Type.Equals("name")).Value}!";
                        this.mid_title.Text = "Please Choose Your Locker:";
                 
                        this.balance.Text = $"You have { String.Format("{0:0.00}", userBalance.balance) } Shekels in your account.";
                   }
                    else
                    {
                        this.name.Text = $"Hi {data.Claims.FirstOrDefault(x => x.Type.Equals("name")).Value}!";
                        this.mid_title.Text = "Here are all your lockers: ";
                    }
                 
                }
            }
        }


        
        Button getBtnForLocker(Locker locker)
        {
            int btnTimingFontSize = 8;
            int btnAvailableFontSize = 12;
            int btn_width = 60;
            int btn_height = 80;
            Button tmp_btn = new Button()
            {
                Text = "L" + locker.Id,
                StyleId = "" + locker.Id,
                WidthRequest = btn_width,
                HeightRequest = btn_height,
                FontSize = btnAvailableFontSize
            };
            if ((!locker.available) && (locker.user_key == userBalance.user_key))
            {
                tmp_btn.BackgroundColor = Color.LightSteelBlue;
                tmp_btn.Padding = new Xamarin.Forms.Thickness(5, 2);
                tmp_btn.Text = "Time Remaining\n" + AzureApi.GetRemainingTime(locker);
                
                tmp_btn.FontSize = btnTimingFontSize;
            }
            else if (locker.available)
            {
                tmp_btn.BackgroundColor = Color.Green;
                tmp_btn.Text += "\n" + locker.price_per_hour + "NIS";
            }
            else
            {
                tmp_btn.BackgroundColor = Color.Red;
                if (App.m_adminMode)
                {
                    tmp_btn.Padding = new Xamarin.Forms.Thickness(5, 2);
                    tmp_btn.Text = locker.user_key + "\n" + AzureApi.GetRemainingTime(locker);
                    tmp_btn.FontSize = btnTimingFontSize;
                }
            }
            return tmp_btn;
        }

        void SetLockerList()
        {
            int numOfRows = 4;
            int lockersInRow = 5;
            ButtonsRow1.Children.Clear();
            ButtonsRow2.Children.Clear();
            ButtonsRow3.Children.Clear();
            ButtonsRow4.Children.Clear();
         
            List<Locker> lockersList = AzureApi.GetAllLockers().OrderBy(lck => lck.Id).ToList();
            for (int rowIdx = 0; rowIdx < numOfRows; rowIdx++)
            {
                lockerRows[rowIdx].Clear();
                for (int lockerInRowIdx = 0; lockerInRowIdx < lockersInRow; lockerInRowIdx++)
                {
                    Locker tmpLocker = lockersList[rowIdx * lockersInRow + lockerInRowIdx ];
                    lockerRows[rowIdx].Add(tmpLocker);
                }
            }
            for (int idxInRow = 0; idxInRow < lockersInRow; idxInRow++)
            {
                Button btn1 = getBtnForLocker(lockerRows[0][idxInRow]);
                Button btn2 = getBtnForLocker(lockerRows[1][idxInRow]);
                Button btn3 = getBtnForLocker(lockerRows[2][idxInRow]);
                Button btn4 = getBtnForLocker(lockerRows[3][idxInRow]);

                btn1.Clicked += Locker_ClickedAsync;
                btn2.Clicked += Locker_ClickedAsync;
                btn3.Clicked += Locker_ClickedAsync;
                btn4.Clicked += Locker_ClickedAsync;
                ButtonsRow1.Children.Add(btn1);
                ButtonsRow2.Children.Add(btn2);
                ButtonsRow3.Children.Add(btn3);
                ButtonsRow4.Children.Add(btn4);
            }
            if (App.m_adminMode)
            {
                ModeInfoLbl.Text = "You are logged-in as Admin";
                ModeInfoLbl.TextColor = Color.DarkGreen;
                ModeInfoLbl.FontAttributes = FontAttributes.Bold;
                balance.IsVisible = false;
                RechargeButton.IsVisible = false;
            }
            else
            {
                SetCostsButton.IsVisible = false;
            }
        }

        async void RechargeBalanceButtonClicked(object sender, System.EventArgs e)
        {
            string result = await DisplayPromptAsync("Recharge balance", "how much would you like to recharge? maximum is 999 nis", maxLength: 3 , keyboard: Keyboard.Numeric);
            int balance;
            if (result != null)
            {
                if (int.TryParse(result, out balance))
                {
                    userBalance.balance += balance;
                    AzureApi.SetUserBalance(userBalance);
                    this.balance.Text = $"You have { String.Format("{0:0.00}", userBalance.balance) } Shekels in your account.";
                }
                else
                {
                    await DisplayAlert("Failed to recharge", "Please choose value with no fraction", "OK");
                    RechargeBalanceButtonClicked(sender, e);

                }
            }

        }

        async void NavigateToCostSelectionPage(object sender, System.EventArgs e)
        {
            
            await Navigation.PushAsync(new SetCostsMainPage());
        }


        async Task<bool> setTimeToOccupyLockerAlert(Locker locker)
        {
            if (!App.m_adminMode)
            {
                if (userBalance.balance < locker.price_per_hour * 0.5)
                    return false;
                string result = await DisplayPromptAsync("occupy locker, id:" + locker.Id, "Insert time in minutes: between 30 and 999 mins", maxLength: 3, keyboard: Keyboard.Numeric);
                int intResult;
                if (result != null && int.TryParse(result, out intResult) && intResult >= 30)
                {
                    double minutesToHours = double.Parse(result) / 60;
                    if (userBalance.balance < locker.price_per_hour * minutesToHours)
                        return false;
                    bool answer = await DisplayAlert("You will be charged " + String.Format("{0:0.00}",locker.price_per_hour * minutesToHours )+ " NIS", "Would you like to continue?", "Yes", "No");
                    if (answer)
                    {
                        locker.user_key = userBalance.user_key;
                        locker.release_time = DateTimeOffset.Now.AddHours(minutesToHours);
                        AzureApi.SetOccupy(locker);
                        userBalance.balance -= (locker.price_per_hour * minutesToHours);
                        AzureApi.SetUserBalance(userBalance);
                        this.balance.Text = $"You have { String.Format("{0:0.00}", userBalance.balance) } Shekels in your account.";
                        
                        await Navigation.PushAsync(new Locker1OrderedSuccess(locker.Id, authenticationResult));
                    }
                }
                else if (result != null)
                {
                    await DisplayAlert("Wrong input", "please choose a number within the specified range with no fractions", "OK");
                    await setTimeToOccupyLockerAlert(locker);
                }
            }
            else
            {
                string result = await DisplayPromptAsync("occupy locker, id:" + locker.Id, "Insert time in minutes", maxLength: 3, keyboard: Keyboard.Numeric);
                int intResult;
                if (result != null && int.TryParse(result, out intResult))
                {
                    double minutesToHours = double.Parse(result) / 60;
                    locker.user_key = userBalance.user_key;
                    locker.release_time = DateTimeOffset.Now.AddHours(minutesToHours);
                    AzureApi.SetOccupy(locker);
                   
                    await Navigation.PushAsync(new Locker1OrderedSuccess(locker.Id, authenticationResult));
                }
                else if (result != null)
                {
                    await DisplayAlert("Wrong input", "please choose a whole number ", "OK");
                    await setTimeToOccupyLockerAlert(locker);
                }
            }
            return true;

            
        }

        async void Locker_ClickedAsync(object sender, System.EventArgs e)
        {
            int locker_id = int.Parse((sender as Button).StyleId);
            var locker = AzureApi.GetLocker(locker_id);

            if (locker.available)
            {
                
                if (!(await setTimeToOccupyLockerAlert(locker)))
                    await DisplayAlert("Please recharge", "You don't have enough money to occupy locker", "OK");
                

            }
            else if (locker.user_key == userBalance.user_key)
            {
                
                await Navigation.PushAsync(new LockerProfilePage(locker_id));
            }

            else if (!locker.available && App.m_adminMode)
            {
               
                await Navigation.PushAsync(new AdminClickedRedLockerPage(locker_id));
            }
            else
            {
                
                await Navigation.PushAsync(new Locker2OrderFailed("" + locker_id));
            }
            SetLockerList();
        }
        async void OnSignOutClicked(object sender, System.EventArgs e)
        {
            AuthenticationResult result;
            try
            {

                Navigation.RemovePage(this);
                IEnumerable<IAccount> accounts = await App.AuthenticationClient.GetAccountsAsync();
                await App.AuthenticationClient.RemoveAsync(accounts.FirstOrDefault());
                await connection.StopAsync();
                await Navigation.PushAsync(new SimpleLoginPage());
            }
            catch (MsalClientException)
            {
            }
        }
    }
}