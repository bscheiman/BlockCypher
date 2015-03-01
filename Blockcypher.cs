#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BlockCypher.Helpers;
using BlockCypher.Objects;
using BlockCypher.Pcl;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

#endregion

namespace BlockCypher {
    public class Blockcypher {
        public Uri BaseUrl { get; set; }
        public Endpoint Endpoint { get; set; }
        public string UserToken { get; set; }

        public Blockcypher(string token = "", Endpoint endpoint = Endpoint.BtcMain) {
            UserToken = token;
            Endpoint = endpoint;

            switch (endpoint) {
                case Endpoint.BcyTest:
                    BaseUrl = new Uri("https://api.blockcypher.com/v1/bcy/test");
                    break;

                case Endpoint.BtcMain:
                    BaseUrl = new Uri("https://api.blockcypher.com/v1/btc/main");
                    break;

                case Endpoint.BtcTest3:
                    BaseUrl = new Uri("https://api.blockcypher.com/v1/btc/test3");
                    break;

                case Endpoint.LtcMain:
                    BaseUrl = new Uri("https://api.blockcypher.com/v1/ltc/main");
                    break;

                case Endpoint.UroMain:
                    BaseUrl = new Uri("https://api.blockcypher.com/v1/uro/main");
                    break;

                case Endpoint.DogeMain:
                    BaseUrl = new Uri("https://api.blockcypher.com/v1/doge/main");
                    break;
            }
        }

        public Task<Faucet> Faucet(string address, Satoshi amount) {
            if (Endpoint != Endpoint.BcyTest)
                throw new Exception("Invalid endpoint: faucet is only allowed for BcyTest");

            return PostAsync<Faucet>("faucet", new {
                address,
                amount = (int) amount.Value
            });
        }

        public Task<AddressInfo> GenerateAddress() {
            return PostAsync<AddressInfo>("addrs", null);
        }

        public Task<HookInfo> GenerateHook(string address, HookEvent hook, string url) {
            string evt = "";

            switch (hook) {
                case HookEvent.ConfirmedTransaction:
                    evt = "confirmed-tx";
                    break;

                case HookEvent.DoubleSpendTransaction:
                    evt = "double-spend-tx";
                    break;

                case HookEvent.NewBlock:
                    evt = "new-block";
                    break;

                case HookEvent.TransactionConfirmation:
                    evt = "tx-confirmation";
                    break;

                case HookEvent.UnconfirmedTransaction:
                    evt = "unconfirmed-tx";
                    break;
            }

            return PostAsync<HookInfo>("hooks", new {
                @event = evt,
                url,
                address
            });
        }

        public Task<AddressBalance> GetBalanceForAddress(string address) {
            return GetAsync<AddressBalance>(string.Format("addrs/{0}", address));
        }

        public IEnumerable<Task<AddressBalance>> GetBalanceForAddresses(params string[] addresses) {
            return addresses.Select(GetBalanceForAddress);
        }

        public Task<Transaction[]> GetTransactions(AddressInfo fromAddress) {
            return GetTransactions(fromAddress.Public);
        }

        public async Task<Transaction[]> GetTransactions(string fromAddress) {
            var addressInfo = await GetBalanceForAddress(fromAddress);

            if (addressInfo.Transactions == null)
                return new Transaction[0];

            var txs = addressInfo.Transactions.Select(t => t.TxHash).Distinct().ToArray();
            var groups = txs.Select((x, i) => new {
                Key = i / 40,
                Value = x
            }).GroupBy(x => x.Key, x => x.Value, (k, g) => g.ToArray()).ToArray();

            var list = new List<Transaction>();

            foreach (string url in groups.Select(g => string.Format("txs/{0}", string.Join(";", g)))) {
                var transactions = await GetAsync<Transaction[]>(url);

                if (transactions != null)
                    list.AddRange(transactions);
            }

            return list.OrderBy(t => t.Confirmed).ToArray();
        }

        public Task<UnsignedTransaction> Send(AddressInfo fromAddress, AddressInfo toAddress, Satoshi amount) {
            return Send(fromAddress.Address, toAddress.Address, fromAddress.Private, fromAddress.Public, amount);
        }

        public async Task<UnsignedTransaction> Send(string fromAddress, string toAddress, string fromPrivate, string fromPublic,
                                                    Satoshi amount) {
            var unsignedTx = await PostAsync<UnsignedTransaction>("txs/new", new BasicTransaction {
                Inputs = new[] {
                    new TxInput {
                        Addresses = new[] {
                            fromAddress
                        }
                    }
                },
                Outputs = new[] {
                    new TxOutput {
                        Addresses = new[] {
                            toAddress
                        },
                        Value = amount
                    }
                }
            });

            // SIGN

            unsignedTx.Signatures = new List<string>();
            unsignedTx.PubKeys = new List<string>();

            Sign(unsignedTx, fromPrivate, true, true, true);

            return await PostAsync<UnsignedTransaction>("txs/send", unsignedTx);
        }

        private static byte[] GetBytesFromBase58Key(string privateKey) {
            var tmp = Base58Helper.DecodeWithCheckSum(privateKey);
            var bytes = new byte[tmp.Length - 1];

            Array.Copy(tmp, 1, bytes, 0, tmp.Length - 1);

            return bytes;
        }

        private static void Sign(UnsignedTransaction unsignedTransaction, string privateKey, bool isHex, bool addPubKey,
                                 bool forceCompressed = false) {
            bool compressed = false;
            var bytes = isHex ? privateKey.FromHexString() : GetBytesFromBase58Key(privateKey);

            if (bytes.Length == 33 && bytes[32] == 1) {
                compressed = true;
                bytes = bytes.Take(32).ToArray();
            }

            var privKeyB = new BigInteger(1, bytes);
            var parms = SecNamedCurves.GetByName("secp256k1");
            var curve = new ECDomainParameters(parms.Curve, parms.G, parms.N, parms.H);
            var halfCurveOrder = parms.N.ShiftRight(1);

            var point = curve.G.Multiply(privKeyB);

            if (compressed || forceCompressed)
                point = new FpPoint(curve.Curve, point.X, point.Y, true);

            var publicKey = point.GetEncoded();
            var signer = new ECDsaSigner();
            var privKey = new ECPrivateKeyParameters(privKeyB, curve);
            signer.Init(true, privKey);

            foreach (string toSign in unsignedTransaction.ToSign) {
                if (addPubKey)
                    unsignedTransaction.PubKeys.Add(publicKey.ToHexString());

                var components = signer.GenerateSignature(toSign.FromHexString());
                var r = components[0];
                var s = components[1];

                if (s.CompareTo(halfCurveOrder) > 0)
                    s = curve.N.Subtract(s);

                using (var ms = new MemoryStream())
                using (var asn = new Asn1OutputStream(ms)) {
                    var seq = new DerSequenceGenerator(asn);
                    seq.AddObject(new DerInteger(r));
                    seq.AddObject(new DerInteger(s));

                    seq.Close();

                    string signedString = ms.ToArray().ToHexString();

                    unsignedTransaction.Signatures.Add(signedString);
                }
            }
        }

        #region Helpers
        internal async Task<T> GetAsync<T>(string url) {
            var client = GetClient();

            var response = await client.GetAsync(string.Format("{0}/{1}", BaseUrl, url));
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            return content.FromJson<T>();
        }

        internal HttpClient GetClient() {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        internal async Task<T> PostAsync<T>(string url, object obj) where T : new() {
            var client = GetClient();

            var response =
                await
                    client.PostAsync(string.Format("{0}/{1}", BaseUrl, url),
                        new StringContent((obj ?? new object()).ToJson(), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            return content.FromJson<T>();
        }
        #endregion
    }
}