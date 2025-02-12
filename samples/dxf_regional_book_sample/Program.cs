﻿#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.events;
using com.dxfeed.native;

namespace dxf_regional_book_sample
{
    internal class RegionalBookListener : IDxRegionalBookListener
    {
        public void OnChanged(DxPriceLevelBook book)
        {
            Console.WriteLine($"\nNew Regional Order Book for {book.Symbol}:");
            Console.WriteLine($"{"Ask",-10} {"Size",-10} {"Time",-15} | {"Bid",-10} {"Size",-10} {"Time",-15}");
            for (var i = 0; i < Math.Max(book.Asks.Length, book.Bids.Length); ++i)
            {
                if (i < book.Asks.Length)
                    Console.Write("{0,-10:n4} {1,-10:n4} {2,-15:yyyyMMdd-HHmmss}", book.Asks[i].Price,
                        book.Asks[i].Size,
                        book.Asks[i].Time);
                else
                    Console.Write("{0,-10} {1,-10} {2,-15}", "", "", "");
                Console.Write(" | ");
                if (i < book.Bids.Length)
                    Console.Write("{0,-10:n4} {1,-10:n4} {2,-15:yyyyMMdd-HHmmss}", book.Bids[i].Price,
                        book.Bids[i].Size,
                        book.Bids[i].Time);
                Console.WriteLine();
            }
        }
    }

    internal class QuoteListener : IDxQuoteListener
    {
        public void OnQuote<TB, TE>(TB buf)
            where TB : IDxEventBuf<TE>
            where TE : IDxQuote
        {
            foreach (var q in buf)
                Console.WriteLine("{0} {1}", buf.Symbol, q);
        }
    }

    internal class Program
    {
        private const int HostIndex = 0;
        private const int SymbolIndex = 1;

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParam<string> param)
        {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 5)
            {
                Console.WriteLine(
                    "Usage: dxf_regional_book_sample <host:port> <symbol> [-T <token>] [-p]\n" +
                    "where\n" +
                    "    host:port  - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
                    "    symbol     - The regional book symbol (IBM, AAPL etc)\n" +
                    "    -T <token> - The authorization token\n" +
                    "    -p         - Enables the data transfer logging\n\n" +
                    "example: dxf_regional_book_sample demo.dxfeed.com:7300 MSFT\n"
                );
                return;
            }

            var address = args[HostIndex];
            var symbol = args[SymbolIndex];
            var token = new InputParam<string>(null);
            var logDataTransferFlag = false;

            for (var i = SymbolIndex + 1; i < args.Length; i++)
            {
                if (!token.IsSet && i < args.Length - 1 &&
                    TryParseTaggedStringParam("-T", args[i], args[i + 1], token))
                {
                    i++;
                    continue;
                }

                if (logDataTransferFlag == false && args[i].Equals("-p"))
                {
                    logDataTransferFlag = true;
                    i++;
                }
            }

            Console.WriteLine("Connecting to {0} on {1}...", address, symbol);

            try
            {
                NativeTools.InitializeLogging("dxf_regional_book_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler))
                {
                    using (con.CreateRegionalBook(symbol, new RegionalBookListener(), new QuoteListener()))
                    {
                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            }
            catch (DxException dxException)
            {
                Console.WriteLine("Native exception occurred: " + dxException.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occurred: " + exc.Message);
            }
        }

        private class InputParam<T>
        {
            private T value;

            private InputParam()
            {
                IsSet = false;
            }

            public InputParam(T defaultValue) : this()
            {
                value = defaultValue;
            }

            public bool IsSet { get; private set; }

            public T Value
            {
                get { return value; }
                set
                {
                    this.value = value;
                    IsSet = true;
                }
            }
        }
    }
}