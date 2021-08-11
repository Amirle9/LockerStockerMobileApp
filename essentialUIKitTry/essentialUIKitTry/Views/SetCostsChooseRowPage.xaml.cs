using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CounterFunctions;
using Xamarin.Forms;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show chat profile page
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetCostsChooseRowPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetCostsMainPage" /> class.
        /// </summary>

        public SetCostsChooseRowPage()
        {
            this.InitializeComponent();
            SetPageView();
        }
        async void SetPageView()
        {
            Label placeholderLbl = new Label();
            Label instructionsLbl = new Label();
            placeholderLbl.Text = "";
            instructionsLbl.Text = "For which row of lockers do you want to set a new price?";

            Button setCostRow1Btn = new Button()
            {
                Text = "1st Row"
            };
            setCostRow1Btn.Clicked += HandleSetCostRow1Btn;
            Button setCostRow2Btn = new Button()
            {
                Text = "2nd Row"
            };
            setCostRow2Btn.Clicked += HandleSetCostRow2Btn;
            Button setCostRow3Btn = new Button()
            {
                Text = "3rd Row"
            };
            setCostRow3Btn.Clicked += HandleSetCostRow3Btn;
            Button setCostRow4Btn = new Button()
            {
                Text = "4th Row"
            };
            setCostRow4Btn.Clicked += HandleSetCostRow4Btn;

            MainStack.Children.Add(placeholderLbl);
            MainStack.Children.Add(instructionsLbl);
            MainStack.Children.Add(setCostRow1Btn);
            MainStack.Children.Add(setCostRow2Btn);
            MainStack.Children.Add(setCostRow3Btn);
            MainStack.Children.Add(setCostRow4Btn);
        }
        
        async void HandleSetCostRow1Btn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsByRowPage(1));
        }
        async void HandleSetCostRow2Btn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsByRowPage(2));
        }
        async void HandleSetCostRow3Btn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsByRowPage(3));
        }
        async void HandleSetCostRow4Btn(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SetCostsByRowPage(4));
        }
    }
}