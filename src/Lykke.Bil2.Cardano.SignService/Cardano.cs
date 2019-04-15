using System;
using System.Runtime.InteropServices;

namespace Lykke.Bil2.Cardano.SignService
{
    internal static class Cardano
    {
        public enum cardano_bip39_error
        {
            BIP39_SUCCESS = 0,
            BIP39_INVALID_MNEMONIC = 1,
            BIP39_INVALID_CHECKSUM = 2,
            BIP39_INVALID_WORD_COUNT = 3
        }

        public enum cardano_transaction_error
        {
            CARDANO_TRANSACTION_SUCCESS = 0,
            CARDANO_TRANSACTION_NO_OUTPUT = 1,
            CARDANO_TRANSACTION_NO_INPUT = 2,
            CARDANO_TRANSACTION_SIGNATURE_MISMATCH = 3,
            CARDANO_TRANSACTION_OVER_LIMIT = 4,
            CARDANO_TRANSACTION_SIGNATURES_EXCEEDED = 5,
            CARDANO_TRANSACTION_COIN_OUT_OF_BOUNDS = 6
        }

        public enum cardano_result
        {
            CARDANO_RESULT_SUCCESS = 0,
            CARDANO_RESULT_ERROR = 1
        }

        public enum cardano_protocol_magic
        {
            mainnet = 764824073,
            testnet = 1097911063
        }

        [DllImport("cardano_c")]
        public static extern int cardano_address_is_valid(string address_base58);

        /// <summary>
        /// Construct extended private key from the given bytes.
        /// The memory must be freed with <see cref="cardano_xprv_delete"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="xprv"></param>
        /// <returns></returns>
        [DllImport("cardano_c")]
        public static extern cardano_result cardano_xprv_from_bytes(byte[] bytes, ref IntPtr xprv);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_xprv_from_bytes"/>.
        /// </summary>
        /// <param name="xprv">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_xprv_delete(IntPtr xprv);

        /// <summary>
        /// Get the associated public key by private key.
        /// The memory must be freed with <see cref="cardano_xpub_delete"/>.
        /// </summary>
        /// <param name="xprv">Pointer to the private key.</param>
        /// <returns>Pointer to the object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_xprv_to_xpub(IntPtr xprv);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_xprv_to_xpub"/>.
        /// </summary>
        /// <param name="xpub">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_xpub_delete(IntPtr xpub);

        /// <summary>
        /// Derieves address from public key.
        /// The memory must be freed with <see cref="cardano_address_delete"/>.
        /// </summary>
        /// <param name="xpub">Pointer to the public key.</param>
        /// <param name="protocol_magic">Network.</param>
        /// <returns>Pointer to the object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_address_new_from_pubkey(IntPtr xpub, cardano_protocol_magic protocol_magic);

        /// <summary>
        /// Encodes address bytes to base58 string.
        /// </summary>
        /// <param name="address">Pointer to the address.</param>
        /// <returns>Address base58 representation.</returns>
        [DllImport("cardano_c")]
        public static extern string cardano_address_export_base58(IntPtr address);

        /// <summary>
        /// Created address object from base58 string.
        /// </summary>
        /// <param name="address">Address string.</param>
        /// <returns>Pointer to the address object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_address_import_base58(string address);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_address_new_from_pubkey"/>.
        /// </summary>
        /// <param name="address">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_address_delete(IntPtr address);

        /// <summary>
        /// Create builder for a transaction.
        /// The memory must be freed with <see cref="cardano_transaction_builder_delete"/>.
        /// </summary>
        /// <returns>Pointer to the object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_transaction_builder_new();

        /// <summary>
        /// Delete cardano_transaction_builder and free the associated memory.
        /// </summary>
        /// <param name="tb">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_builder_delete(IntPtr tb);

        /// <summary>
        /// Create object used for addressing a specific output of a transaction built from a TxId (hash of the tx) and the offset in the outputs of this transaction.
        /// The memory must be freed with <see cref="cardano_transaction_output_ptr_delete"/>.
        /// </summary>
        /// <param name="txid">Tx hash.</param>
        /// <param name="index">Tx output index.</param>
        /// <returns>Pointer to the object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_transaction_output_ptr_new(byte[] txid, uint index);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_output_ptr_new"/>.
        /// </summary>
        /// <param name="txo">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_output_ptr_delete(IntPtr txo);

        /// <summary>
        /// Add input to the transaction.
        /// </summary>
        /// <param name="tb">The builder for the transaction.</param>
        /// <param name="txo">Output pointer created with <see cref="cardano_transaction_output_ptr_new"/></param>
        /// <param name="value">The expected amount to spend, in "lovelace"</param>
        [DllImport("cardano_c")]
        public static extern cardano_transaction_error cardano_transaction_builder_add_input(IntPtr tb, IntPtr txo, ulong value);

        /// <summary>
        /// Create output for a transaction.
        /// The memory must be freed with <see cref="cardano_transaction_output_delete"/>.
        /// </summary>
        /// <param name="address">Destination address.</param>
        /// <param name="amount">Amount in "lovelace".</param>
        /// <returns>Pointer to the object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_transaction_output_new(IntPtr address, ulong amount);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_output_new"/>.
        /// </summary>
        /// <param name="txo">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_output_delete(IntPtr output);

        /// <summary>
        /// Add output to transaction.
        /// </summary>
        /// <param name="tb">The builder for the transaction.</param>
        /// <param name="txo">Output created with <see cref="cardano_transaction_output_new"/></param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_builder_add_output(IntPtr tb, IntPtr output);

        /// <summary>
        /// Get a transaction object.
        /// The memory must be freed with <see cref="cardano_transaction_delete"/>.
        /// </summary>
        /// <param name="tb">The builder for the transaction.</param>
        /// <param name="tx">Output pointer to the transaction object in unmanaged memory.</param>
        /// <returns></returns>
        [DllImport("cardano_c")]
        public static extern cardano_transaction_error cardano_transaction_builder_finalize(IntPtr tb, ref IntPtr tx);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_builder_finalize"/>.
        /// </summary>
        /// <param name="tx">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_delete(IntPtr tx);

        /// <summary>
        /// Take a transaction and create a working area for adding witnesses.
        /// The memory must be freed with <see cref="cardano_transaction_finalized_delete"/>.
        /// </summary>
        /// <param name="tx">Pointer to the transaction object in unmanaged memory.</param>
        /// <returns>Output pointer to the finalized transaction object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_transaction_finalized_new(IntPtr tx);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_finalized_new"/>.
        /// </summary>
        /// <param name="tx">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_finalized_delete(IntPtr tx);

        /// <summary>
        /// Add a witness associated with the next input.
        /// Witness need to be added in the same order to the inputs, otherwise protocol level mismatch will happen, and the transaction will be rejected.
        /// </summary>
        /// <param name="tf">Finalized transaction.</param>
        /// <param name="xprv">Private key.</param>
        /// <param name="protocol_magic">Network protocol magic.</param>
        /// <param name="txid">Transaction identifier.</param>
        /// <returns></returns>
        [DllImport("cardano_c")]
        public static extern cardano_transaction_error cardano_transaction_finalized_add_witness(IntPtr tf, byte[] xprv, uint protocol_magic, byte[] txid);

        /// <summary>
        /// Returns finalized transaction with the vector of witnesses.
        /// The memory must be freed with <see cref="cardano_transaction_signed_delete"/>.
        /// </summary>
        /// <param name="tf">A finalized transaction with witnesses.</param>
        /// <param name="txaux">Pointer to the object in unmanaged memory.</param>
        /// <returns></returns>
        [DllImport("cardano_c")]
        public static extern cardano_transaction_error cardano_transaction_finalized_output(IntPtr tf, ref IntPtr txaux);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_finalized_output"/>.
        /// </summary>
        /// <param name="tx">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_signed_delete(IntPtr txaux);

        /// <summary>
        /// Serializes signed transaction to byte array.
        /// </summary>
        /// <param name="txaux">Signed transaction.</param>
        /// <param name="bytes">Pointer to target result byte array.</param>
        /// <param name="size">Result array length.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_signed_to_bytes(IntPtr txaux, ref IntPtr bytes, ref uint size);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_signed_to_bytes"/>.
        /// </summary>
        /// <param name="bytes">Pointer to the object in unmanaged memory.</param>
        /// <param name="size">Array length.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_signed_delete_bytes(IntPtr bytes, uint size);
    }
}