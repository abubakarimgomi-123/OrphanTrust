using System;
using System.Configuration;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace OrphanTrust.Infrastructure.Blockchain
{
    public class BlockchainService
    {
        private readonly Web3 _web3;
        private readonly Account _account;
        private readonly string _donationRegistryAddress;
        private readonly string _distributionLedgerAddress;
        private readonly string _orphanageVerifyAddress;

        private const string DonationRegistryAbi = @"[
          {""inputs"":[
            {""internalType"":""uint256"",""name"":""_donationId"",""type"":""uint256""},
            {""internalType"":""address"",""name"":""_donor"",""type"":""address""},
            {""internalType"":""uint256"",""name"":""_orphanageId"",""type"":""uint256""},
            {""internalType"":""uint256"",""name"":""_amount"",""type"":""uint256""},
            {""internalType"":""string"",""name"":""_currency"",""type"":""string""},
            {""internalType"":""string"",""name"":""_paymentReference"",""type"":""string""}
          ],
          ""name"":""recordDonation"",
          ""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],
          ""stateMutability"":""nonpayable"",""type"":""function""}
        ]";

        private const string OrphanageVerifyAbi = @"[
          {""inputs"":[
            {""internalType"":""uint256"",""name"":""_orphanageId"",""type"":""uint256""},
            {""internalType"":""string"",""name"":""_name"",""type"":""string""},
            {""internalType"":""string"",""name"":""_registrationNumber"",""type"":""string""},
            {""internalType"":""string"",""name"":""_region"",""type"":""string""}
          ],
          ""name"":""verifyOrphanage"",
          ""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],
          ""stateMutability"":""nonpayable"",""type"":""function""}
        ]";

        private const string DistributionLedgerAbi = @"[
          {""inputs"":[
            {""internalType"":""uint256"",""name"":""_distributionId"",""type"":""uint256""},
            {""internalType"":""uint256"",""name"":""_donationId"",""type"":""uint256""},
            {""internalType"":""uint256"",""name"":""_orphanageId"",""type"":""uint256""},
            {""internalType"":""uint256"",""name"":""_amount"",""type"":""uint256""},
            {""internalType"":""string"",""name"":""_purpose"",""type"":""string""},
            {""internalType"":""string"",""name"":""_evidenceHash"",""type"":""string""}
          ],
          ""name"":""recordDistribution"",
          ""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],
          ""stateMutability"":""nonpayable"",""type"":""function""}
        ]";

        public BlockchainService()
        {
            string rpcUrl = ConfigurationManager
                .AppSettings["AvalancheFujiRpcUrl"];
            string privateKey = ConfigurationManager
                .AppSettings["BlockchainAdminPrivateKey"];

            _donationRegistryAddress = ConfigurationManager
                .AppSettings["DonationRegistryContractAddress"];
            _distributionLedgerAddress = ConfigurationManager
                .AppSettings["DistributionLedgerContractAddress"];
            _orphanageVerifyAddress = ConfigurationManager
                .AppSettings["OrphanageVerifyContractAddress"];

            _account = new Account(privateKey, 43113);
            _web3 = new Web3(_account, rpcUrl);
            _web3.TransactionManager.UseLegacyAsDefault = true;
        }

        private HexBigInteger GetGasPrice()
        {
            // 25 Gwei — standard for Avalanche Fuji
            return new HexBigInteger(
                BigInteger.Multiply(
                    new BigInteger(25),
                    BigInteger.Pow(10, 9)));
        }

        public async Task<string> RecordDonationAsync(
            int donationId, string donorAddress,
            int orphanageId, decimal amount,
            string currency, string paymentReference)
        {
            var contract = _web3.Eth.GetContract(
                DonationRegistryAbi, _donationRegistryAddress);

            var function = contract.GetFunction("recordDonation");

            var txInput = function.CreateTransactionInput(
                from: _account.Address,
                gas: new HexBigInteger(300000),
                gasPrice: GetGasPrice(),
                value: new HexBigInteger(0),
                functionInput: new object[]
                {
                    new BigInteger(donationId),
                    _account.Address,
                    new BigInteger(orphanageId),
                    new BigInteger((long)(amount * 100)),
                    currency ?? "TZS",
                    paymentReference ?? ""
                });

            var txHash = await _web3.Eth.TransactionManager
                .SendTransactionAsync(txInput);

            return txHash;
        }

        public async Task<string> VerifyOrphanageAsync(
            int orphanageId, string name,
            string registrationNumber, string region)
        {
            var contract = _web3.Eth.GetContract(
                OrphanageVerifyAbi, _orphanageVerifyAddress);

            var function = contract.GetFunction("verifyOrphanage");

            var txInput = function.CreateTransactionInput(
                from: _account.Address,
                gas: new HexBigInteger(300000),
                gasPrice: GetGasPrice(),
                value: new HexBigInteger(0),
                functionInput: new object[]
                {
                    new BigInteger(orphanageId),
                    name ?? "",
                    registrationNumber ?? "",
                    region ?? ""
                });

            var txHash = await _web3.Eth.TransactionManager
                .SendTransactionAsync(txInput);

            return txHash;
        }

        public async Task<string> RecordDistributionAsync(
            int distributionId, int donationId,
            int orphanageId, decimal amount,
            string purpose, string evidenceHash)
        {
            var contract = _web3.Eth.GetContract(
                DistributionLedgerAbi, _distributionLedgerAddress);

            var function = contract.GetFunction("recordDistribution");

            var txInput = function.CreateTransactionInput(
                from: _account.Address,
                gas: new HexBigInteger(300000),
                gasPrice: GetGasPrice(),
                value: new HexBigInteger(0),
                functionInput: new object[]
                {
                    new BigInteger(distributionId),
                    new BigInteger(donationId),
                    new BigInteger(orphanageId),
                    new BigInteger((long)(amount * 100)),
                    purpose ?? "",
                    evidenceHash ?? ""
                });

            var txHash = await _web3.Eth.TransactionManager
                .SendTransactionAsync(txInput);

            return txHash;
        }

        public string GetExplorerUrl(string txHash)
        {
            return "https://testnet.snowtrace.io/tx/" + txHash;
        }
    }
}