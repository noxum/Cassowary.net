using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Cassowary
{
    public static class ClSimplexSolverExtensions
    {
        private static readonly ClStrength _defaultStrength = ClStrength.Required;

        #region add
        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, Expression<Func<double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, ClAbstractVariable a, Expression<Func<double, bool>> constraint, ClStrength strength = null)
        {
            Dictionary<string, ClAbstractVariable> variables = ConstructVariables(constraint.Parameters, a);
            return AddConstraint(solver, variables, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, Expression<Func<double, double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, ClAbstractVariable a, ClAbstractVariable b, Expression<Func<double, bool>> constraint, ClStrength strength = null)
        {
            Dictionary<string, ClAbstractVariable> variables = ConstructVariables(constraint.Parameters, a, b);
            return AddConstraint(solver, variables, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, Expression<Func<double, double, double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, ClAbstractVariable a, ClAbstractVariable b, ClAbstractVariable c, Expression<Func<double, bool>> constraint, ClStrength strength = null)
        {
            Dictionary<string, ClAbstractVariable> variables = ConstructVariables(constraint.Parameters, a, b, c);
            return AddConstraint(solver, variables, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, Expression<Func<double, double, double, double, bool>> constraint, ClStrength strength = null)
        {
            return AddConstraint(solver, constraint.Parameters, constraint.Body, strength);
        }

        public static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, ClAbstractVariable a, ClAbstractVariable b, ClAbstractVariable c, ClAbstractVariable d, Expression<Func<double, bool>> constraint, ClStrength strength = null)
        {
            Dictionary<string, ClAbstractVariable> variables = ConstructVariables(constraint.Parameters, a, b, c, d);
            return AddConstraint(solver, variables, constraint.Body, strength);
        }


        private static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, IEnumerable<ParameterExpression> parameters, Expression body, ClStrength strength)
        {
            Dictionary<string, ClAbstractVariable> variables = parameters.Select(a => solver.GetVariable(a.Name) ?? new ClVariable(a.Name)).ToDictionary(a => a.Name);

            return AddConstraint(solver, variables, body, strength);
        }

        private static IEnumerable<ClAbstractVariable> AddConstraint(this ClSimplexSolver solver, Dictionary<string, ClAbstractVariable> variables, Expression body, ClStrength strength)
        {
            var constraints = FromExpression(variables, body, strength ?? _defaultStrength);
            foreach (var c in constraints)
                solver.AddConstraint(c);

            return variables.Values;
        }

        private static Dictionary<string, ClAbstractVariable> ConstructVariables(ReadOnlyCollection<ParameterExpression> parameters, params ClAbstractVariable[] variables)
        {
            if (variables.Length != parameters.Count)
                throw new ArgumentException(string.Format("Expected {0} parameters, found {1}", parameters.Count, variables.Length));

            var names = Enumerable.Range('a', 26).Select(a => Convert.ToChar(a).ToString(CultureInfo.InvariantCulture)).ToArray();

            Dictionary<string, ClAbstractVariable> map = new Dictionary<string, ClAbstractVariable>();
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Name != names[i])
                    throw new ArgumentException(string.Format("Parameter at position {0} must be named {1}, instead of {2}", i, names[i], parameters[i].Name));

                map[names[i]] = variables[i];
            }

            return map;
        }
        #endregion

        #region expression conversion
        private static IEnumerable<ClConstraint> FromExpression(IDictionary<string, ClAbstractVariable> variables, Expression expression, ClStrength strength)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                {
                    foreach (var c in FromExpression(variables, ((BinaryExpression) expression).Left, strength))
                        yield return c;
                    foreach (var c in FromExpression(variables, ((BinaryExpression) expression).Right, strength))
                        yield return c;
                    break;
                }
                case ExpressionType.Equal:
                {
                    yield return CreateEquality(variables, (BinaryExpression) expression, strength);
                    break;
                }
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThanOrEqual:
                {
                    yield return CreateLinearInequality(variables, (BinaryExpression) expression, strength);
                    break;
                }
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

        private static ClLinearInequality CreateLinearInequality(IDictionary<string, ClAbstractVariable> variables, BinaryExpression expression, ClStrength strength)
        {
            var op = (expression.NodeType == ExpressionType.GreaterThanOrEqual) ? Cl.Operator.GreaterThanOrEqualTo : Cl.Operator.LessThanOrEqualTo;
            return new ClLinearInequality(CreateLinearExpression(variables, expression.Left), op, CreateLinearExpression(variables, expression.Right), strength);
        }
        #endregion
    }
}
