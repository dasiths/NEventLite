using System;

namespace NEventLite.Scope
{
    public abstract class ILifeTimeScope:IDisposable
    {
        public ILifeTimeScope()
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
