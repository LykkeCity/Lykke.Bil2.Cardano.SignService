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
        /// Create object used for addressing a specific output of a transaction built from a TxId (hash of the tx) and the offset in the outputs of this transaction.
        /// The memory must be freed with <see cref="cardano_transaction_output_ptr_delete"/>.
        /// </summary>
        /// <param name="txid">Tx hash.</param>
        /// <param name="index">Tx output index.</param>
        /// <returns>Pointer to the object in unmanaged memory.</returns>
        [DllImport("cardano_c")]
        public static extern IntPtr cardano_transaction_output_ptr_new(byte[] txid, int index);

        /// <summary>
        /// Free the memory of an object allocated with <see cref="cardano_transaction_output_ptr_new"/>.
        /// </summary>
        /// <param name="txo">Pointer to the object in unmanaged memory.</param>
        [DllImport("cardano_c")]
        public static extern void cardano_transaction_output_ptr_delete(IntPtr txo);
    }
}