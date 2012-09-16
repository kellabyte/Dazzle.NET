using System;
using System.Linq;
using System.Text;
using Irony;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Dazzle.Query
{
    /// <summary>
    /// Dql syntax reader.
    /// </summary>
    public class DqlQueryReader
    {
        private LanguageData language;

        public DqlQueryReader(LanguageData languageData)
        {
            if (languageData == null)
            {
                throw new ArgumentNullException("languageData");
            }
            this.language = languageData;
        }

        /// <summary>
        /// Reads a Dql query into an abstract syntax tree.
        /// </summary>
        /// <param name="query">Dql query.</param>
        /// <param name="node">The current abstract syntax tree to construct.</param>
        /// <returns>Time taken to build the abstract syntax tree in milliseconds.</returns>
        public long Read(string query, out ParseTree node)
        {
            var parser = new Parser(language);
            var tree = parser.Parse(query);

            if (tree.HasErrors())
            {
                // TODO: Throw LQL errors.
                throw new InvalidOperationException();
            }

#if DEBUG
            this.DisplayTree(tree.Root, 0);
#endif
            node = tree;
            return tree.ParseTimeMilliseconds;
        }

#if DEBUG
        /// <summary>
        /// Outputs the abstract syntax tree for debugging.
        /// </summary>
        /// <param name="node">The current node in the abstract syntax tree.</param>
        /// <param name="level">The current child level in the abstract syntax tree.</param>
        public void DisplayTree(ParseTreeNode node, int level)
        {
            for (int i = 0; i < level; i++)
            {
                System.Diagnostics.Debug.Write("  ");
            }
            System.Diagnostics.Debug.WriteLine(node);

            foreach (ParseTreeNode child in node.ChildNodes)
            {
                DisplayTree(child, level + 1);
            }
        }
#endif
    }
}
