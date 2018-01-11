using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashDesk.Tests
{
    public class TestDepositStatistics
    {
        [Fact]
        public async Task DepositStatistics()
        {
            using (var dal = new DataAccess())
            {
                dal.InitializeDatabaseAsync();
                var memberNumber = dal.AddMemberAsync("Foo", "DepositStatistics", DateTime.Today.AddYears(-18));
                dal.JoinMemberAsync(memberNumber);
                dal.DepositAsync(memberNumber, 100M);
                dal.DepositAsync(memberNumber, 100M);

                var statistics = dal.GetDepositStatisticsAsync();
                Assert.True(statistics.Count() > 0);
                Assert.True(statistics.Any(s => s.Member.MemberNumber == memberNumber));
                Assert.Equal(statistics.Where(s => s.Member.MemberNumber == memberNumber).Sum(s => s.TotalAmount), 200M);
            }
        }
    }
}
