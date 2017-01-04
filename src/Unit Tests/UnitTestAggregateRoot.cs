using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEventLite.Domain;
using NEventLite.Snapshot;
using NEventLite_Example.Domain;

namespace NEventLite.Test
{
    [TestClass]
    public class UnitTestAggregateRoot
    {
        [TestMethod]
        public void IsNoteAnAggregateRoot()
        {
            Assert.IsInstanceOfType(new Note(), typeof(AggregateRoot));
        }

        [TestMethod]
        public void IsNoteSnapshottable()
        {
            Assert.IsInstanceOfType(new Note(), typeof(ISnapshottable));
        }
    }
}
