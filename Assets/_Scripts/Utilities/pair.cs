using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pair<T, U> : IEquatable<pair<T, U>>
{
    public T first { get; set; }
    public U second { get; set; }

    public pair(T first, U second)
    {
        this.first = first;
        this.second = second;
    }

    public bool Equals(pair<T, U> other)
    {
        if (other == null)
            return false;

        return EqualityComparer<T>.Default.Equals(first, other.first) &&
               EqualityComparer<U>.Default.Equals(second, other.second);
    }

    public override bool Equals(object obj)
    {
        return obj is pair<T, U> pair && Equals(pair);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(first, second);
    }

    public override string ToString()
    {
        return $"({first}, {second})";
    }

    public static bool operator ==(pair<T, U> left, pair<T, U> right)
    {
        return EqualityComparer<pair<T, U>>.Default.Equals(left, right);
    }

    public static bool operator !=(pair<T, U> left, pair<T, U> right)
    {
        return !(left == right);
    }

    public void Deconstruct(out T first, out U second)
    {
        first = this.first;
        second = this.second;
    }
}