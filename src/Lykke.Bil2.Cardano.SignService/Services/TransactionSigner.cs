using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Cardano.SignService.Models;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.SignService.Services;
using Newtonsoft.Json;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Cardano.SignService.Services
{
    public class TransactionSigner : ITransactionSigner
    {
        public TransactionSigner(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext)
        {
            var ctx = JsonConvert.DeserializeObject<TransactionContext>(requestTransactionContext.DecodeToString());
            var tb_ptr = default(IntPtr); // transaction builder
            var input_ptrs = Enumerable.Empty<IntPtr>(); // transaction inputs (pointers to previous transactions outputs)
            var out_address_ptrs = Enumerable.Empty<(IntPtr output, IntPtr address)>(); // transaction outputs and destination addresses
            var tx_ptr = default(IntPtr); // transaction
            var tf_ptr = default(IntPtr); // transaction ready to sign

            try
            {
                // create tx builder instance
                tb_ptr = Cardano.cardano_transaction_builder_new();

                // add inputs
                input_ptrs = ctx.Inputs.Select(input =>
                {
                    var txo_ptr = Cardano.cardano_transaction_output_ptr_new(new Base58String(input.TxId).DecodeToBytes(), input.Index);
                    Cardano.cardano_transaction_builder_add_input(tb_ptr, txo_ptr, input.Amount);
                    return txo_ptr;
                });

                // add outputs
                out_address_ptrs = ctx.Outputs.Select(output =>
                {
                    var address_ptr = Cardano.cardano_address_import_base58(output.Address);
                    var out_ptr = Cardano.cardano_transaction_output_new(address_ptr, output.Amount);
                    Cardano.cardano_transaction_builder_add_output(tb_ptr, out_ptr);
                    return (out_ptr, address_ptr);
                });

                var finalizationResult = Cardano.cardano_transaction_builder_finalize(tb_ptr, ref tx_ptr);
                if (finalizationResult !=
                    Cardano.cardano_transaction_error.CARDANO_TRANSACTION_SUCCESS)
                {
                    throw new BlockchainIntegrationException($"Failed to finalize transaction: {finalizationResult}");
                }

                tf_ptr = Cardano.cardano_transaction_finalized_new(tx_ptr);

                foreach (var k in privateKeys)
                {

                }
            }
            finally
            {
                // unmanaged memory cleanup

                if (tb_ptr != default(IntPtr))
                    Cardano.cardano_transaction_builder_delete(tb_ptr);

                foreach (var txo_ptr in input_ptrs)
                    Cardano.cardano_transaction_output_ptr_delete(txo_ptr);

                foreach (var out_address in out_address_ptrs)
                {
                    Cardano.cardano_address_delete(out_address.address);
                    Cardano.cardano_transaction_output_delete(out_address.output);
                }
            }
        }
    }
}
