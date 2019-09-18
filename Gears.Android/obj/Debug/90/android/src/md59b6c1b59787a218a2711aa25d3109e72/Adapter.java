package md59b6c1b59787a218a2711aa25d3109e72;


public class Adapter
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_GetObject:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Gears.Droid.Services.Adapter, Gears.Android", Adapter.class, __md_methods);
	}


	public Adapter ()
	{
		super ();
		if (getClass () == Adapter.class)
			mono.android.TypeManager.Activate ("Gears.Droid.Services.Adapter, Gears.Android", "", this, new java.lang.Object[] {  });
	}

	@android.webkit.JavascriptInterface

	public void GetObject ()
	{
		n_GetObject ();
	}

	private native void n_GetObject ();

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
