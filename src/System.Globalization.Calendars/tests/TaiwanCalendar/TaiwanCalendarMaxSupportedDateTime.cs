// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Globalization.Tests
{
    public class TaiwanCalendarMaxSupportedDateTime
    {
        [Fact]
        public void MaxSupportedDateTime()
        {
            Assert.Equal(DateTime.MaxValue, new TaiwanCalendar().MaxSupportedDateTime);
        }
    }
}
