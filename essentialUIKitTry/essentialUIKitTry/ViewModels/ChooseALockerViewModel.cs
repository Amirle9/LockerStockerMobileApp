using essentialUIKitTry.Validators;
using essentialUIKitTry.Validators.Rules;
using Xamarin.Forms.Internals;

namespace essentialUIKitTry.ViewModels
{
    /// <summary>
    /// ViewModel for login page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ChooseALockerViewModel : BaseViewModel
    {
        #region Fields

        public string Color1 = "green";
        public string Color2 = "green";
        public string Color3 = "green";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="LoginViewModel" /> class.
        /// </summary>
        public ChooseALockerViewModel()
        {
            //this.InitializeProperties();
            //this.AddValidationRules();
            Color1 = "green";
            Color2 = "green";
            Color3 = "green";
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the email ID from user in the login page.
        /// </summary>
        /*public ValidatableObject<string> Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (this.email == value)
                {
                    return;
                }

                this.SetProperty(ref this.email, value);
            }
        }*/
        #endregion

        #region Methods

        /// <summary>
        /// This method to validate the email
        /// </summary>
        /// <returns>returns bool value</returns>
        /*public bool IsEmailFieldValid()
        {
            bool isEmailValid = this.Email.Validate();
            return isEmailValid;
        }

        /// <summary>
        /// Initializing the properties.
        /// </summary>
        private void InitializeProperties()
        {
            this.Email = new ValidatableObject<string>();
        }

        /// <summary>
        /// This method contains the validation rules
        /// </summary>
        private void AddValidationRules()
        {
            this.Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Email Required" });
            this.Email.Validations.Add(new IsValidEmailRule<string> { ValidationMessage = "Invalid Email" });
        }*/

        #endregion
    
    
    
    
    
    }
}
