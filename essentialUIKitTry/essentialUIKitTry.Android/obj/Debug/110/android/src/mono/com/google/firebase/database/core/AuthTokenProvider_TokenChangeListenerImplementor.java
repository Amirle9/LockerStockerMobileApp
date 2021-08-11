package mono.com.google.firebase.database.core;


public class AuthTokenProvider_TokenChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.firebase.database.core.AuthTokenProvider.TokenChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTokenChange:()V:GetOnTokenChangeHandler:Firebase.Database.Core.IAuthTokenProviderTokenChangeListenerInvoker, Xamarin.Firebase.Database\n" +
			"n_onTokenChange:(Ljava/lang/String;)V:GetOnNamedTokenChange_Ljava_lang_String_Handler:Firebase.Database.Core.IAuthTokenProviderTokenChangeListenerInvoker, Xamarin.Firebase.Database\n" +
			"";
		mono.android.Runtime.register ("Firebase.Database.Core.IAuthTokenProviderTokenChangeListenerImplementor, Xamarin.Firebase.Database", AuthTokenProvider_TokenChangeListenerImplementor.class, __md_methods);
	}


	public AuthTokenProvider_TokenChangeListenerImplementor ()
	{
		super ();
		if (getClass () == AuthTokenProvider_TokenChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Firebase.Database.Core.IAuthTokenProviderTokenChangeListenerImplementor, Xamarin.Firebase.Database", "", this, new java.lang.Object[] {  });
	}


	public void onTokenChange ()
	{
		n_onTokenChange ();
	}

	private native void n_onTokenChange ();


	public void onTokenChange (java.lang.String p0)
	{
		n_onTokenChange (p0);
	}

	private native void n_onTokenChange (java.lang.String p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
