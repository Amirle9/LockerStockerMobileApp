using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace essentialUIKitTry.Services
{
    public interface IFirebaseAuth
    {
        Task<string> LoginWithEmailAndPassword(string email, string password);

        bool SignOut();

        bool IsSignIn();
    }
}
