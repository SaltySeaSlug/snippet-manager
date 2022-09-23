using System.Collections;

namespace snippet_manager.Common
{
    public class NodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            TreeNode tx = (TreeNode)x;
            TreeNode ty = (TreeNode)y;

            if (tx.Level == 0)
            {
                if (tx.Text == "My Snippets")
                    return 0;
                else
                    return 1;
            }

            if (tx.Level == 1)
            {
                return tx.Text.CompareTo(ty.Text);
            }

            if (tx.Level == 2)
            {
                return tx.Text.CompareTo(ty.Text);
            }

            if (tx.Level == 3)
            {
                return tx.Text.CompareTo(ty.Text);
            }

            return 0;
        }
    }
}