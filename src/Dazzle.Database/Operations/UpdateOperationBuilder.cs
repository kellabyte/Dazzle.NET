using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dazzle.Query;
using Irony.Parsing;

namespace Dazzle.Operations
{
    /// <summary>
    /// Builds an Update operation into the <see cref="QueryExecutionPlan"/> by reading the <see cref="ParseTreeNode"/>.
    /// </summary>
    public class UpdateOperationBuilder : IOperationBuilder
    {
        /// <summary>
        /// The name of the operation in the abstract syntax tree.
        /// </summary>
        public string OperationName { get { return "updateStmt"; } }

        /// <summary>
        /// Build the current operation in the <see cref="QueryExecutionPlan"/> by reading the abstract syntax tree.
        /// </summary>
        /// <param name="scope">Scope of the current <see cref="ParseTreeNode"/>.</param>
        /// <param name="node">Current <see cref="ParseTreeNode"/> in the abstract syntax tree.</param>
        /// <param name="plan">The current <see cref="QueryExecutionPlan"/> being built.</param>
        public void BuildOperation(Stack<string> scope, ParseTreeNode node, QueryExecutionPlan plan)
        {
            UpdateOperation operation;

            if (node.Term.Name == this.OperationName)
            {
                // Update statement
                operation = new UpdateOperation();
                operation.TableName = node.ChildNodes[1].ChildNodes[0].Token.ValueString;
                plan.Operations.Add(operation);
                
            }

            operation = (UpdateOperation) plan.Current;

            if (node.Term.Name == "assignment")
            {
                operation.Assignments.Add(node.ChildNodes[0].ChildNodes[0].Token.ValueString, node.ChildNodes[2].Token.ValueString);
            }
            else if (scope.Contains("whereClauseOpt") && node.Term.Name == "binExpr")
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
