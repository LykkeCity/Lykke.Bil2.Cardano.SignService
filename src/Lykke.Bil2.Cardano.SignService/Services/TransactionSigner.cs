using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Common;
using Lykke.Bil2.Cardano.SignService.Models;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.SignService.Services;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;
using static Lykke.Bil2.Cardano.Native;

namespace Lykke.Bil2.Cardano.SignService.Services
{
    public class TransactionSigner : ITransactionSigner
    {
        private readonly Native.cardano_protocol_magic _protocolMagic;

        public TransactionSigner(string network)
        {
            _protocolMagic = Enum.Parse<Native.cardano_protocol_magic>(network);
        }

        public Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext)
        {
            var ctx = JsonConvert.DeserializeObject<TransactionContext>(requestTransactionContext.DecodeToString());
            var tb = default(IntPtr); // transaction builder
            var inputs = Enumerable.Empty<IntPtr>(); // transaction inputs (pointers to previous transactions outputs)
            var outAddresses = Enumerable.Empty<(IntPtr output, IntPtr address)>(); // transaction outputs and destination addresses
            var tx = default(IntPtr); // transaction
            var tf = default(IntPtr); // finalized (ready to sign) transaction
            var xprvs = new Dictionary<string, IntPtr>(); //private keys
            var txaux = default(IntPtr); // signed transaction
            var txauxBytes = default(IntPtr); // signed transaction bytes
            var txauxBytesLength = 0U; // signed transaction size

            try
            {
                // create tx builder instance
                tb = cardano_transaction_builder_new();

                // add inputs (pointers to previous transactions outputs)
                inputs = ctx.Inputs
                    .Select(input =>
                    {
                        var txo = cardano_transaction_output_ptr_new(input.TxId.GetHexStringToBytes(), input.Index);
                        cardano_transaction_builder_add_input(tb, txo, input.Amount);
                        return txo;
                    })
                    .ToList();

                // add outputs
                outAddresses = ctx.Outputs
                    .Select(output =>
                    {
                        var address = cardano_address_import_base58(output.Address);
                        var txo = cardano_transaction_output_new(address, output.Amount);
                        cardano_transaction_builder_add_output(tb, txo);
                        return (txo, address);
                    })
                    .ToList();

                // get transaction
                var buildResult = cardano_transaction_builder_finalize(tb, ref tx);
                if (buildResult !=
                    cardano_transaction_error.CARDANO_TRANSACTION_SUCCESS)
                {
                    throw new BlockchainIntegrationException($"Failed to build transaction: {buildResult}");
                }

                // fill transaction identifier array with txid data
                var txid = new byte[32];
                cardano_transaction_id(tx, txid);

                // prepare transaction for signing
                tf = cardano_transaction_finalized_new(tx);

                // construct addreses from private keys
                xprvs = privateKeys
                    .Select(k =>
                    {
                        var xprv = default(IntPtr);
                        if (cardano_xprv_from_bytes(new Base58String(k).DecodeToBytes(), ref xprv) != cardano_result.CARDANO_RESULT_SUCCESS)
                        {
                            throw new RequestValidationException("Invalid private key(s)");
                        }
                        return xprv;
                    })
                    .ToDictionary
                    (
                        xprv => cardano_address_from_xprv(xprv, _protocolMagic),
                        xprv => xprv
                    );

                // sign each input with corresponding private key
                foreach (var input in ctx.Inputs)
                {
                    cardano_transaction_finalized_add_witness(tf, xprvs[input.Address], _protocolMagic, txid);
                }

                // construct signed transaction
                cardano_transaction_finalized_output(tf, ref txaux);

                // serialize signed transaction
                cardano_transaction_signed_bytes(txaux, ref txauxBytes, ref txauxBytesLength);
                
                var data = new byte[txauxBytesLength];
                
                Marshal.Copy(txauxBytes, data, 0, data.Length);

                return Task.FromResult
                (
                    new SignTransactionResponse
                    (
                        data.EncodeToBase58(),
                        txid.ToHexString().ToLower()
                    )
                );
            }
            finally
            {
                // unmanaged memory cleanup

                if (tb != default(IntPtr))
                    cardano_transaction_builder_delete(tb);

                foreach (var txo in inputs)
                    cardano_transaction_output_ptr_delete(txo);

                foreach (var outAddress in outAddresses)
                {
                    cardano_address_delete(outAddress.address);
                    cardano_transaction_output_delete(outAddress.output);
                }

                if (tx != default(IntPtr))
                    cardano_transaction_delete(tx);

                if (tf != default(IntPtr))
                    cardano_transaction_finalized_delete(tf);

                foreach (var xprv in xprvs.Values)
                    cardano_xprv_delete(xprv);
                
                if (txaux != default(IntPtr))
                    cardano_transaction_signed_delete(txaux);

                if (txauxBytes != default(IntPtr))
                    cardano_transaction_signed_bytes_delete(txauxBytes, txauxBytesLength);
            }
        }
    }
}
