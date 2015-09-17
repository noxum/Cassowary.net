using System;
using Cassowary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CassowaryTests
{
    [TestClass]
    public class BugReproduction
    {
        /// <summary>
        /// https://github.com/martindevans/Cassowary.net/issues/1
        /// </summary>
        [TestMethod]
        public void EndEdit_NonRequiredVariable_SolvesCorrectly()
        {
            //Create a solver
            var solver = new ClSimplexSolver();
            var variable = new ClVariable(0f);

            //Add a stay, indicating this var should stay at it's current value if possible
            solver.AddStay(variable, ClStrength.Strong, 1);

            const double EXPECTED_VALUE = 10.0;

            //Suggest a value for the variable in an edit context
            var editContext = solver.BeginEdit(variable);
            editContext.SuggestValue(variable, EXPECTED_VALUE);
            editContext.EndEdit();

            //Assert that the value has changed to the suggested value
            Assert.AreEqual(EXPECTED_VALUE, variable.Value);
        }
    }
}
