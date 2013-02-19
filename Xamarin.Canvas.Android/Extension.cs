using System;
using Android.Content.Res;
using Android.Graphics.Drawables;
using System.Reflection;
using System.Linq;
using Android.Graphics;

namespace Xamarin.Canvas.Android
{
	public static class Extension
	{
		public static Drawable GetDrawable (this Resources res, string name)
		{
			return res.GetDrawable(IdFromTitle(name));
		}
		public static Bitmap GetBitmap (this Resources res, string name)
		{
			return BitmapFactory.DecodeResource (res, IdFromTitle (name));
		}
		static int IdFromTitle (string title)
		{
			var name = System.IO.Path.GetFileNameWithoutExtension (title);
			int id = GetId (typeof(Resource.Drawable), name);
			return id;// Resources.System.GetDrawable (Resource.Drawable.dashboard);
		}
		static int GetId (Type type, string propertyName)
		{
			FieldInfo[] props = type.GetFields ();
			FieldInfo prop = props.Select (p => p).Where (p => p.Name == propertyName).FirstOrDefault ();
			if (prop != null)
				return (int)prop.GetValue (type);
			return 0;
		}
	}
}

