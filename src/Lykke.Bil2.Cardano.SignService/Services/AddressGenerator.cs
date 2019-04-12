using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.SignService.Models;
using Lykke.Bil2.Sdk.SignService.Services;

namespace Lykke.Bil2.Cardano.SignService.Services
{
    public class AddressGenerator : IAddressGenerator
    {
        private readonly Cardano.cardano_protocol_magic _protocolMagic;

        public AddressGenerator(string network)
        {
            _protocolMagic = Enum. Parse<Cardano.cardano_protocol_magic>(network);
        }

        public Task<AddressCreationResult> CreateAddressAsync()
        {
            var seed = new byte[64];
            var xprv = new byte[96]; // eXtended PRiVate key

            // generate seed (we don't need BIP39 so just use cryptographic random generator)
            new RNGCryptoServiceProvider().GetBytes(seed);

            // make up the key data
            Array.Copy(SHA512.Create().ComputeHash(seed, 0, 32), xprv, 64);
            Array.Copy(seed, 32, xprv, 64, 32);

            // some Cardano magic
            xprv[0] &= 248;
            xprv[31] &= 63;
            xprv[31] |= 64;
            xprv[31] &= 0b1101_1111;

            var xprv_ptr = IntPtr.Zero; // XPRV pointer for P/Invoke

            if (Cardano.cardano_xprv_from_bytes(xprv, ref xprv_ptr) != Cardano.cardano_result.CARDANO_RESULT_SUCCESS)
            {
                throw new InvalidOperationException("XPRV generation failed");
            }

            var xpub_ptr = Cardano.cardano_xprv_to_xpub(xprv_ptr); // eXtended PUBlic key
            var addr_ptr = Cardano.cardano_address_new_from_pubkey(xpub_ptr, _protocolMagic); // address bytes
            var addr = Cardano.cardano_address_export_base58(addr_ptr); // address base58 representation

            // unmanaged memory cleanup
            Cardano.cardano_xprv_delete(xprv_ptr);
            Cardano.cardano_xpub_delete(xpub_ptr);
            Cardano.cardano_address_delete(addr_ptr);

            return Task.FromResult
            (
                new AddressCreationResult
                (
                    addr,
                    Base58String.Encode(xprv).Value
                )
            );
        }

        public Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, CreateAddressTagRequest request)
        {
            throw new OperationNotSupportedException();
        }
    }
}
