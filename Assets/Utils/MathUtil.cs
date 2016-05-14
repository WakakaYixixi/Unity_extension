using UnityEngine;

public class MathUtil {

	/// <summary>
	/// 判断相交
	/// </summary>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	public static bool Intersect(ref Rect a,ref Rect b ) {
		FlipNegative( ref a );
		FlipNegative( ref b );
		bool c1 = a.xMin < b.xMax;
		bool c2 = a.xMax > b.xMin;
		bool c3 = a.yMin < b.yMax;
		bool c4 = a.yMax > b.yMin;
		return c1 && c2 && c3 && c4;
	}

	/// <summary>
	/// 反转
	/// </summary>
	/// <param name="r">The red component.</param>
	public static void FlipNegative(ref Rect r) {
		if( r.width < 0 ) 
			r.x -= ( r.width *= -1 );
		if( r.height < 0 )
			r.y -= ( r.height *= -1 );
	}


}
