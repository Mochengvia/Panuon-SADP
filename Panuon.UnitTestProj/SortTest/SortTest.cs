using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panuon_SADP_net40.Logic;
namespace Panuon.UnitTestProj.SortTest
{
    [TestClass]
    public class SortTest
    {
        private int[] array = new int[10] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
        private int[] sortedarray = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        [TestMethod]
        public void SelectSortTest()
        {
            var newarray = SortStepGenerator.InsertSort(array);
            Assert.AreEqual(newarray[0], sortedarray[0]);
        }
    }
}
