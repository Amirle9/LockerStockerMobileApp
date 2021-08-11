using essentialUIKitTry.ViewModels;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Azure.Storage.Blobs;
using System.IO;

namespace essentialUIKitTry.Views
{
    /// <summary>
    /// Page to show the something went wrong
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InsideALockerImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsideALockerImage" /> class.
        /// </summary>
        public InsideALockerImage(string lockerId)
        {
            this.InitializeComponent();
            this.BindingContext = InsideALockerImageViewModel.BindingContext;

            // TODO: if not downloaded - download
            var externalStorage = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var lockersPicturesPath = Path.Combine(externalStorage, "Pictures//LockerStocker");
            var imagePath = Path.Combine(lockersPicturesPath, "LockerStocker_" + lockerId + ".jpeg");
            this.lockerImage.Source = ImageSource.FromFile(imagePath);

        }
        void Back_To_Locker_Profile(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}