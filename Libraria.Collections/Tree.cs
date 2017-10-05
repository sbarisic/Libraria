using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraria.Collections {
	public class Tree<T> {
		public class TreeNode {
			public List<TreeNode> Children = new List<TreeNode>();
			public TreeNode Parent;

			public T Userdata;

			public bool HasChildren {
				get {
					return Children.Count > 0;
				}
			}

			public TreeNode(T Userdata = default(T)) {
				this.Userdata = Userdata;
			}

			public TreeNode(IEnumerable<TreeNode> Children) {
				Add(Children);
			}

			public void Add(TreeNode Node) {
				Children.Add(Node);
				Node.Parent = this;
			}

			public void Add(IEnumerable<TreeNode> Nodes) {
				foreach (var N in Nodes)
					Add(N);
			}

			public TreeNode[] PathToParent() {
				List<TreeNode> Nodes = new List<TreeNode>();
				Nodes.Add(this);

				TreeNode Last = null;
				while ((Last = Nodes.Last()).Parent != null)
					Nodes.Add(Last.Parent);

				return Nodes.ToArray();
			}

			public override string ToString() {
				if (Userdata == null)
					return "null";
				return Userdata.ToString();
			}
		}

		public TreeNode Root;

		public Tree(T RootUserdata, IEnumerable<TreeNode> Nodes) {
			Root = new TreeNode();
			Root.Userdata = RootUserdata;
			Root.Add(Nodes);
		}

		public TreeNode[] GetLeaves() {
			List<TreeNode> Leaves = new List<TreeNode>();
			Leaves.Add(Root);

		Repeat:
			for (int i = 0; i < Leaves.Count; i++) {
				if (Leaves[i].HasChildren) {
					Leaves.Replace(Leaves[i], Leaves[i].Children);
					goto Repeat;
				}
			}

			return Leaves.ToArray();
		}
	}
}
