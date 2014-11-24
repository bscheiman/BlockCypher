#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlockCypher.Helpers;
using BlockCypher.Objects;
using bscheiman.Common.Extensions;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using RestSharp;

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

        public Task<AddressBalance> GetBalanceForAddress(string address) {
            return GetAsync<AddressBalance>("addrs/{address}", null, new Parameter {
                Name = "address",
                Value = address,
                Type = ParameterType.UrlSegment
            });
        }

        public IEnumerable<Task<AddressBalance>> GetBalanceForAddresses(params string[] addresses) {
            return addresses.Select(GetBalanceForAddress);
        }

        public Task Send(AddressInfo fromAddress, AddressInfo toAddress, Satoshi amount) {
            return Send(fromAddress.Address, toAddress.Address, fromAddress.Private, fromAddress.Public, amount);
        }

        public async Task<UnsignedTransaction> Send(string fromAddress, string toAddress, string fromPrivate, string fromPublic,
            Satoshi amount) {
            var unsignedTx = await PostAsync<UnsignedTransaction>("txs/new", new Transaction {
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

            var privateKey = fromPrivate.FromHexString();
            var pubKeys = new List<string>();
            var signatures = new List<string>();

            foreach (var s in unsignedTx.Transactions.Inputs.SelectMany(i => i.Addresses)) {
                pubKeys.Add(fromPublic);

                var ecParams = new ECDomainParameters(SecNamedCurves.GetByName("secp256k1").Curve, SecNamedCurves.GetByName("secp256k1").G,
                    SecNamedCurves.GetByName("secp256k1").N);
                var signer = new ECDsaSigner();
                signer.Init(true, new ECPrivateKeyParameters(new BigInteger(1, privateKey), ecParams));

                var sig = signer.GenerateSignature(Base58Helper.Decode(s));

                using (var ms = new MemoryStream())
                using (var asn = new Asn1OutputStream(ms)) {
                    var seq = new DerSequenceGenerator(asn);
                    seq.AddObject(new DerInteger(sig[0]));
                    seq.AddObject(new DerInteger(sig[1]));
                    seq.Close();

                    signatures.Add(ms.ToArray().ToHexString());
                }
            }

            unsignedTx.Signatures = signatures;
            unsignedTx.PubKeys = pubKeys;

            return await PostAsync<UnsignedTransaction>("txs/send", unsignedTx);
        }

        #region Helpers
        internal Task<T> DeleteAsync<T>(string url, params Parameter[] parameters) where T : new() {
            var tcs = new TaskCompletionSource<T>();
            var client = GetClient(url);

            client.ExecuteAsync(GetRequest(url, Method.DELETE, null, parameters), response => tcs.SetResult(response.Content.FromJson<T>()));

            return tcs.Task;
        }

        internal Task<T> GetAsync<T>(string url, object obj = null, params Parameter[] parameters) {
            var tcs = new TaskCompletionSource<T>();
            var client = GetClient(url);

            client.ExecuteAsync(GetRequest(url, Method.GET, obj, parameters), response => tcs.SetResult(response.Content.FromJson<T>()));

            return tcs.Task;
        }

        internal RestClient GetClient(string url) {
            var client = new RestClient(BaseUrl) {
                UserAgent = "Blockcypher.NET // @bscheiman"
            };

            return client;
        }

        internal RestRequest GetRequest(string url, Method method, object obj, params Parameter[] parameters) {
            var request = new RestRequest(url, method);

            foreach (var p in parameters)
                request.AddParameter(p);

            if (!string.IsNullOrEmpty(UserToken)) {
                request.AddParameter(new Parameter {
                    Name = "token",
                    Type = ParameterType.QueryString,
                    Value = UserToken
                });
            }

            request.AddParameter("application/json", (obj ?? new {
            }).ToJson(), ParameterType.RequestBody);

            return request;
        }

        internal Task<T> PostAsync<T>(string url, object obj, params Parameter[] parameters) where T : new() {
            var tcs = new TaskCompletionSource<T>();
            var client = GetClient(url);

            client.ExecuteAsync(GetRequest(url, Method.POST, obj, parameters), response => tcs.SetResult(response.Content.FromJson<T>()));

            return tcs.Task;
        }

        internal Task<T> PutAsync<T>(string url, object obj, params Parameter[] parameters) where T : new() {
            var tcs = new TaskCompletionSource<T>();
            var client = GetClient(url);

            client.ExecuteAsync(GetRequest(url, Method.PUT, obj, parameters), response => tcs.SetResult(response.Content.FromJson<T>()));

            return tcs.Task;
        }
        #endregion
    }
}