#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using com.dxfeed.api.data;
using com.dxfeed.api.extras;
using com.dxfeed.native;
using System;
using System.IO;
using System.Threading;

namespace dxf_issue
{
    internal static class Program
    {
        private static void TestFileHandlingCycle()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;
            string testFile = Path.Combine(projectDirectory, "test_file(IBM).tmp");
            Console.WriteLine($"While processing: " + testFile);
            while (true)
            {
                using (var connection = new NativeConnection(testFile, null))
                {
                    connection.CreateSubscription(EventType.TimeAndSale, 0L, new EventPrinter()).AddSymbol("IBM");
                    Thread.Sleep(100);
                }
            }
        }

        private static void Main(string[] args)
        {
            TestFileHandlingCycle();
        }
    }
}