package md5a6214798b54a77ffaae91ebf993268bc;


public class JSBridge
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_Notify:(Ljava/lang/String;)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Gears.Droid.Custom.Renderer.JSBridge, Gears.Android", JSBridge.class, __md_methods);
	}


	public JSBridge ()
	{
		super ();
		if (getClass () == JSBridge.class)
			mono.android.TypeManager.Activate ("Gears.Droid.Custom.Renderer.JSBridge, Gears.Android", "", this, new java.lang.Object[] {  });
	}

	public JSBridge (md5a6214798b54a77ffaae91ebf993268bc.MyWebViewRenderer p0)
	{
		super ();
		if (getClass () == JSBridge.class)
			mono.android.TypeManager.Activate ("Gears.Droid.Custom.Renderer.JSBridge, Gears.Android", "Gears.Droid.Custom.Renderer.MyWebViewRenderer, Gears.Android", this, new java.lang.Object[] { p0 });
	}

	@android.webkit.JavascriptInterface

	public void Notify (java.lang.String p0)
	{
		n_Notify (p0);
	}

	private native void n_Notify (java.lang.String p0);

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
