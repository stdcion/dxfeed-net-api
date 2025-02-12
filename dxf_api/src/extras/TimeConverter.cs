#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

using System;

namespace com.dxfeed.api.extras
{
    /// <summary>
    /// Helper class for converting DateTime to UTC UnixTime milliseconds and back.
    /// </summary>
    public class TimeConverter
    {
        private static readonly DateTime offset = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts UTC UnixTime milliseconds to DateTime
        /// </summary>
        /// <param name="utcMillis">The UTC UnixTime milliseconds</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime ToUtcDateTime(long utcMillis)
        {
            return offset.AddMilliseconds(utcMillis);
        }

        /// <summary>
        /// Converts DateTime to UTC UnixTime milliseconds
        /// </summary>
        /// <param name="time">DateTime</param>
        /// <returns>The UTC UnixTime milliseconds</returns>
        public static long ToUnixTime(DateTime time)
        {
            return (time - offset).Milliseconds;
        }
    }
}
