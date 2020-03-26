using System;
using System.Linq;
using Xunit;

namespace Valuation.Common.Test
{
    public class DateExtensionTest
    {
        [Fact]
        public void ShouldReturnCorrectDateRange()
        {
            var from = new DateTime(2020, 01, 01);
            var to = from.AddDays(10);

            var range = DateTimeExtensions.GetDateRange(from, to);
            Assert.Equal(11, range.Count());
            Assert.True(range.All(date => date >= from && date <= to));
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 01)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 02)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 03)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 04)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 05)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 06)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 07)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 08)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 09)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 10)) != null);
            Assert.True(range.Single(d => d == new DateTime(2020, 01, 11)) != null);
        }
    }
}
