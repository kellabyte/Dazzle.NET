using System.Collections.Generic;
using Dazzle.Query;
using Irony.Parsing;

namespace Dazzle.Operations
{
    /// <summary>
    /// Builds operations into the <see cref="QueryExecutionPlan"/> by reading the <see cref="ParseTreeNode"/>.
    /// </summary>
    public interface IOperationBuilder
    {
        string OperationName { get; }

        /// <summary>
        /// Build the current operation in the <see cref="QueryExecutionPlan"/> by reading the abstract syntax tree.
        /// </summary>
        /// <param name="scope">Scope of the current <see cref="ParseTreeNode"/>.</param>
        /// <param name="node">Current <see cref="ParseTreeNode"/> in the abstract syntax tree.</param>
        /// <param name="plan">The current <see cref="QueryExecutionPlan"/> being built.</param>
        void BuildOperation(Stack<string> scope, ParseTreeNode node, QueryExecutionPlan plan);
    }
}
