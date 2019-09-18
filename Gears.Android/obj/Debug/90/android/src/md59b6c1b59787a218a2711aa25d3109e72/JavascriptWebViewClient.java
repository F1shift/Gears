package md59b6c1b59787a218a2711aa25d3109e72;


public class JavascriptWebViewClient
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPageStarted:(Landroid/webkit/WebView;Ljava/lang/String;Landroid/graphics/Bitmap;)V:GetOnPageStarted_Landroid_webkit_WebView_Ljava_lang_String_Landroid_graphics_Bitmap_Handler\n" +
			"";
		mono.android.Runtime.register ("Gears.Droid.Services.JavascriptWebViewClient, Gears.Android", JavascriptWebViewClient.class, __md_methods);
	}


	public JavascriptWebViewClient ()
	{
		super ();
		if (getClass () == JavascriptWebViewClient.class)
			mono.android.TypeManager.Activate ("Gears.Droid.Services.JavascriptWebViewClient, Gears.Android", "", this, new java.lang.Object[] {  });
	}

	public JavascriptWebViewClient (java.lang.String p0)
	{
		super ();
		if (getClass () == JavascriptWebViewClient.class)
			mono.android.TypeManager.Activate ("Gears.Droid.Services.JavascriptWebViewClient, Gears.Android", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public void onPageStarted (android.webkit.WebView p0, java.lang.String p1, android.graphics.Bitmap p2)
	{
		n_onPageStarted (p0, p1, p2);
	}

	private native void n_onPageStarted (android.webkit.WebView p0, java.lang.String p1, android.graphics.Bitmap p2);

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
