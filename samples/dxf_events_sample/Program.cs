﻿#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using com.dxfeed.api;
using com.dxfeed.api.data;
using com.dxfeed.native;

namespace dxf_events_sample
{
    /// <summary>
    ///     This sample class demonstrates subscription to events.
    ///     The sample configures via command line, subscribes to events and prints received data.
    /// </summary>
    internal static class Program
    {
        private const int HostIndex = 0;
        private const int EventIndex = 1;
        private const int SymbolIndex = 2;

        private static bool TryParseDateTimeParam(string stringParam, InputParameter<DateTime?> param)
        {
            DateTime dateTimeValue;

            if (!DateTime.TryParse(stringParam, out dateTimeValue)) return false;

            param.Value = dateTimeValue;

            return true;
        }

        private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
            InputParameter<string> param)
        {
            if (!paramTagString.Equals(tag)) return false;

            param.Value = paramString;

            return true;
        }

        private static void DisconnectHandler(IDxConnection con)
        {
            Console.WriteLine("Disconnected");
        }

        private static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 7)
            {
                Console.WriteLine(
                    "Usage: dxf_events_sample <host:port> <event> <symbol> [<date>] [-T <token>] [-p]\n" +
                    "where\n" +
                    "    host:port  - The address of dxFeed server (demo.dxfeed.com:7300)\n" +
                    "    event      - Any of the {Profile,Order,Quote,Trade,TimeAndSale,Summary,\n" +
                    "                 TradeETH,SpreadOrder,Greeks,TheoPrice,Underlying,Series,\n" +
                    "                 Configuration}\n" +
                    "    symbol     - IBM, MSFT, ...\n\n" +
                    "    date       - The date of time series event in the format YYYY-MM-DD (optional)\n" +
                    "    -T <token> - The authorization token\n" +
                    "    -p         - Enables the data transfer logging\n\n" +
                    "example: dxf_events_sample demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST\n" +
                    "or: dxf_events_sample demo.dxfeed.com:7300 TimeAndSale MSFT,IBM 2016-10-10\n"
                );
                return;
            }

            var address = args[HostIndex];

            EventType events;
            if (!Enum.TryParse(args[EventIndex], true, out events))
            {
                Console.WriteLine($"Unsupported event type: {args[EventIndex]}");
                return;
            }

            var symbols = args[SymbolIndex].Split(',');
            var dateTime = new InputParameter<DateTime?>(null);
            var token = new InputParameter<string>(null);
            var logDataTransferFlag = false;

            for (var i = SymbolIndex + 1; i < args.Length; i++)
            {
                if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime)) continue;

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

            Console.WriteLine($"Connecting to {address} for [{events}] on [{string.Join(", ", symbols)}] ...");

            try
            {
                NativeTools.InitializeLogging("dxf_events_sample.log", true, true, logDataTransferFlag);
                using (var con = token.IsSet
                    ? new NativeConnection(address, token.Value, DisconnectHandler)
                    : new NativeConnection(address, DisconnectHandler))
                {
                    using (var s = dateTime.IsSet
                        ? con.CreateSubscription(events, dateTime.Value, new EventListener())
                        : con.CreateSubscription(events, new EventListener()))
                    {
                        s.AddSymbols(symbols);

                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                    }
                }
            }
            catch (DxException dxException)
            {
                Console.WriteLine($"Native exception occurred: {dxException.Message}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception occurred: {exc.Message}");
            }
        }
    }
}