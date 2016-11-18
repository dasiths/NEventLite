using System;

namespace NEventLite.Scope
{
    public abstract class LifeTimeScope:IDisposable
    {
        protected LifeTimeScope()
        {
            OnStart();
        }

        protected abstract void OnStart();

        protected abstract void OnEnd();

        public virtual void Dispose()
        {
            OnEnd();
        }
    }
}
