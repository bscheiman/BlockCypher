#region
using System.ComponentModel;

#endregion

namespace BlockCypher {
    public enum HookEvent {
        UnconfirmedTransaction,
        NewBlock,
        ConfirmedTransaction,
        TransactionConfirmation,
        DoubleSpendTransaction
    }
}