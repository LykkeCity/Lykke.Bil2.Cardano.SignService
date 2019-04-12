# Blockchain integration sign service

This component of blockchain integration is intended for generation deposit addresses for new users and signing transactions.

> **IMPORTANT**
>
> It __must not__ use any logging to prevent sensitive data leaks.
> It __should not__ communicate with any other service if possible, even with blockchain node.

To implement signing component:

1. Implement `IAddressGenerator` interface for generating deposit addresses;

2. Implement `ITransactionSigner` interface for signing transactions;

3. Provide implementations to SDK code in `ConfigureServices()` method on startup;

4. Provide integration name and other options in `Configure()` method on startup.

