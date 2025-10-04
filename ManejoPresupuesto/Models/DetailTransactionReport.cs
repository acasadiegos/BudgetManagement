namespace ManejoPresupuesto.Models
{
    public class DetailTransactionReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<TransactionsByDate> GroupTransactions { get; set; } = new List<TransactionsByDate>();
        public decimal DepositsBalance => GroupTransactions.Sum(x => x.DepositsBalance);
        public decimal WithDrawalsBalance => GroupTransactions.Sum(x => x.WithDrawalsBalance);
        public decimal Total => DepositsBalance - WithDrawalsBalance;
        public class TransactionsByDate
        {
            public DateTime TransactionDate { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
            public decimal DepositsBalance => 
                Transactions.Where(x => x.OperationTypeId == OperationType.Income)
                .Sum(x => x.Amount);

            public decimal WithDrawalsBalance =>
                Transactions.Where(x => x.OperationTypeId == OperationType.Expense)
                .Sum(x => x.Amount);

        }
    }
}
