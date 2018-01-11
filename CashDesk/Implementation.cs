using CashDesk.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {
        private Models.CashDeskContext db;

        /// <inheritdoc />
        public void InitializeDatabaseAsync()
        {
            if (db == null)
            {
                db = new Models.CashDeskContext();
            } else
            {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public int AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            this.checkInit();
            if (firstName == null || lastName == null)
            {
                throw new ArgumentException();
            }
            else
            {
                var duplicate = this.db.Members.Where(p => p.LastName.ToUpper().Equals(lastName.ToUpper())).ToArray();
                if (duplicate.Count() >= 1) 
                {
                    throw new DuplicateNameException();
                }
                else
                {
                    this.db.Members.Add(new Member { FirstName = firstName, LastName = lastName, Birthday = birthday });
                    this.db.SaveChanges();
                    var memberNumber = this.db.Members.Where(p => p.LastName.ToUpper().Equals(lastName.ToUpper())).ToArray().First();
                    return memberNumber.MemberNumber;
                }   
            }
        }

        /// <inheritdoc />
        public void DeleteMemberAsync(int memberNumber)
        {
            this.checkInit();
            if (memberNumber < 0)
            {
                throw new ArgumentException();
            }
            else
            {
                var member = this.db.Members.Where(p => p.MemberNumber.Equals(memberNumber)).ToArray().First();
                this.db.Members.Remove(member);
                this.db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public IMembership JoinMemberAsync(int memberNumber)
        {
            this.checkInit();
            if (memberNumber < 0)
            {
                throw new ArgumentException();
            }
            else
            {
                var member = this.db.Members.Where(p => p.MemberNumber.Equals(memberNumber)).First();
                if (this.db.Memberships.Where(p => p.End == null && p.Member.Equals(member)).Count() > 0)
                {
                    throw new AlreadyMemberException();
                }
                else
                {
                    var membership = this.db.Memberships.Add(new Membership { Member = member, Begin = DateTime.Now });
                    this.db.SaveChanges();
                    return membership.Entity;
                }
            }
        }

        /// <inheritdoc />
        public IMembership CancelMembershipAsync(int memberNumber)
        {
            this.checkInit();
            if (memberNumber < 0)
            {
                throw new ArgumentException();
            }
            else
            {
                var member = this.db.Members.Where(p => p.MemberNumber.Equals(memberNumber)).First();
                
                if (this.db.Memberships.Where(p => p.End == null && p.Member.Equals(member)).Count() > 0)
                {
                    var membership = this.db.Memberships.Where(p => p.Member.Equals(member)).First();
                    membership.End = DateTime.Now;
                    this.db.SaveChanges();
                    return membership;
                }
                else
                {
                    throw new NoMemberException();
                }
            }
        }

        /// <inheritdoc />
        public void DepositAsync(int memberNumber, decimal amount)
        {
            this.checkInit();

            if (memberNumber < 0 || amount < 0)
            {
                throw new ArgumentException();
            } 
            else
            {
                var member = this.db.Members.Where(p => p.MemberNumber.Equals(memberNumber)).First();
                var membership = this.db.Memberships.Where(p => p.End == null && p.Member.Equals(member)).First();

                if (membership == null)
                {
                    throw new NoMemberException();
                }
                else
                {
                    this.db.Deposits.Add(new Deposit { Membership = membership, Amount = amount });
                    this.db.SaveChanges();
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDepositStatistics> GetDepositStatisticsAsync()
        {
            this.checkInit();
            var deposit = this.db.Deposits.GroupBy(p => new { Year = p.Membership.Begin.Year, Member = p.Membership.Member }).Select(p => new DepositStatistics { Member = p.Key.Member, Year = p.Key.Year, TotalAmount = p.Sum(t => t.Amount) }).ToList();
            return deposit;
        }

        /// <inheritdoc />
        public void Dispose() { }

        public void checkInit()
        {
            if (this.db == null)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
