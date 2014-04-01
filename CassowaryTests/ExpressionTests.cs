using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cassowary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CassowaryTests
{
    [TestClass]
    public class ExpressionTests
    {
        private readonly ClSimplexSolver _solver = new ClSimplexSolver();

        [TestMethod]
        public void SingleParameterGreaterThanOrEqualToExpression()
        {
            _solver.AddConstraint(a => a >= 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value >= 10);
        }

        [TestMethod]
        public void SingleParameterGreaterThanOrEqualToExpressionRuntimeVars()
        {
            ClVariable variable = new ClVariable(Guid.NewGuid().ToString());

            _solver.AddConstraint(variable, a => a >= 10);
            Assert.IsTrue(variable.Value >= 10);
        }

        [TestMethod]
        public void SingleParameterGreaterThanOrEqualToExpressionSwitched()
        {
            _solver.AddConstraint(a => 10 >= a);
            Assert.IsTrue(10 >= ((ClVariable)_solver.GetVariable("a")).Value);
        }

        [TestMethod]
        public void SingleParameterLessThanOrEqualToExpression()
        {
            _solver.AddConstraint(a => a <= 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value <= 10);
        }

        [TestMethod]
        public void SingleParameterLessThanOrEqualToExpressionSwitched()
        {
            _solver.AddConstraint(a => 10 <= a);
            Assert.IsTrue(10 <= ((ClVariable)_solver.GetVariable("a")).Value);
        }

        [TestMethod]
        public void SingleParameterEqualToExpression()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => a == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterEqualToExpressionSwitched()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => 10 == a);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearAdditionConstraint()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => a + 3 == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value + 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearAdditionConstraintSwitched()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => 10 == a + 3);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value + 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearSubtractionConstraint()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => a - 3 == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value - 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearSubtractionConstraintSwitched()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => 10 == a - 3);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value - 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearMultiplicationConstraint()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => a * 3 == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value * 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearMultiplicationConstraintSwitched()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => 10 == a * 3);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value * 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearDivisionConstraint()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => a / 3 == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value / 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearDivisionConstraintSwitched()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => 10 == a / 3);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value / 3 == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearConstraint()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => a / 3 * 2 + 1 - 3 == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value / 3 * 2 + 1 - 3 == 10);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void SingleParameterLinearDivisionSwitched()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint(a => 10 == a / 3 * 2 + 1 - 3);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value / 3 * 2 + 1 - 3 == 10);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void TwoParameterGreaterThanOrEqualToExpression()
        {
            _solver.AddConstraint((a, b) => a >= b);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value >= ((ClVariable)_solver.GetVariable("b")).Value);
        }

        [TestMethod]
        public void TwoParameterGreaterThanOrEqualToExpressionRuntimeVars()
        {
            ClVariable varA = new ClVariable("a");
            ClVariable varB = new ClVariable("b");

            _solver.AddConstraint(varA, varB, (a, b) => a >= b);
            Assert.IsTrue(varA.Value >= varB.Value);
        }

        [TestMethod]
        public void TwoParameterLessThanOrEqualToExpression()
        {
            _solver.AddConstraint((a, b) => a <= b);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value <= ((ClVariable)_solver.GetVariable("b")).Value);
        }

        [TestMethod]
        public void TwoParameterEqualityToExpression()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint((a, b) => a == b);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value == ((ClVariable)_solver.GetVariable("b")).Value);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void TwoParameterLinearAdditionConstraint()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint((a, b) => a + b == 10);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value + ((ClVariable)_solver.GetVariable("b")).Value == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void TwoParameterLinearAdditionConstraint2()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint((a, b) => a + 10 == b);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value + ((ClVariable)_solver.GetVariable("b")).Value == 10);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void TwoParameterLinearConstraint()
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint((a, b) => a * 10 + b >= 15);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value * 10 + ((ClVariable)_solver.GetVariable("b")).Value >= 15);
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void TwoParameterLinearConstraint2()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            _solver.AddConstraint((a, b) => a * 10 + 15 >= b);
            Assert.IsTrue(((ClVariable)_solver.GetVariable("a")).Value * 10 + 15 >= ((ClVariable)_solver.GetVariable("b")).Value);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [TestMethod]
        public void MultiParameterAndConstraints()
        {
            _solver.AddConstraint((a, b, c, d) => a >= b && b >= c && c >= d && d * 2 + 3 - a <= 20);

            var aa = ((ClVariable)_solver.GetVariable("a")).Value;
            var bb = ((ClVariable)_solver.GetVariable("b")).Value;
            var cc = ((ClVariable)_solver.GetVariable("c")).Value;
            var dd = ((ClVariable)_solver.GetVariable("d")).Value;

            Assert.IsTrue(aa >= bb);
            Assert.IsTrue(bb >= cc);
            Assert.IsTrue(cc >= dd);
            Assert.IsTrue(dd * 2 + 3 - aa <= 20);
        }

        //[TestMethod]
        //public void Playground()
        //{
        //    _solver.AddConstraint(a => a > 10 & a < 20);
        //}
    }
}
