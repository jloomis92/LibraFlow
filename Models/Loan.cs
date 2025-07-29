using System;

namespace LibraFlow.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime CheckedOutDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Book Book { get; set; }
        public Member Member { get; set; }
    }
}
