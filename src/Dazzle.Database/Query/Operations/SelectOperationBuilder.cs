using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Dazzle.Query.Operations
{
    /// <summary>
    /// Builds a Select operation into the <see cref="QueryExecutionPlan"/> by reading the <see cref="ParseTreeNode"/>.
    /// </summary>
    public class SelectOperationBuilder : IOperationBuilder
    {
        /// <summary>
        /// Build the current operation in the <see cref="QueryExecutionPlan"/> by reading the abstract syntax tree.
        /// </summary>
        /// <param name="scope">Scope of the current <see cref="ParseTreeNode"/>.</param>
        /// <param name="node">Current <see cref="ParseTreeNode"/> in the abstract syntax tree.</param>
        /// <param name="plan">The current <see cref="QueryExecutionPlan"/> being built.</param>
        public void BuildOperation(Stack<string> scope, ParseTreeNode node, QueryExecutionPlan plan)
        {
            if (node.Term.Name == "selectStmt")
            {
                // Select clause
                plan.Operations.Add(new LevelDbSelectOperation());
            }
            else if (scope.Contains("selList") && node.Term.Name == "id_simple")
            {
                // Select column list
                plan.Current.ColumnNames.Add(node.Token.ValueString);
            }
            else if (scope.Contains("selList") && node.Term.Name == "*")
            {
                // Select column list
                plan.Current.ColumnNames.Add(node.Token.ValueString);
            }
            else if (scope.Contains("fromClauseOpt") && node.Term.Name == "id_simple")
            {
                // From clause
                plan.Current.TableName = node.Token.ValueString;
            }
            else if (scope.Contains("whereClauseOpt") && node.Term.Name == "binExpr")
            {
                // Where clause
                var column = node.ChildNodes[0].ChildNodes[0].Token.ValueString;
                var val = node.ChildNodes[2].Token.ValueString;
                plan.Current.WhereClauses.Add(column, val);
            }
        }
    }
}
