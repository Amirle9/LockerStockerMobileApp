package mono.com.google.firebase.database.core;


public class AuthTokenProvider_GetTokenCompletionListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.firebase.database.core.AuthTokenProvider.GetTokenCompletionListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onError:(Ljava/lang/String;)V:GetOnError_Ljava_lang_String_Handler:Firebase.Database.Core.IAuthTokenProviderGetTokenCompletionListenerInvoker, Xamarin.Firebase.Database\n" +
			"n_onSuccess:(Ljava/lang/String;)V:GetOnSuccess_Ljava_lang_String_Handler:Firebase.Database.Core.IAuthTokenProviderGetTokenCompletionListenerInvoker, Xamarin.Firebase.Database\n" +
			"";
		mono.android.Runtime.register ("Firebase.Database.Core.IAuthTokenProviderGetTokenCompletionListenerImplementor, Xamarin.Firebase.Database", AuthTokenProvider_GetTokenCompletionListenerImplementor.class, __md_methods);
	}


	public AuthTokenProvider_GetTokenCompletionListenerImplementor ()
	{
		super ();
		if (getClass () == AuthTokenProvider_GetTokenCompletionListenerImplementor.class)
			mono.android.TypeManager.Activate ("Firebase.Database.Core.IAuthTokenProviderGetTokenCompletionListenerImplementor, Xamarin.Firebase.Database", "", this, new java.lang.Object[] {  });
	}


	public void onError (java.lang.String p0)
	{
		n_onError (p0);
	}

	private native void n_onError (java.lang.String p0);


	public void onSuccess (java.lang.String p0)
	{
		n_onSuccess (p0);
	}

	private native void n_onSuccess (java.lang.String p0);

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
