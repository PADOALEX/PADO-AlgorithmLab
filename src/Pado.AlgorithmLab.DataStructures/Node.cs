namespace Pado.AlgorithmLab.DataStructures;

/// <summary>A user-implemented node; its links are only writable by the library.</summary>
public sealed class Node<T>(T value)
{
    public T Value { get; } = value;
    public Node<T>? Previous { get; internal set; }
    public Node<T>? Next { get; internal set; }
}
