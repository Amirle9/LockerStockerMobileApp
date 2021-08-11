package mono.com.google.firebase.database.core;


public class SyncTree_CompletionListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.firebase.database.core.SyncTree.CompletionListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onListenComplete:(Lcom/google/firebase/database/DatabaseError;)Ljava/util/List;:GetOnListenComplete_Lcom_google_firebase_database_DatabaseError_Handler:Firebase.Database.Core.SyncTree/ICompletionListenerInvoker, Xamarin.Firebase.Database\n" +
			"";
		mono.android.Runtime.register ("Firebase.Database.Core.SyncTree+ICompletionListenerImplementor, Xamarin.Firebase.Database", SyncTree_CompletionListenerImplementor.class, __md_methods);
	}


	public SyncTree_CompletionListenerImplementor ()
	{
		super ();
		if (getClass () == SyncTree_CompletionListenerImplementor.class)
			mono.android.TypeManager.Activate ("Firebase.Database.Core.SyncTree+ICompletionListenerImplementor, Xamarin.Firebase.Database", "", this, new java.lang.Object[] {  });
	}


	public java.util.List onListenComplete (com.google.firebase.database.DatabaseError p0)
	{
		return n_onListenComplete (p0);
	}

	private native java.util.List n_onListenComplete (com.google.firebase.database.DatabaseError p0);

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
