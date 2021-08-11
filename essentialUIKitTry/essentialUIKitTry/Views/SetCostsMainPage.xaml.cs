using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CounterFunctions;
using Xamarin.Forms;
using essentialUIKitTry;
using static Android.App.VoiceInteractor;
using Microsoft.Identity.Client;
using Prompt = Microsoft.Identity.Client.Prompt;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show chat profile page
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetCostsMainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetCostsMainPage" /> class.
        /// </summary>

        public SetCostsMainPage()
        {
            this.InitializeComponent();
            SetPageView();
        }
        async void SetPageView()
        {
            Label placeholderLbl = new Label();
            placeholderLbl.Text = "";
            Button setCostByLockerBtn = new Button()
            {
                Text = "Set Cost Of a Single Locker"
            };
            setCostByLockerBtn.Clicked += HandleSetCostByLockerBtn;
            Button setCostByRowBtn = new Button()
            {
                Text = "Set Cost Of a Row"
            };
            setCostByRowBtn.Clicked += HandleSetCostByRowBtn;
            Button setCostGloballyBtn = new Button()
            {
                Text = "Set Cost Of All Lockers"
            };
            setCostGloballyBtn.Clicked += HandleSetCostGloballyBtn;

            

            MainStack.Children.Add(placeholderLbl);
            MainStack.Children.Add(setCostByLockerBtn);
            MainStack.Children.Add(setCostByRowBtn);
            MainStack.Children.Add(setCostGloballyBtn);
            
        }

        async void HandleSetCostByLockerBtn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsChooseALockerPage());
        }
        async void HandleSetCostByRowBtn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsChooseRowPage());
        }
        async void HandleSetCostGloballyBtn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsGloballyPage());
        }
       // async void BackToLockers(object sender, System.EventArgs e)
       // {
         //   AuthenticationResult result;

         //   try
         //   {
        //        result = await App.AuthenticationClient
              //      .AcquireTokenInteractive(Constants.Scopes)
                    //.WithPrompt(Prompt.ForceLogin)
              //      .WithPrompt(Prompt.SelectAccount)
                 //   .WithParentActivityOrWindow(App.UIParent)
                  //  .ExecuteAsync();

            //    await Navigation.PushAsync(new ChooseALocker(result));
        //    }
          //  catch (MsalClientException ex)
        //    {
        //    }
      //  }
    }
}