using Lykke.Bil2.Contract.Common;

namespace Lykke.Bil2.Cardano.SignService.Models
{
    public class TransactionContext
    {
        public Input[] Inputs { get; set; }
        public Output[] Outputs { get; set; }
    }

    public class Input
    {
        public string Address { get; set; }
        public string TxId { get; set; }
        public uint Index { get; set; }
        public ulong Amount { get; set; }
    }

    public class Output
    {
        public string Address { get; set; }
        public ulong Amount { get; set; }
    }
}