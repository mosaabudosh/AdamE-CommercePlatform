using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Generic.Generic
{
    public class TreeNode
    {
        public object Data { get; set; }
        public List<TreeNode<object>> Children { get; set; }
    }

    public class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; }
    }
}
