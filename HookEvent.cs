#region
using System.ComponentModel;

#endregion

namespace BlockCypher {
    public enum HookEvent {
        [Description("unconfirmed-tx")] UnconfirmedTransaction,
        [Description("new-block")] NewBlock,
        [Description("confirmed-tx")] ConfirmedTransaction,
        [Description("tx-confirmation")] TransactionConfirmation,
        [Description("double-spend-tx")] DoubleSpendTransaction
    }
}