using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using InspiredCalculator.Shared;
using Qkmaxware.Cas;
using Qkmaxware.Cas.Calculus;

namespace InspiredCalculator;

public class SideEffects {
    public ReadOnlyDictionary<string, IExpression> Variables {get; private set;}
    public Dictionary<string, IExpression> Assignments {get; private set;} = new Dictionary<string, IExpression>();
    public List<IGraphics> Graphics {get; private set;} = new List<IGraphics>();

    public SideEffects(ReadOnlyDictionary<string, IExpression> vars) {
        this.Variables = vars;
    }
}

public class TreeBuilderNode {
    public InputMode? Mode {get; private set;}
    private TreeBuilderNode? parent;
    private TreeBuilderNode[] children ;

    public TreeBuilderNode() {
        this.parent = null;
        this.Mode = null;
        this.children = new TreeBuilderNode[0];
    }

    public TreeBuilderNode? GetParent() => this.parent;
    public TreeBuilderNode? GetRoot() {
        var root = this;
        while (root.parent is not null)
            root = root.parent;
        return root;
    }
    public IEnumerable<TreeBuilderNode> GetChildren() => this.children;
    public void SetChild(int index, TreeBuilderNode node) {
        if (index >= 0 && index < children.Length) {
            children[index] = node;
            node.parent = this;
        }
    }
    public TreeBuilderNode GetChild(int index) {
        return this.children[index];
    }
    public int ChildrenCount => this.children.Length;
    public TreeBuilderNode? TryGetChild(int index) {
        if (index >= 0 && index < children.Length) {
            return GetChild(index);
        } else {
            return null;
        }
    }
    public bool ReplaceChild(TreeBuilderNode from, TreeBuilderNode to) {
        for (var i = 0; i < children.Length; i++) {
            TreeBuilderNode child = children[i];
            if (ReferenceEquals(child, from)) {
                child.parent = null;
                children[i] = to;
                to.parent = this;
                return true;
            }
        }
        return false;
    }

    public TreeBuilderNode(TreeBuilderNode? parent) : this() {
        this.parent = parent;
    }

    public TreeBuilderNode GetRightmostChild() {
        if (this.ChildrenCount < 1) {
            return this;
        }

        return this.children[this.children.Length - 1].GetRightmostChild();
    }

    public TreeBuilderNode GetLeftmostChild() {
        if (this.ChildrenCount < 1) {
            return this;
        }

        return this.children[0].GetLeftmostChild();
    }

    public TreeBuilderNode GetAboveNode() {
        if (Mode is DivOperatorInputMode) {
            return this.GetChild(0);
        } 
        if (parent is not null && (parent?.Mode is MatrixInputMode mtx)) {
            var d1 = Array.IndexOf(parent.children, this);
            if (d1 == -1) {
                return parent.GetAboveNode();
            }

            var (row, col) = mtx.Get2dIndex(d1);
            if (row == 0) {
                return parent;
            } else {
                return parent.children[mtx.Get1dIndex(row - 1, col)];
            }
        }
        else {
            var parent = this.GetParent();
            if (parent is not null) {
                return parent.GetAboveNode();
            } else {
                return this;
            }
        }
    }
    public TreeBuilderNode? GetBelowNode() {
        if (Mode is DivOperatorInputMode) {
            return this.GetChild(1);
        }
        if (parent is not null && (parent?.Mode is MatrixInputMode mtx)) {
            var d1 = Array.IndexOf(parent.children, this);
            if (d1 == -1) {
                return parent.GetBelowNode();
            }

            var (row, col) = mtx.Get2dIndex(d1);
            if (row == (mtx.Rows - 1)) {
                return parent;
            } else {
                return parent.children[mtx.Get1dIndex(row + 1, col)];
            }
        }
        else {
            var parent = this.GetParent();
            if (parent is not null) {
                return parent.GetBelowNode();
            } else {
                return this;
            }
        }
    }
    public TreeBuilderNode GetLeftNode() {
        var parent = this.GetParent();
        if (parent is null)
            return this;
        var index = Array.IndexOf(parent.children, this);
        if (index == -1 || index < 1) {
            // Exit the node
            return parent;
        }
        else {
            // Go to the previous node's furthest right child
            var left_sibling = parent.children[index - 1];
            return left_sibling.GetRightmostChild();
        }
    }
    public TreeBuilderNode GetRightNode() {
        var parent = this.GetParent();
        if (parent is null)
            return this;
        var index = Array.IndexOf(parent.children, this);
        if (index >= parent.children.Length - 1) {
            // Exit the node
            return parent;
        }
        else {
            // Go to the previous node's furthest right child
            var right_sibling = parent.children[index + 1];
            return right_sibling.GetLeftmostChild();
        }
    }

    public void ReconfigureMode(InputMode? mode) {
        if (mode is null) {
            this.Mode = null;
            this.children = new TreeBuilderNode[0];
            return;
        }

        this.Mode = mode;
        this.children = new TreeBuilderNode[mode.SubExpressionCount()];
        for (var i = 0; i < this.children.Length; i++) {
            this.children[i] = new TreeBuilderNode(this);
        }
    }

    public void Clear() {
        this.ReconfigureMode(null);
    }

    public IExpression? MakeExpression(SideEffects sideEffects) => Mode?.MakeExpression(sideEffects, children.Select(x => x.MakeExpression(sideEffects)).ToArray());
}