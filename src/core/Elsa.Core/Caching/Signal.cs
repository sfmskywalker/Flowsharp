﻿using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Elsa.Caching
{
    public class Signal : ISignal
    {
        private readonly ConcurrentDictionary<string, ChangeTokenInfo> _changeTokens;

        public Signal()
        {
            _changeTokens = new ConcurrentDictionary<string, ChangeTokenInfo>();
        }

        public IChangeToken GetToken(string key)
        {
            return _changeTokens.GetOrAdd(
                key,
                _ =>
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    var changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
                    return new ChangeTokenInfo(changeToken, cancellationTokenSource);
                }).ChangeToken;
        }

        public void Trigger(string key)
        {
            if (_changeTokens.TryRemove(key, out var changeTokenInfo)) changeTokenInfo.TokenSource.Cancel();
        }

        private struct ChangeTokenInfo
        {
            public ChangeTokenInfo(IChangeToken changeToken, CancellationTokenSource tokenSource)
            {
                ChangeToken = changeToken;
                TokenSource = tokenSource;
            }

            public IChangeToken ChangeToken { get; }
            public CancellationTokenSource TokenSource { get; }
        }
    }
}