package mono.com.google.firebase.database.core;


public class EventRegistrationZombieListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.firebase.database.core.EventRegistrationZombieListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onZombied:(Lcom/google/firebase/database/core/EventRegistration;)V:GetOnZombied_Lcom_google_firebase_database_core_EventRegistration_Handler:Firebase.Database.Core.IEventRegistrationZombieListenerInvoker, Xamarin.Firebase.Database\n" +
			"";
		mono.android.Runtime.register ("Firebase.Database.Core.IEventRegistrationZombieListenerImplementor, Xamarin.Firebase.Database", EventRegistrationZombieListenerImplementor.class, __md_methods);
	}


	public EventRegistrationZombieListenerImplementor ()
	{
		super ();
		if (getClass () == EventRegistrationZombieListenerImplementor.class)
			mono.android.TypeManager.Activate ("Firebase.Database.Core.IEventRegistrationZombieListenerImplementor, Xamarin.Firebase.Database", "", this, new java.lang.Object[] {  });
	}


	public void onZombied (com.google.firebase.database.core.EventRegistration p0)
	{
		n_onZombied (p0);
	}

	private native void n_onZombied (com.google.firebase.database.core.EventRegistration p0);

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
