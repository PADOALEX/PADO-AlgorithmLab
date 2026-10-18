using Pado.AlgorithmLab.DataStructures;
using Xunit;

namespace Pado.AlgorithmLab.Tests;

public sealed class DoublyLinkedListTests
{
    [Fact]
    public void EmptyList_HasNoNodes_AndThrowsForRemoval()
    {
        var list = new DoublyLinkedList<int>();
        list.AssertInvariants();
        Assert.Equal(0, list.Count);
        Assert.Null(list.Head);
        Assert.Null(list.Tail);
        Assert.Empty(list);
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
        Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(0));
        Assert.False(list.Remove(5));
    }

    [Fact]
    public void SingleNode_RemainsCorrectAfterBothRemovalModes()
    {
        var list = new DoublyLinkedList<string>();
        var node = list.AddFirst("a");
        Assert.Same(node, list.Head);
        Assert.Same(node, list.Tail);
        Assert.Null(node.Next);
        Assert.Null(node.Previous);
        Assert.Equal("a", list.RemoveLast());
        list.AssertInvariants();
        list.AddLast("b");
        Assert.Equal("b", list.RemoveFirst());
        list.AssertInvariants();
    }

    [Fact]
    public void HeadTailMiddle_AndBounds_AreCorrect()
    {
        var list = new DoublyLinkedList<int>();
        list.AddFirst(2);
        list.AddHead(1);
        list.AddTail(4);
        list.AddLast(5);
        list.AssertInvariants();
        Assert.Equal(new[] { 1, 2, 4, 5 }, list.ToArray());
        Assert.Equal(4, list[2]);
        Assert.Equal(4, list.RemoveAt(2));
        Assert.True(list.Remove(1));
        Assert.False(list.Remove(100));
        Assert.Equal(5, list.RemoveLast());
        Assert.Equal(new[] { 2 }, list.ToArray());
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(2));
        list.AssertInvariants();
    }

    [Fact]
    public void ClearAndClone_DetachAllNodes_AndKeepCopiesIndependent()
    {
        var list = new DoublyLinkedList<int>(new[] { 1, 2, 3 });
        var first = list.Head!;
        var middle = first.Next!;
        var last = list.Tail!;
        var clone = list.CloneManual();
        Assert.NotSame(first, clone.Head);
        list.Clear();
        list.AssertInvariants();
        clone.AssertInvariants();
        Assert.Null(first.Next);
        Assert.Null(middle.Previous);
        Assert.Null(middle.Next);
        Assert.Null(last.Previous);
        Assert.Equal(new[] { 1, 2, 3 }, clone.ToArray());
    }

    [Fact]
    public void Enumerator_RejectsConcurrentStructuralChanges()
    {
        var list = new DoublyLinkedList<int>(new[] { 1, 2 });
        using var enumerator = list.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        list.AddLast(3);
        Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
    }

    [Fact]
    public void Average_Empty_AndIntBounds()
    {
        var list = new DoublyLinkedList<int>();
        Assert.False(Variant9Algorithms.TryCalculateAverageManual(list, out var sum, out _));
        Assert.Equal(0, sum);
        list.AddTail(int.MaxValue);
        list.AddTail(int.MinValue);
        Assert.True(Variant9Algorithms.TryCalculateAverageManual(list, out sum, out var average));
        Assert.Equal(-1, sum);
        Assert.Equal(-0.5, average);
        list.AssertInvariants();
    }

    [Fact]
    public void AdjacentDuplicates_PreserveSurvivingNodeIdentity()
    {
        var list = new DoublyLinkedList<int>(new[] { 7, 7, 8, 7, 7 });
        var first = list.Head!;
        var removed = first.Next!;
        var middle = removed.Next!;
        var lastSurvivor = middle.Next!;
        var removedTail = lastSurvivor.Next!;
        Assert.Equal(2, Variant9Algorithms.RemoveConsecutiveDuplicatesManual(list));
        Assert.Equal(new[] { 7, 8, 7 }, list.ToArray());
        Assert.Same(first, list.Head);
        Assert.Same(lastSurvivor, list.Tail);
        Assert.Null(removed.Next);
        Assert.Null(removed.Previous);
        Assert.Null(removedTail.Next);
        Assert.Null(removedTail.Previous);
        Assert.Equal(0, Variant9Algorithms.RemoveConsecutiveDuplicatesManual(list));
        list.AssertInvariants();
    }

    [Fact]
    public void SplitByOneBasedPositions_ReusesNodesAndEmptiesSource()
    {
        var source = new DoublyLinkedList<string>(new[] { "a", "b", "c", "d", "e" });
        var first = source.Head!;
        var second = first.Next!;
        Variant9Algorithms.SplitByPositionManual(source, out var odds, out var evens);
        Assert.Empty(source);
        Assert.Equal(new[] { "a", "c", "e" }, odds.ToArray());
        Assert.Equal(new[] { "b", "d" }, evens.ToArray());
        Assert.Same(first, odds.Head);
        Assert.Same(second, evens.Head);
        source.AssertInvariants();
        odds.AssertInvariants();
        evens.AssertInvariants();
    }

    [Fact]
    public void ExhaustiveSmallLists_ValidateThreeVariantTasks()
    {
        // 1 + 3 + 9 + ... + 729 = 1093 distinct patterns.
        var cases = 0;
        var combinations = 1;
        for (var length = 0; length <= 6; length++, combinations *= 3)
        {
            for (var pattern = 0; pattern < combinations; pattern++)
            {
                var values = new int[length];
                var digits = pattern;
                for (var index = 0; index < length; index++)
                {
                    values[index] = digits % 3 - 1;
                    digits /= 3;
                }
                var source = new DoublyLinkedList<int>(values);
                source.AssertInvariants();
                Assert.Equal(length != 0,
                    Variant9Algorithms.TryCalculateAverageManual(source, out var sum, out var average));
                Assert.Equal(values.Sum(x => (long)x), sum);
                if (length != 0) Assert.Equal((double)sum / length, average);

                var expectedRuns = values.Where((x, index) => index == 0 || x != values[index - 1]).ToArray();
                var cloned = source.CloneManual();
                Assert.Equal(length - expectedRuns.Length,
                    Variant9Algorithms.RemoveConsecutiveDuplicatesManual(cloned));
                Assert.Equal(expectedRuns, cloned.ToArray());
                cloned.AssertInvariants();

                Variant9Algorithms.SplitByPositionManual(source, out var odd, out var even);
                Assert.Equal(values.Where((_, i) => i % 2 == 0), odd.ToArray());
                Assert.Equal(values.Where((_, i) => i % 2 != 0), even.ToArray());
                Assert.Empty(source);
                source.AssertInvariants();
                odd.AssertInvariants();
                even.AssertInvariants();
                cases++;
            }
        }
        Assert.Equal(1093, cases);
    }

    [Fact]
    public void RandomMutationSequence_MatchesFrameworkReferenceList()
    {
        // Only tests use the framework collection as a control oracle.
        var random = new Random(907);
        var actual = new DoublyLinkedList<int>();
        var expected = new LinkedList<int>();
        for (var step = 0; step < 1500; step++)
        {
            var action = random.Next(5);
            var value = random.Next(-5, 6);
            switch (action)
            {
                case 0:
                    actual.AddFirst(value);
                    expected.AddFirst(value);
                    break;
                case 1:
                    actual.AddLast(value);
                    expected.AddLast(value);
                    break;
                case 2 when expected.Count > 0:
                    Assert.Equal(expected.First!.Value, actual.RemoveFirst());
                    expected.RemoveFirst();
                    break;
                case 3 when expected.Count > 0:
                    Assert.Equal(expected.Last!.Value, actual.RemoveLast());
                    expected.RemoveLast();
                    break;
                case 4:
                    Assert.Equal(expected.Remove(value), actual.Remove(value));
                    break;
            }
            actual.AssertInvariants();
            Assert.Equal(expected.ToArray(), actual.ToArray());
            Assert.Equal(expected.Count, actual.Count);
        }
    }

}
