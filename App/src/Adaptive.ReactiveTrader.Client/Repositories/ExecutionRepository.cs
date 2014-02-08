﻿using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Client.ServiceClients.Execution;
using Adaptive.ReactiveTrader.Shared.Execution;
using Adaptive.ReactiveTrader.Shared.Extensions;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    class ExecutionRepository : IExecutionRepository
    {
        private readonly IExecutionServiceClient _executionServiceClient;
        private readonly ITradeFactory _tradeFactory;

        public ExecutionRepository(IExecutionServiceClient executionServiceClient, ITradeFactory tradeFactory)
        {
            _executionServiceClient = executionServiceClient;
            _tradeFactory = tradeFactory;
        }

        public IObservable<ITrade> Execute(IExecutablePrice executablePrice, long notional)
        {
            var price = executablePrice.Parent;

            var request = new TradeRequestDto
            {
                Direction = executablePrice.Direction == Direction.Buy ? DirectionDto.Buy : DirectionDto.Sell,
                Notional = notional,
                QuoteId = price.QuoteId,
                SpotRate = executablePrice.Rate,
                Symbol = price.CurrencyPair.Symbol,
                ValueDate = price.ValueDate
            };

            return _executionServiceClient.Execute(request)
                .Select(_tradeFactory.Create)
                .CacheFirstResult();
        }
    }
}