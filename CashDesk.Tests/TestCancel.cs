using System;
using System.Threading.Tasks;
using Xunit;

namespace CashDesk.Tests
{
    public class TestCancel
    {
        [Fact]
        public void InvalidParameters()
        {
            using (var dal = new DataAccess())
            {
                Assert.ThrowsAsync<ArgumentException>(async () => dal.CancelMembershipAsync(Int32.MaxValue));
            }
        }

        [Fact]
        public async Task CancelMember()
        {
            using (var dal = new DataAccess())
            {
                dal.InitializeDatabaseAsync();
                var memberNumber = dal.AddMemberAsync("Foo", "CancelMember", DateTime.Today.AddYears(-18));
                dal.JoinMemberAsync(memberNumber);
                dal.CancelMembershipAsync(memberNumber);

                // Make sure that member can join again
                dal.JoinMemberAsync(memberNumber);
            }
        }

        [Fact]
        public async Task NoMember()
        {
            using (var dal = new DataAccess())
            {
                dal.InitializeDatabaseAsync();
                var memberNumber = dal.AddMemberAsync("Foo", "NoMemberCancel", DateTime.Today.AddYears(-18));
                await Assert.ThrowsAsync<NoMemberException>(async () => dal.CancelMembershipAsync(memberNumber));
            }
        }
    }
}
