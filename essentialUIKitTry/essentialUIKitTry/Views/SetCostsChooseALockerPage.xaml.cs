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

namespace essentialUIKitTry
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetCostsChooseALockerPage : ContentPage
    {
        public List<Locker>[] lockerRows = {new List<Locker>(), new List<Locker>(), new List<Locker>(), new List<Locker>() };

        public SetCostsChooseALockerPage()
        {
            InitializeComponent();
            SetLockerList();
        }


        Button getBtnForLocker(Locker locker)
        {
            int btnTimingFontSize=8;
            int btn_width = 60;
            int btn_height = 80;
            Button tmp_btn = new Button()
            {
                StyleId = "" + locker.Id,
                WidthRequest = btn_width,
                HeightRequest = btn_height,
                TextColor = Color.Black,
                BorderWidth = 1,
                BorderColor = Color.Black
            };
            tmp_btn.BackgroundColor = Color.White;
            tmp_btn.Padding = new Xamarin.Forms.Thickness(5,2);
            tmp_btn.Text = "L" + locker.Id + "\n" + locker.price_per_hour + "NIS";
            tmp_btn.FontSize = btnTimingFontSize;
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
            //ChooseALockerMainStack.Children.Clear();

            List<Locker> lockersList = AzureApi.GetAllLockers();
            for (int rowIdx = 0; rowIdx < numOfRows; rowIdx++)
            {
                lockerRows[rowIdx].Clear();
                for (int lockerInRowIdx = 0; lockerInRowIdx < lockersInRow; lockerInRowIdx++)
                {
                    Locker tmpLocker = lockersList[rowIdx * lockersInRow + lockerInRowIdx];
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
        }


        protected override void OnAppearing()
        {

            SetLockerList();
            base.OnAppearing();
        }
        async void NavigateToCostSelectionPage(object sender, System.EventArgs e) {
            await Navigation.PushAsync(new SetCostsMainPage());
        }

        async void Locker_ClickedAsync(object sender, System.EventArgs e)
        {
            int locker_id= int.Parse((sender as Button).StyleId);
            await Navigation.PushAsync(new SetCostsByLockerPage(locker_id));
        }
    }
}