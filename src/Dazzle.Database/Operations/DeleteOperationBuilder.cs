using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Dazzle.Query;

namespace Dazzle.Operations
{
    /// <summary>
    /// Builds a Delete operation into the <see cref="QueryExecutionPlan"/> by reading the <see cref="ParseTreeNode"/>.
    /// </summary>
    public class DeleteOperationBuilder : IOperationBuilder
    {
        /// <summary>
        /// The name of the operation in the abstract syntax tree.
        /// </summary>
        public string OperationName { get { return "deleteStmt"; } }

        /// <summary>
        /// Build the current operation in the <see cref="QueryExecutionPlan"/> by reading the abstract syntax tree.
        /// </summary>
        /// <param name="scope">Scope of the current <see cref="ParseTreeNode"/>.</param>
        /// <param name="node">Current <see cref="ParseTreeNode"/> in the abstract syntax tree.</param>
        /// <param name="plan">The current <see cref="QueryExecutionPlan"/> being built.</param>
        public void BuildOperation(Stack<string> scope, ParseTreeNode node, QueryExecutionPlan plan)
        {
            DeleteOperation operation;

            if (node.Term.Name == this.OperationName)
            {
                // Delete operation
                operation = new DeleteOperation();
                operation.TableName = node.ChildNodes[2].ChildNodes[0].Token.ValueString;
                plan.Operations.Add(operation);

            }

            operation = (DeleteOperation)plan.Current;

            if (scope.Contains("whereClauseOpt") && node.Term.Name == "binExpr")
            {
                // Where clause
                if (node.ChildNodes[0].Term.Name == "binExpr")
                {
                    // Handle multiple where clauses.
                    foreach (var child in node.ChildNodes)
                    {
                        if (child.Term.Name == "binExpr")
                        {
                            var column = child.ChildNodes[0].ChildNodes[0].Token.ValueString;
                            var val = child.ChildNodes[2].Token.ValueString;
                            operation.WhereClauses.Add(column, val);
                        }
                    }
                }
                else if (node.Term.Name == "binExpr" && operation.WhereClauses.Count == 0)
                {
                    // Handle a single where clause.
                    var column = node.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    var val = node.ChildNodes[2].Token.ValueString;
                    operation.WhereClauses.Add(column, val);
                }
            }
        }
    }
}
