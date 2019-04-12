using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.SignService.Services;

namespace Lykke.Bil2.Cardano.SignService.Services
{
    public class TransactionSigner : ITransactionSigner
    {
        public TransactionSigner(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext)
        {
            // TODO: sign transaction and return its body and hash
            //
            // For example:
            //
            // SignedTx signedTx;
            //
            // try
            // {
            //     signedTx = TxSigner.Sign(requestTransactionContext.DecodeToString(), privateKeys);
            // }
            // catch (FormatException ex)
            // {
            //     throw new RequestValidationException("Invalid transaction context, must be valid Cardano transaction",
            //         ex, nameof(requestTransactionContext));
            // }
            //
            // return Task.FromResult(new SignTransactionResponse
            // (
            //     signedTx.Raw.ToBase58(),
            //     signedTx.Hash
            // ));

            
            throw new System.NotImplementedException();
        }
    }
}
