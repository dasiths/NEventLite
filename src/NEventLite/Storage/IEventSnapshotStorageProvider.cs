﻿using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    // This class is for storing snapshots as a series of events

    public interface IEventSnapshotStorageProvider<in TSnapshotKey> : ISnapshotStorageProvider<TSnapshotKey>
    {
        Task<TSnapshot> GetSnapshotAsync<TSnapshot, TAggregateKey>(TAggregateKey aggregateId, long version) where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>;
    }
}