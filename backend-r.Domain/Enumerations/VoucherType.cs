namespace backend_r.Domain.Enumerations
{
    public class VoucherType : Enumeration
    {
        public static VoucherType Loan_Principal = new VoucherType(1, nameof(Loan_Principal).ToLowerInvariant());
        public static VoucherType Save_Principal = new VoucherType(2, nameof(Save_Principal).ToLowerInvariant());
        public static VoucherType Share_Principal = new VoucherType(3, nameof(Share_Principal).ToLowerInvariant());
        public static VoucherType Loan_Interest = new VoucherType(4, nameof(Loan_Interest).ToLowerInvariant());
        public static VoucherType Save_Interest = new VoucherType(5, nameof(Save_Interest).ToLowerInvariant());
        public static VoucherType Share_Interest = new VoucherType(6, nameof(Share_Interest).ToLowerInvariant());
        public static VoucherType Loan_Penality = new VoucherType(7, nameof(Loan_Penality).ToLowerInvariant());
        public static VoucherType Save_Penality = new VoucherType(8, nameof(Save_Penality).ToLowerInvariant());
        public static VoucherType Share_Penality = new VoucherType(9, nameof(Share_Penality).ToLowerInvariant());
        public static VoucherType Deposit = new VoucherType(10, nameof(Deposit).ToLowerInvariant());
        public static VoucherType Withdrawal = new VoucherType(11, nameof(Withdrawal).ToLowerInvariant());
        public static VoucherType Daily_Loan_Bigining_Balance = new VoucherType(12, nameof(Daily_Loan_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Monthly_Loan_Bigining_Balance = new VoucherType(13, nameof(Monthly_Loan_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Yearly_Loan_Bigining_Balance = new VoucherType(14, nameof(Yearly_Loan_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Daily_Save_Bigining_Balance = new VoucherType(15, nameof(Daily_Save_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Monthly_Save_Bigining_Balance = new VoucherType(16, nameof(Monthly_Save_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Yearly_Save_Bigining_Balance = new VoucherType(17, nameof(Yearly_Save_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Daily_Share_Bigining_Balance = new VoucherType(18, nameof(Daily_Share_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Monthly_Share_Bigining_Balance = new VoucherType(19, 
            nameof(Monthly_Share_Bigining_Balance).ToLowerInvariant());
        public static VoucherType Yearly_Share_Bigining_Balance = new VoucherType(20, nameof(Yearly_Share_Bigining_Balance)
            .ToLowerInvariant());
        public static VoucherType Disbursement = new VoucherType(21, nameof(Disbursement).ToLowerInvariant());
        public static IEnumerable<VoucherType> List() => new [] { Loan_Principal, Loan_Interest, Loan_Penality,
            Save_Principal, Save_Interest, Save_Penality, Share_Principal, Share_Interest, Share_Penality, Deposit,
            Withdrawal, Daily_Loan_Bigining_Balance, Monthly_Loan_Bigining_Balance, Yearly_Loan_Bigining_Balance,
            Daily_Save_Bigining_Balance, Monthly_Save_Bigining_Balance, Yearly_Save_Bigining_Balance,
            Daily_Share_Bigining_Balance, Monthly_Share_Bigining_Balance, Yearly_Share_Bigining_Balance, Disbursement };

        private VoucherType() {}
        
        private VoucherType(int id, string name) : base(id, name) {}
    }
}