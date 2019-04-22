using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.SignService.Models;
using Lykke.Bil2.Sdk.SignService.Services;
using static Lykke.Bil2.Cardano.Native;

namespace Lykke.Bil2.Cardano.SignService.Services
{
    public class AddressGenerator : IAddressGenerator
    {
        private readonly Native.cardano_protocol_magic _protocolMagic;

        public AddressGenerator(string network)
        {
            _protocolMagic = Enum.Parse<Native.cardano_protocol_magic>(network);
        }

        public Task<AddressCreationResult> CreateAddressAsync()
        {
            // generate non-deterministic independent random private key and address

            var (address, xprv) = cardano_address_new(_protocolMagic);

            return Task.FromResult
            (
                new AddressCreationResult
                (
                    address,
                    xprv.EncodeToBase58().ToString()
                )
            );
        }

        public Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, CreateAddressTagRequest request)
        {
            throw new OperationNotSupportedException();
        }
    }
}
