using essentialUIKitTry.ViewModels;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show the no item
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Locker1OrderFailed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Locker1OrderFailed" /> class.
        /// </summary>
        public Locker1OrderFailed()
        {
            this.InitializeComponent();
            this.BindingContext = Locker1OrderFailedViewModel.BindingContext;
        }
        void Back_To_Menu(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}