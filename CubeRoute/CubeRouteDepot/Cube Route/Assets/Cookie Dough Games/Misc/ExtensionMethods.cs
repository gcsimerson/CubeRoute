using UnityEngine;

/// <summary>
/// This class adds additional behavior to built-in classes.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Reset position to 0, rotation to 0, and scale to 1.
    /// </summary>
    /// <param name="trans"></param>
    public static void ResetTransformation( this Transform trans )
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    /// <summary>
    /// Sets the x/y/z local scales to a uniform value.
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="scale"></param>
    public static void SetLocalScale( this Transform trans, float scale )
    {
        trans.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// Uses the x,y coordinates of the Vector3 to make a new Vector2.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector2 xy( this Vector3 vector )
    {
        return new Vector2( vector.x, vector.y );
    }

    /// <summary>
    /// Uses the x,z coordinates of the Vector3 to make a new Vector2.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector2 xz( this Vector3 vector )
    {
        return new Vector2( vector.x, vector.z );
    }

    /// <summary>
    /// Uses the y,z coordinates of the Vector3 to make a new Vector2.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector2 yz( this Vector3 vector )
    {
        return new Vector2( vector.y, vector.z );
    }

    /// <summary>
    /// Sets the RGB color values using integers in the range of 0 - 255.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="alpha"></param>
    public static void SetRGB256( this Color color, int red = 255, int green = 255, int blue = 255, int alpha = 255 )
    {
        color.r = Mathf.Clamp01( red / 255f );
        color.g = Mathf.Clamp01( green / 255f );
        color.b = Mathf.Clamp01( blue / 255f );
        color.a = Mathf.Clamp01( alpha / 255f );
    }

    /// <summary>
    /// Treating this float as the starting time: how much time has elapsed since then.
    /// </summary>
    /// <param name="startTime"></param>
    /// <returns></returns>
    public static float TimeElapsedSince( this float startTime )
    {
        return Time.time - startTime;
    }

    public static float? TimeElapsedSince( this float? startTime )
    {
        return Time.time - startTime;
    }

    /// <summary>
    /// Treating this float as the starting time: what is the time difference between the end time given as the parameter (result may be negative).
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public static float TimeDifference( this float startTime, float endTime )
    {
        return endTime - startTime;
    }

    /// <summary>
    /// Returns the *absolute* time difference between two timestamps (never negative).
    /// </summary>
    /// <param name="time1"></param>
    /// <param name="time2"></param>
    /// <returns></returns>
    public static float TimeBetween( this float time1, float time2 )
    {
        return Mathf.Abs( time2 - time1 );
    }

    /// <summary>
    /// Determine how many horizontal degrees are between where the primary transform is currently facing and the given position.
    /// y-values are ignored for this calculation.
    /// </summary>
    /// <param name="primary"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static float FacingAngleBetween( this Transform primary, Vector3 position )
    {
        position.y = primary.position.y;

        return Vector3.Angle( primary.forward, position - primary.position );
    }
}