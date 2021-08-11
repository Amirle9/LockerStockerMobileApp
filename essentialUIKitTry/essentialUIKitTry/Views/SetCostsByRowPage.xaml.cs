using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CounterFunctions;
using Xamarin.Forms;
using System;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show chat profile page
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetCostsByRowPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetCostsMainPage" /> class.
        /// </summary>

        private Entry newCostEntry;
        private Label instructionLbl;
        private int rowNum;
        public SetCostsByRowPage(int rowNum)
        {
            this.rowNum = rowNum;
            this.InitializeComponent();
            SetPageView(rowNum);
        }
        async void SetPageView(int rowNum)
        {
            instructionLbl = new Label()
            {
                Text = "Insert new cost in NIS, for row" + rowNum + ":"
            };
            newCostEntry = new Entry();
            Button approvalBtn = new Button()
            {
                Text = "Set new cost",
                TextColor = Color.White,
                BackgroundColor = Color.Green
            };
            approvalBtn.Clicked += HandleApprovalBtn;
            MainStack.Children.Add(instructionLbl);
            MainStack.Children.Add(newCostEntry);
            MainStack.Children.Add(approvalBtn);
        }
        
        void HandleApprovalBtn(object sender, System.EventArgs e)
        {
            AzureApi.SetCostsByRow(rowNum, Int32.Parse(newCostEntry.Text));
            Navigation.PopAsync();
        }
    }
}