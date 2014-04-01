using System;
using System.Diagnostics;
using Cassowary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CassowaryTests
{
    [TestClass]
    public class ClTests
    {
        private readonly Random _rnd;

        public ClTests()
        {
            _rnd = new Random(123456789);
        }

        [TestMethod]
        public void Simple1()
        {
            ClVariable x = new ClVariable("x", 167);
            ClVariable y = new ClVariable("y", 2);
            ClSimplexSolver solver = new ClSimplexSolver();

            ClLinearEquation eq = new ClLinearEquation(x, new ClLinearExpression(y));
            solver.AddConstraint(eq);

            Assert.AreEqual(x.Value, y.Value);
        }

        [TestMethod]
        public void JustStay1()
        {
            ClVariable x = new ClVariable("x", 5);
            ClVariable y = new ClVariable("y", 10);
            ClSimplexSolver solver = new ClSimplexSolver();

            solver.AddStay(x);
            solver.AddStay(y);

            Assert.IsTrue(Cl.Approx(x, 5));
            Assert.IsTrue(Cl.Approx(y, 10));
        }

        [TestMethod]
        public void AddDelete1()
        {
            ClVariable x = new ClVariable("x");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver.AddConstraint(new ClLinearEquation(x, 100, ClStrength.Weak));

            ClLinearInequality c10 = new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, 10.0);
            ClLinearInequality c20 = new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, 20.0);

            solver.AddConstraint(c10).AddConstraint(c20);
            Assert.IsTrue(Cl.Approx(x, 10.0));

            solver.RemoveConstraint(c10);
            Assert.IsTrue(Cl.Approx(x, 20.0));

            solver.RemoveConstraint(c20);
            Assert.IsTrue(Cl.Approx(x, 100.0));

            ClLinearInequality c10Again = new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, 10.0);

            solver.AddConstraint(c10).AddConstraint(c10Again);
            Assert.IsTrue(Cl.Approx(x, 10.0));

            solver.RemoveConstraint(c10);
            Assert.IsTrue(Cl.Approx(x, 10.0));

            solver.RemoveConstraint(c10Again);
            Assert.IsTrue(Cl.Approx(x, 100.0));
        }

        [TestMethod]
        public void AddDelete2()
        {
            ClVariable x = new ClVariable("x");
            ClVariable y = new ClVariable("y");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver
              .AddConstraint(new ClLinearEquation(x, 100.0, ClStrength.Weak))
              .AddConstraint(new ClLinearEquation(y, 120.0, ClStrength.Strong));

            ClLinearInequality c10 = new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, 10.0);
            ClLinearInequality c20 = new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, 20.0);

            solver
              .AddConstraint(c10)
              .AddConstraint(c20);
            Assert.IsTrue(Cl.Approx(x, 10.0));
            Assert.IsTrue(Cl.Approx(y, 120.0));

            solver.RemoveConstraint(c10);
            Assert.IsTrue(Cl.Approx(x, 20.0));
            Assert.IsTrue(Cl.Approx(y, 120.0));
            
            ClLinearEquation cxy = new ClLinearEquation(Cl.Times(2.0, x), y);
            solver.AddConstraint(cxy);
            Assert.IsTrue(Cl.Approx(x, 20.0));
            Assert.IsTrue(Cl.Approx(y, 40.0));
            
            solver.RemoveConstraint(c20);
            Assert.IsTrue(Cl.Approx(x, 60.0));
            Assert.IsTrue(Cl.Approx(y, 120.0));
            
            solver.RemoveConstraint(cxy);
            Assert.IsTrue(Cl.Approx(x, 100.0));
            Assert.IsTrue(Cl.Approx(y, 120.0));
        }

        [TestMethod]
        public void Casso1()
        {
            ClVariable x = new ClVariable("x");
            ClVariable y = new ClVariable("y");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver
              .AddConstraint(new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, y))
              .AddConstraint(new ClLinearEquation(y, Cl.Plus(x, 3.0)))
              .AddConstraint(new ClLinearEquation(x, 10.0, ClStrength.Weak))
              .AddConstraint(new ClLinearEquation(y, 10.0, ClStrength.Weak));

            Assert.IsTrue(
                (Cl.Approx(x, 10.0) && Cl.Approx(y, 13.0)) ||
                (Cl.Approx(x, 7.0) && Cl.Approx(y, 10.0))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(CassowaryRequiredFailureException))]
        public void Inconsistent1()
        {
            ClVariable x = new ClVariable("x");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver
                .AddConstraint(new ClLinearEquation(x, 10.0))
                .AddConstraint(new ClLinearEquation(x, 5.0));
        }

        [TestMethod]
        [ExpectedException(typeof(CassowaryRequiredFailureException))]
        public void Inconsistent2()
        {
            ClVariable x = new ClVariable("x");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver
                .AddConstraint(new ClLinearInequality(x, Cl.Operator.GreaterThanOrEqualTo, 10.0))
                .AddConstraint(new ClLinearInequality(x, Cl.Operator.LessThanOrEqualTo, 5.0));
        }

        [TestMethod]
        public void Multiedit()
        {
            ClVariable x = new ClVariable("x");
            ClVariable y = new ClVariable("y");
            ClVariable w = new ClVariable("w");
            ClVariable h = new ClVariable("h");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver
                .AddStay(x)
                .AddStay(y)
                .AddStay(w)
                .AddStay(h)

                .AddEditVar(x)
                .AddEditVar(y)
                .BeginEdit()

                .SuggestValue(x, 10)
                .SuggestValue(y, 20)
                .Resolve();

            Assert.IsTrue(Cl.Approx(x, 10));
            Assert.IsTrue(Cl.Approx(y, 20));
            Assert.IsTrue(Cl.Approx(w, 0));
            Assert.IsTrue(Cl.Approx(h, 0));

            solver
                .AddEditVar(w)
                .AddEditVar(h)
                .BeginEdit();

            solver
                .SuggestValue(w, 30)
                .SuggestValue(h, 40)
                .EndEdit();

            Assert.IsTrue(Cl.Approx(x, 10));
            Assert.IsTrue(Cl.Approx(y, 20));
            Assert.IsTrue(Cl.Approx(w, 30));
            Assert.IsTrue(Cl.Approx(h, 40));

            solver
                .SuggestValue(x, 50)
                .SuggestValue(y, 60)
                .EndEdit();

            Assert.IsTrue(Cl.Approx(x, 50));
            Assert.IsTrue(Cl.Approx(y, 60));
            Assert.IsTrue(Cl.Approx(w, 30));
            Assert.IsTrue(Cl.Approx(h, 40));
        }

        [TestMethod]
        [ExpectedException(typeof(CassowaryRequiredFailureException))]
        public void Inconsistent3()
        {
            ClVariable w = new ClVariable("w");
            ClVariable x = new ClVariable("x");
            ClVariable y = new ClVariable("y");
            ClVariable z = new ClVariable("z");
            ClSimplexSolver solver = new ClSimplexSolver();

            solver
                .AddConstraint(new ClLinearInequality(w, Cl.Operator.GreaterThanOrEqualTo, 10.0))
                .AddConstraint(new ClLinearInequality(x, Cl.Operator.GreaterThanOrEqualTo, w))
                .AddConstraint(new ClLinearInequality(y, Cl.Operator.GreaterThanOrEqualTo, x))
                .AddConstraint(new ClLinearInequality(z, Cl.Operator.GreaterThanOrEqualTo, y))
                .AddConstraint(new ClLinearInequality(z, Cl.Operator.GreaterThanOrEqualTo, 8.0))
                .AddConstraint(new ClLinearInequality(z, Cl.Operator.LessThanOrEqualTo, 4.0));
        }

        [TestMethod]
        public void AddDel()
        {
            const int nCns = 450;
            const int nVars = 450;
            const int nResolves = 5000;

            Stopwatch timer = new Stopwatch();
            const double ineqProb = 0.12;
            const int maxVars = 3;

            Console.WriteLine("starting timing test. nCns = " + nCns +
                ", nVars = " + nVars + ", nResolves = " + nResolves);

            timer.Start();
            ClSimplexSolver solver = new ClSimplexSolver();

            ClVariable[] rgpclv = new ClVariable[nVars];
            for (int i = 0; i < nVars; i++)
            {
                rgpclv[i] = new ClVariable(i, "x");
                solver.AddStay(rgpclv[i]);
            }

            ClConstraint[] rgpcns = new ClConstraint[nCns];
            int j;
            for (j = 0; j < nCns; j++)
            {
                // number of variables in this constraint
                int nvs = RandomInRange(1, maxVars);
                ClLinearExpression expr = new ClLinearExpression(UniformRandomDiscretized() * 20.0 - 10.0);
                int k;
                for (k = 0; k < nvs; k++)
                {
                    double coeff = UniformRandomDiscretized() * 10 - 5;
                    int iclv = (int)(UniformRandomDiscretized() * nVars);
                    expr.AddExpression(Cl.Times(rgpclv[iclv], coeff));
                }
                if (UniformRandomDiscretized() < ineqProb)
                {
                    rgpcns[j] = new ClLinearInequality(expr);
                }
                else
                {
                    rgpcns[j] = new ClLinearEquation(expr);
                }
            }

            Console.WriteLine("done building data structures");
            Console.WriteLine("time = " + timer.Elapsed);
            timer.Start();
            int cExceptions = 0;
            for (j = 0; j < nCns; j++)
            {
                // add the constraint -- if it's incompatible, just ignore it
                try
                {
                    solver.AddConstraint(rgpcns[j]);
                }
                catch (CassowaryRequiredFailureException)
                {
                    cExceptions++;
                    rgpcns[j] = null;
                }
            }
            Console.WriteLine("done adding constraints [" + cExceptions + " exceptions]");
            Console.WriteLine("time = " + timer.Elapsed + "\n");
            timer.Start();

            int e1Index = (int)(UniformRandomDiscretized() * nVars);
            int e2Index = (int)(UniformRandomDiscretized() * nVars);

            Console.WriteLine("indices " + e1Index + ", " + e2Index);

            ClEditConstraint edit1 = new ClEditConstraint(rgpclv[e1Index], ClStrength.Strong);
            ClEditConstraint edit2 = new ClEditConstraint(rgpclv[e2Index], ClStrength.Strong);

            solver
              .AddConstraint(edit1)
              .AddConstraint(edit2);

            Console.WriteLine("done creating edit constraints -- about to start resolves");
            Console.WriteLine("time = " + timer.Elapsed + "\n");
            timer.Start();

            for (int m = 0; m < nResolves; m++)
            {
                solver.Resolve(rgpclv[e1Index].Value * 1.001,
                               rgpclv[e2Index].Value * 1.001);
            }

            Console.WriteLine("done resolves -- now removing constraints");
            Console.WriteLine("time = " + timer.Elapsed + "\n");

            solver.RemoveConstraint(edit1);
            solver.RemoveConstraint(edit2);

            timer.Start();

            for (j = 0; j < nCns; j++)
            {
                if (rgpcns[j] != null)
                {
                    solver.RemoveConstraint(rgpcns[j]);
                }
            }

            Console.WriteLine("done removing constraints and AddDel timing test");
            Console.WriteLine("time = " + timer.Elapsed + "\n");

            timer.Start();
        }

        private double UniformRandomDiscretized()
        {
            double n = Math.Abs(_rnd.Next());
            return n / int.MaxValue;
        }

        private int RandomInRange(int low, int high)
        {
            return (int)UniformRandomDiscretized() * (high - low) + low;
        }
    }
}
