using Lab6;

namespace Tests;

[TestFixture]
public class Tests
{
    private RedBlackTree<int, int> _tree;

    private List<int> GetKeys() => _tree.RightLeftTraversal().Select(n => n.Key).ToList();
    
    [SetUp]
    public void Setup()
    {
        _tree = new RedBlackTree<int, int>();
    }
    
    [Test]
    public void Insert_OneElement()
    {
        _tree.Insert(1, 0);
        Assert.That(GetKeys(), Is.EquivalentTo([1]));
    }
    
    [Test]
    public void Insert_Case1_Recoloring_Left()
    {
        _tree.Insert(10, 0);
        _tree.Insert(5, 0);
        _tree.Insert(15, 0);
        _tree.Insert(2, 0);

        var keys = GetKeys();
        Assert.That(keys, Is.EquivalentTo([10, 5, 15, 2]));
    }
    
    [Test]
    public void Insert_Case2And3_Rotations_Left()
    {
        _tree.Insert(10, 0);
        _tree.Insert(5, 0);
        _tree.Insert(7, 0);

        var root = _tree.Root;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(root.Key, Is.EqualTo(7));
            Assert.That(root.Color, Is.EqualTo(Color.Black));
            Assert.That(root.Left.Key, Is.EqualTo(5));
            Assert.That(root.Left.Color, Is.EqualTo(Color.Red));
            Assert.That(root.Right.Key, Is.EqualTo(10));
            Assert.That(root.Right.Color, Is.EqualTo(Color.Red));
        }
    }
    
    [Test]
    public void Insert_Case1_Recoloring_Right()
    {
        _tree.Insert(10, 0);
        _tree.Insert(5, 0);
        _tree.Insert(15, 0);
        _tree.Insert(20, 0);

        var result = _tree.RightLeftTraversal();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.First(n => n.Key == 15).Color, Is.EqualTo(Color.Black));
            Assert.That(result.First(n => n.Key == 5).Color, Is.EqualTo(Color.Black));
        }
    }
    
    [Test]
    public void Insert_Case2And3_Rotations_Right()
    {
        _tree.Insert(10, 0);
        _tree.Insert(15, 0);
        _tree.Insert(12, 0);

        var result = _tree.RightLeftTraversal();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.First(r => r.Key == 12).Color, Is.EqualTo(Color.Black));
            Assert.That(result.First(r => r.Key == 15).Color, Is.EqualTo(Color.Red));
            Assert.That(result.First(r => r.Key == 10).Color, Is.EqualTo(Color.Red));
        }
    }
    
    [Test]
    public void Insert_Maintains_SearchProperty()
    {
        var values = new[] { 50, 25, 75, 10, 33, 5, 15 };
        foreach (var v in values) _tree.Insert(v, 0);

        var resultKeys = _tree.RightLeftTraversal().Select(n => n.Key).ToList();
        var expected = values.OrderByDescending(v => v).ToList();
        
        Assert.That(resultKeys, Is.EqualTo(expected));
    }
    
    [Test]
    public void Delete_RootOnly()
    {
        _tree.Insert(10, 0);
        _tree.Delete(10);
        
        Assert.That(_tree.RightLeftTraversal(), Is.Empty);
    }
    
    [Test]
    public void Delete_RedLeaf_NoFixupNeeded()
    {
        _tree.Insert(10, 0);
        _tree.Insert(5, 0);
        
        _tree.Delete(5);
        
        var nodes = _tree.RightLeftTraversal();
        Assert.That(nodes.Count, Is.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(nodes[0].Key, Is.EqualTo(10));
            Assert.That(nodes[0].Color, Is.EqualTo(Color.Black));
        }
    }
    
    [Test]
    public void Delete_BlackNode_Case1_SiblingRed()
    {
        foreach (var val in new[] { 10, 5, 15, 12, 17 }) _tree.Insert(val, 0);
        _tree.Delete(5);
        
        var keys = _tree.RightLeftTraversal().Select(n => n.Key);
        Assert.That(keys, Is.EquivalentTo([10, 15, 12, 17]));
    }
    
    [Test]
    public void Delete_NodeWithTwoChildren()
    {
        _tree.Insert(20, 0);
        _tree.Insert(10, 0);
        _tree.Insert(30, 0);
        _tree.Insert(5, 0);
        _tree.Insert(15, 0);

        _tree.Delete(10);

        var keys = _tree.RightLeftTraversal().Select(n => n.Key).ToList();
        Assert.That(keys, Is.EquivalentTo(new[] { 20, 30, 5, 15 }));
        Assert.That(keys, Does.Not.Contain(10));
    }
    
    [Test]
    public void Delete_Successive_Removals()
    {
        var values = new List<int> { 10, 20, 30, 40, 50, 60, 70, 80 };
        foreach (var v in values) _tree.Insert(v, 0);

        _tree.Delete(40);
        _tree.Delete(10);
        _tree.Delete(80);

        var keys = _tree.RightLeftTraversal().Select(n => n.Key).ToList();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(keys, Is.EquivalentTo([20, 30, 50, 60, 70]));
            Assert.That(IsTreeBalanced(), Is.True);
        }
    }
    
    private bool IsTreeBalanced()
    {
        return CheckBlackHeight(_tree.RightLeftTraversal().FirstOrDefault(n => n.Parent.Key == 0)) != -1;
    }

    private int CheckBlackHeight(RedBlackTree<int, int>.Node? node)
    {
        if (node == null || node.Key == 0) return 1;

        var leftHeight = CheckBlackHeight(node.Left);
        var rightHeight = CheckBlackHeight(node.Right);

        if (leftHeight == -1 || rightHeight == -1 || leftHeight != rightHeight)
            return -1;

        return leftHeight + (node.Color == Color.Black ? 1 : 0);
    }
}