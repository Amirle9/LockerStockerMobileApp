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
    public partial class SetCostsByLockerPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetCostsByLockerPage" /> class.
        /// </summary>

        private Entry newCostEntry;
        private Label instructionLbl;
        private int lockerId;
        public SetCostsByLockerPage(int lockerId)
        {
            this.lockerId = lockerId;
            this.InitializeComponent();
            SetPageView(lockerId);
        }
        void SetPageView(int lockerId)
        {
            instructionLbl = new Label()
            {
                Text = "Insert new cost in NIS, for locker" + lockerId + ":"
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
            AzureApi.SetCostsByLockerId(lockerId, Int32.Parse(newCostEntry.Text));
            Navigation.PopAsync();
        }
    }
}