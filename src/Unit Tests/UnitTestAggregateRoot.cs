using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEventLite.Domain;
using NEventLite.Snapshot;
using NEventLite_Example.Domain;
using NEventLite_Example.Util;

namespace NEventLite.Test
{
    [TestClass]
    public class UnitTestAggregateRoot
    {
        private MyLifeTimeScope _scope;
        private DependencyResolver _container;

        [TestInitialize]
        public void Initialize()
        {
            _scope = new MyLifeTimeScope();
            _container = new DependencyResolver();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _container.Dispose();
            _scope.Dispose();
        }

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
