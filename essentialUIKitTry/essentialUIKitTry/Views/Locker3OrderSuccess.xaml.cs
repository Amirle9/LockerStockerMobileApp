using essentialUIKitTry.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show the payment success.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Locker3OrderSuccess : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Locker3OrderSuccess" /> class.
        /// </summary>
        public Locker3OrderSuccess()
        {
            this.InitializeComponent();
            this.BindingContext = PaymentViewModel.BindingContext;
        }
    }
}