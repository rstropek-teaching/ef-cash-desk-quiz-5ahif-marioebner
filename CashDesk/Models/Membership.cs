using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk.Models
{
    class Membership : IMembership
    {
        [NotMapped]
        IMember IMembership.Member
        {
            get
            {
                return Member;
            }
        }

        [Key]
        public int MembershipId { get; set; }

        [Required]
        public Member Member { get; set; }

        [Required]
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }
}
