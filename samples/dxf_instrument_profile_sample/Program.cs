﻿#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using com.dxfeed.ipf;

namespace dxf_instrument_profile_sample
{
    internal class Program
    {
        private const int MaxPrintCount = 7;

        private static bool IsFilePath(string path)
        {
            try
            {
                return new Uri(path).IsFile;
            }
            catch (UriFormatException)
            {
                //This exception used for determine that path is not valid uri.
                return true;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
                "Usage: dxf_instrument_profile_sample <host> <user> <password>\n" +
                "or:    dxf_instrument_profile_sample <file>\n" +
                "or:    dxf_instrument_profile_sample <host> -T <token>\n" +
                "where\n" +
                "    host      - The valid host to download instruments (https://tools.dxfeed.com/ipf)\n" +
                "    user      - The user name to host access\n" +
                "    password  - The user password to host access\n" +
                "    file      - The name of file or archive (.gz or .zip) contains instrument profiles\n" +
                "    token     - The bearer token\n" +
                "\n" +
                "example: dxf_instrument_profile_sample https://tools.dxfeed.com/ipf demo demo\n" +
                "or:      dxf_instrument_profile_sample profiles.zip\n" +
                "or:      dxf_instrument_profile_sample https://tools.dxfeed.com/ipf token\n"
            );
        }

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();

                return;
            }

            var path = args[0];
            var user = string.Empty;
            var password = string.Empty;
            var token = string.Empty;
            const string ZIP_FILE_PATH = "profiles.zip";
            try
            {
                var reader = new InstrumentProfileReader();
                IList<InstrumentProfile> profiles;
                if (IsFilePath(path))
                {
                    //Read profiles from local file system
                    using (var inputStream = new FileStream(path, FileMode.Open))
                    {
                        profiles = reader.Read(inputStream, path);
                    }
                }
                else
                {
                    if (args.Length >= 2)
                    {
                        if (args[1].Equals("-T"))
                        {
                            if (args.Length >= 3)
                            {
                                token = args[2];
                            }
                            else
                            {
                                PrintUsage();

                                return;
                            }
                        }
                        else
                        {
                            user = args[1];
                            if (args.Length >= 3)
                                password = args[2];
                        }
                    }

                    //Read profiles from server
                    profiles = string.IsNullOrEmpty(token)
                        ? reader.ReadFromFile(path, user, password)
                        : reader.ReadFromFile(path, token);
                }

                //Iterate through received profiles
                Console.WriteLine("Profiles from '{0}' count: {1}", path, profiles.Count);
                Console.WriteLine("Print first {0} instruments:", MaxPrintCount);
                for (var i = 0; i < Math.Min(profiles.Count, MaxPrintCount); i++)
                    Console.WriteLine("#{0}:{1}", i, profiles[i]);
                if (profiles.Count > MaxPrintCount)
                    Console.WriteLine("   {0} instruments left...", profiles.Count - MaxPrintCount);

                //Write profiles to local file system
                var writer = new InstrumentProfileWriter();
                writer.WriteToFile(ZIP_FILE_PATH, profiles);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception occurred: {exc}");
            }
        }
    }
}