﻿#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api.data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using com.dxfeed.api.connection;
using com.dxfeed.api.events;

namespace com.dxfeed.api
{
    /// <summary>
    ///   Interface provides operations with event subscription
    /// </summary>
    public interface IDxConnection : IDisposable
    {
        /// <summary>
        ///   Disconnects from the server
        /// </summary>
        void Disconnect();

        /// <summary>
        ///   Creates event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(EventType type, IDxEventListener listener);

        /// <summary>
        ///   Creates candle event subscription
        /// </summary>
        /// <param name="time">date time in the past</param>
        /// <param name="listener">candle listener callback</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(DateTime? time, IDxCandleListener listener);

        /// <summary>
        ///   Creates time event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="time">Unix time stamp (the number of milliseconds from 1.1.1970)</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(EventType type, long time, IDxEventListener listener);

        /// <summary>
        /// Creates time event subscription.
        /// </summary>
        /// <param name="type">Event type.</param>
        /// <param name="time">Date time in the past.</param>
        /// <param name="listener">Event listener callback.</param>
        /// <returns>Subscription object.</returns>
        /// <exception cref="ArgumentNullException">Listener is null.</exception>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSubscription(EventType type, DateTime? time, IDxEventListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener"></param>
        /// <returns></returns>
        IDxSubscription CreateSnapshotSubscription(Int64 time, IDxSnapshotListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener"></param>
        /// <returns></returns>
        IDxSubscription CreateSnapshotSubscription(DateTime? time, IDxSnapshotListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Time in the past - number of milliseconds from 1.1.1970 (unix time)</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSnapshotSubscription(EventType eventType, Int64 time,
            IDxSnapshotListener listener);

        /// <summary>
        ///   Creates snapshot subscription
        /// </summary>
        /// <param name="eventType">Single event type.</param>
        /// <param name="time">Date time in the past</param>
        /// <param name="listener">snapshot listener callback</param>
        /// <returns>subscription object</returns>
        /// <exception cref="DxException"></exception>
        IDxSubscription CreateSnapshotSubscription(EventType eventType, DateTime? time,
            IDxSnapshotListener listener);

        /// <summary>
        ///   Creates Order View subscription
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        [Obsolete("CreateOrderViewSubscription method is deprecated and will removed in 9.0.0 version. Please use NativeConnection\\NativeSubscription")]
        IDxSubscription CreateOrderViewSubscription(IDxOrderViewListener listener);

        /// <summary>
        ///   Creates regional book
        /// </summary>
        /// <param name="symbol">Single symbol name</param>
        /// <param name="book_listener">Regional book changes listener. Null is allowed.</param>
        /// <param name="quote_listener">Quotes listener. Null is allowed.</param>
        /// <returns>regional book object</returns>
        IDxRegionalBook CreateRegionalBook(string symbol, IDxRegionalBookListener book_listener,
            IDxQuoteListener quote_listener);

        /// <summary>
        /// Asynchronously returns a "snapshot" of data for the specified period.
        /// <br/>
        /// By default, the algorithm is used on the completion of the timeout request. Default Timeout: 5000 ms.
        /// <br/>
        /// <remarks><!-- for doxygen --></remarks>
        /// <example>
        /// Example #1
        /// <code>
        /// using (var c = new NativeConnection(address, con => {})) {
        ///     var result = c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1))).Result;
        /// 
        ///     result.ForEach(Console.WriteLine);
        /// }
        /// </code>
        /// <br/>
        /// Example #2
        /// <code>
        /// using var c = new NativeConnection(address, con => {});
        /// var result = await c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        /// 
        /// result.ForEach(Console.WriteLine);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="eventType">The event type. Now supported all IDxIndexedEvent types except Order &amp; SpreadOrder. TheoPrice and Underlying are not supported yet.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime);

        /// <summary>
        /// Asynchronously returns a "snapshot" of data for the specified period
        /// <br/>
        /// <remarks><!-- for doxygen --></remarks>
        /// <example>
        /// Example #1
        /// <code>
        /// using (var c = new NativeConnection(address, con => {})) {
        ///     var result = c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1)),
        ///         TimeSpan.FromSeconds(10)).Result;
        /// 
        ///     result.ForEach(Console.WriteLine);
        /// }
        /// </code>
        /// <br/> 
        /// Example #2
        /// <code>
        /// using var c = new NativeConnection(address, con => {});
        /// var result = await c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1)),
        ///         TimeSpan.FromSeconds(10));
        /// 
        /// result.ForEach(Console.WriteLine);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="eventType">The event type. Now supported all IDxIndexedEvent types except Order &amp; SpreadOrder. TheoPrice and Underlying are not supported yet.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="timeout">The timespan to wait before the request times out.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime, TimeSpan timeout);

        /// <summary>
        /// Asynchronously returns a "snapshot" of data for the specified period.
        /// <br/>
        /// <remarks><!-- for doxygen --></remarks>
        /// <example>
        /// Example #1
        /// <code>
        /// using (var c = new NativeConnection(address, con => {})) {
        ///     var result = c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1)),
        ///         10000).Result;
        /// 
        ///     result.ForEach(Console.WriteLine);
        /// }
        /// </code>
        /// <br/> 
        /// Example #2
        /// <code>
        /// using var c = new NativeConnection(address, con => {});
        /// var result = await c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1)),
        ///         10000);
        /// 
        /// result.ForEach(Console.WriteLine);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="eventType">The event type. Now supported all IDxIndexedEvent types except Order &amp; SpreadOrder. TheoPrice and Underlying are not supported yet.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime, long timeout);
        
        /// <summary>
        /// Asynchronously returns a "snapshot" of data for the specified period.
        /// <br/>
        /// <remarks><!-- for doxygen --></remarks>
        /// <example>
        /// Example #1
        /// <code>
        /// var cancellationTokenSource = new CancellationTokenSource();
        /// var cancellationToken = cancellationTokenSource.Token;
        /// 
        /// using (var c = new NativeConnection(address, con => { })) {
        ///     var result = c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///         DateTime.Now.Subtract(TimeSpan.FromDays(1)),
        ///         cancellationToken);
        ///       
        ///     cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
        ///     result.Result.ForEach(Console.WriteLine);
        /// }
        /// </code>
        /// <br/>
        /// Example #2
        /// <code>
        /// var cancellationTokenSource = new CancellationTokenSource();
        /// var cancellationToken = cancellationTokenSource.Token;
        /// 
        /// cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
        /// 
        /// using var c = new NativeConnection(address, con => { });
        /// var result = await c.GetDataForPeriod(EventType.Candle, "AAPL{=1m}",
        ///     DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        ///     DateTime.Now.Subtract(TimeSpan.FromDays(1)),
        ///     cancellationToken);
        ///       
        /// result.ForEach(Console.WriteLine);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="eventType">The event type. Now supported all IDxIndexedEvent types except Order &amp; SpreadOrder. TheoPrice and Underlying are not supported yet.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetDataForPeriod(EventType eventType, string symbol, DateTime fromTime,
            DateTime toTime, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously returns a "snapshot" of data for the specified period
        ///
        /// By default, the algorithm is used on the completion of the timeout request. Default Timeout: 5000 ms.
        /// </summary>
        /// <param name="eventType">Order or SpreadOrder.</param>
        /// <param name="orderSource">The order source.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource, string symbol, DateTime fromTime,
            DateTime toTime);

        /// <summary>
        /// Asynchronously returns a "snapshot" of order data for the specified period.
        /// </summary>
        /// <param name="eventType">Order or SpreadOrder.</param>
        /// <param name="orderSource">The order source.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="timeout">The timespan to wait before the request times out.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource, string symbol, DateTime fromTime,
            DateTime toTime, TimeSpan timeout);

        /// <summary>
        /// Asynchronously returns a "snapshot" of order data for the specified period.
        /// </summary>
        /// <param name="eventType">Order or SpreadOrder.</param>
        /// <param name="orderSource">The order source.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource, string symbol, DateTime fromTime,
            DateTime toTime, long timeout);

        /// <summary>
        /// Asynchronously returns a "snapshot" of order data for the specified period.
        /// </summary>
        /// <param name="eventType">Order or SpreadOrder.</param>
        /// <param name="orderSource">The order source.</param>
        /// <param name="symbol">The event symbol. Single symbol name.</param>
        /// <param name="fromTime">The time, inclusive, to request events from.</param>
        /// <param name="toTime">The time, inclusive, to request events to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The task for the result of the request.</returns>
        /// <exception cref="DxException"></exception>
        Task<List<IDxIndexedEvent>> GetOrderDataForPeriod(EventType eventType, OrderSource orderSource, string symbol, DateTime fromTime,
            DateTime toTime, CancellationToken cancellationToken);

        /// <summary>
        ///   Add dumping raw data of incoming traffic of connection into specific file
        /// </summary>
        /// <param name="rawFileName">file name for raw data</param>
        /// <exception cref="ArgumentException">Invalid argument <c>rawFileName</c></exception>
        /// <exception cref="DxException"></exception>
        void WriteRawData(string rawFileName);

        /// <summary>
        ///   Retrieves properties for specified connection. Properties can be changed during
        ///   reconnection. So returned array is a snapshot of properties at the moment of the call.
        /// </summary>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        ///   Retrieves string with current connected address in format "&lt;host&gt;:&lt;port&gt;". If return value is NULL
        ///   then connection is not connected (reconnection, no valid addresses, closed connection and others).
        /// </summary>
        string ConnectedAddress { get; }

        /// <summary>
        ///   Retrieves the current connection status
        /// </summary>
        ConnectionStatus Status { get; }
    }
}