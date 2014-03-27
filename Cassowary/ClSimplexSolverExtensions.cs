using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cassowary
{
    public static class ClSimplexSolverExtensions
    {
        private static readonly ClStrength _defaultStrength = ClStrength.Required;

        #region add
        public static ClSimplexSolver AddConstraint(this ClSimplexSolver solver, Expression<Func<double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static ClSimplexSolver AddConstraint(this ClSimplexSolver solver, Expression<Func<double, double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static ClSimplexSolver AddConstraint(this ClSimplexSolver solver, Expression<Func<double, double, double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static ClSimplexSolver AddConstraint(this ClSimplexSolver solver, Expression<Func<double, double, double, double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        private static ClSimplexSolver AddConstraint(this ClSimplexSolver solver, IEnumerable<ParameterExpression> parameters, Expression body, ClStrength strength)
        {
            Dictionary<string, ClAbstractVariable> variables = parameters.Select(a => solver.GetVariable(a.Name) ?? new ClVariable(a.Name)).ToDictionary(a => a.Name);

            var constraints = FromExpression(variables, body, strength ?? _defaultStrength);
            foreach (var c in constraints)
                solver.AddConstraint(c);

            return solver;
        }
        #endregion

        private static IEnumerable<ClConstraint> FromExpression(Dictionary<string, ClAbstractVariable> variables, Expression expression, ClStrength strength)
        {
            if (expression.NodeType == ExpressionType.And || expression.NodeType == ExpressionType.AndAlso)
            {
                foreach (var c in FromExpression(variables, ((BinaryExpression)expression).Left, strength))
                    yield return c;
                foreach (var c in FromExpression(variables, ((BinaryExpression)expression).Right, strength))
                    yield return c;
            }
            else if (expression.NodeType == ExpressionType.Equal)
            {
                yield return CreateEquality(variables, (BinaryExpression)expression, strength);
            }
            else if (expression.NodeType == ExpressionType.GreaterThanOrEqual || expression.NodeType == ExpressionType.LessThanOrEqual)
            {
                yield return CreateLinearInequality(variables, (BinaryExpression)expression, strength);
            }
        }

        private static ClLinearExpression CreateLinearExpression(IDictionary<string, ClAbstractVariable> variables, Expression a)
        {
            switch (a.NodeType)
            {
                case ExpressionType.Add:
                {
                    var b = (BinaryExpression)a;
                    return Cl.Plus(CreateLinearExpression(variables, b.Left), CreateLinearExpression(variables, b.Right));
                }
                case ExpressionType.Subtract:
                {
                    var b = (BinaryExpression)a;
                    return Cl.Minus(CreateLinearExpression(variables, b.Left), CreateLinearExpression(variables, b.Right));
                }
                case ExpressionType.Multiply:
                {
                    var b = (BinaryExpression)a;
                    return Cl.Times(CreateLinearExpression(variables, b.Left), CreateLinearExpression(variables, b.Right));
                }
                case ExpressionType.Divide:
                {
                    var b = (BinaryExpression)a;
                    return Cl.Divide(CreateLinearExpression(variables, b.Left), CreateLinearExpression(variables, b.Right));
                }
                case ExpressionType.Parameter:
                    return new ClLinearExpression(variables[((ParameterExpression)a).Name]);
                case ExpressionType.Constant:
                    return new ClLinearExpression((double)((ConstantExpression)a).Value);
                default:
                    throw new ArgumentException(string.Format("Invalid node type {0}", a.NodeType), "a");
            }
        }

        private static ClConstraint CreateEquality(IDictionary<string, ClAbstractVariable> variables, BinaryExpression expression, ClStrength strength)
        {
            return new ClLinearEquation(CreateLinearExpression(variables, expression.Left), CreateLinearExpression(variables, expression.Right), strength);
        }

        private static ClLinearInequality CreateLinearInequality(Dictionary<string, ClAbstractVariable> variables, BinaryExpression expression, ClStrength strength)
        {
            var op = (expression.NodeType == ExpressionType.GreaterThanOrEqual) ? Cl.Operator.GreaterThanOrEqualTo : Cl.Operator.LessThanOrEqualTo;
            return new ClLinearInequality(CreateLinearExpression(variables, expression.Left), op, CreateLinearExpression(variables, expression.Right), strength);
        }
    }
}
