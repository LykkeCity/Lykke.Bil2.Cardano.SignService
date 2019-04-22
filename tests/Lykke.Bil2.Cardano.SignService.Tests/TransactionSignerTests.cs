using System;
using System.Threading.Tasks;
using Lykke.Bil2.Cardano.SignService.Models;
using Lykke.Bil2.Cardano.SignService.Services;
using Lykke.Bil2.Contract.Common.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ShouldSignTransaction()
        {
            // Arrange

            var transactionSigner = new TransactionSigner("testnet");
            var keys = new[] { "Xdzn1yYpfXtckTgUM7eAZWCtxnm2yRgaMvurLoHBv5bmFvEZi9NWo2HcmLC5id77phWzgRLuFSbppHjRmZBKkx9L5PmHreae1zhyCFR1HoNQrY1NAoDXKDAsa1ziCJTf5iK" };
            var context = new TransactionContext
            {
                Inputs = new Input[]
                {
                    new Input
                    {
                        Address = "2cWKMJemoBakpuQuYAfemE7UMGSSbUwcXXg6BfyvdvNuTCmyMvwTAB76mpHyQ5aBwBHtn",
                        Amount = 1000,
                        Index = 0,
                        TxId = "785821260eac6dbf2c7cf7aa27820e6dba7f9c2d4e70e32a669e6cc457c3c99f"
                    }
                },
                Outputs = new Output[]
                {
                    new Output
                    {
                        Address = "37btjrVyb4KFzXj1Taj9G5nd5YHNNbRzz1rLbakbENt7rts5kVRx81GLvNbeCrA6CKo7HYDcSpSm1mw6No8S4wWfPKxXDPcha3H29wedDrDhuvYmEE",
                        Amount = 1000
                    }
                }
            };

            // Act

            var result = await transactionSigner.SignAsync(keys, JsonConvert.SerializeObject(context).ToBase58());

            // Assert

            Assert.AreEqual
            (
                "4e87c9f787cdef7ae75d646942cd2b0707ad747eb97ffd5a2d04c8ee1b31665e",
                result.TransactionId.ToString()
            );
        }
    }
}