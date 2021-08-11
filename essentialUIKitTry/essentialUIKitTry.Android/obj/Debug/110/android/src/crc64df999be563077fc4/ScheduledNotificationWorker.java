package crc64df999be563077fc4;


public class ScheduledNotificationWorker
	extends androidx.work.Worker
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_doWork:()Landroidx/work/ListenableWorker$Result;:GetDoWorkHandler\n" +
			"";
		mono.android.Runtime.register ("Plugin.LocalNotification.Platform.Droid.ScheduledNotificationWorker, Plugin.LocalNotification", ScheduledNotificationWorker.class, __md_methods);
	}


	public ScheduledNotificationWorker (android.content.Context p0, androidx.work.WorkerParameters p1)
	{
		super (p0, p1);
		if (getClass () == ScheduledNotificationWorker.class)
			mono.android.TypeManager.Activate ("Plugin.LocalNotification.Platform.Droid.ScheduledNotificationWorker, Plugin.LocalNotification", "Android.Content.Context, Mono.Android:AndroidX.Work.WorkerParameters, Xamarin.AndroidX.Work.Runtime", this, new java.lang.Object[] { p0, p1 });
	}


	public androidx.work.ListenableWorker.Result doWork ()
	{
		return n_doWork ();
	}

	private native androidx.work.ListenableWorker.Result n_doWork ();

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
