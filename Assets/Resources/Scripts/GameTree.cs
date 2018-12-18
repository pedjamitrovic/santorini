using System.Collections.Generic;

delegate T TreeVisitor<T>(T nodeData);

class GameTree<T>
{
    public T data;
    public List<GameTree<T>> children;

    public GameTree(T data)
    {
        this.data = data;
        children = new List<GameTree<T>>();
    }
    public void AddChild(T data)
    {
        children.Add(new GameTree<T>(data));
    }
    public GameTree<T> GetChild(int i)
    {
        return children[i];
    }
    public T Traverse(TreeVisitor<T> visitor)
    {
        foreach (GameTree<T> kid in children)
        {
            kid.Traverse(visitor);
        }
        return visitor(data);
    }
}