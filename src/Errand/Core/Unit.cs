namespace Errand.Core;

/// <summary>
/// Represents a void type, since Void is not a valid return type in C#.
/// "There is only one Unit, and it does not share power." - Unit type
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    /// <summary>
    /// Default and only value of the Unit type.
    /// </summary>
    public static readonly Unit Value = new();

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object</param>
    /// <returns>Always returns 0</returns>
    public int CompareTo(Unit other) => 0;

    /// <summary>
    /// Compares the current instance with another object of the same type. 
    /// </summary>
    /// <param name="obj">An object to compare with this instance</param>
    /// <returns>Always returns 0</returns>
    int IComparable.CompareTo(object? obj) => 0;

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object</param>
    /// <returns>Always returns true</returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether the specified object is equal to the current object. 
    /// </summary>
    /// <param name="obj">The object to compare with the current object</param>
    /// <returns>True if the specified object is a Unit type</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for this instance. 
    /// </summary>
    /// <returns>Always returns 0</returns>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the unit value as "()"</returns>
    public override string ToString() => "()";

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Unit left, Unit right) => false;
}