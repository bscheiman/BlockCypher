#region
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Org.BouncyCastle.Math.EC;
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

        public Task<HookInfo> GenerateHook(string address, HookEvent hook, string url) {
            return PostAsync<HookInfo>("hooks", new {
                @event = hook.GetAttributeOfType<DescriptionAttribute>().Description,
                url,
                address
            });
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

        private static byte[] GetBytesFromBase58Key(string privateKey) {
            var tmp = Base58Helper.DecodeWithCheckSum(privateKey);
            var bytes = new byte[tmp.Length - 1];

            Array.Copy(tmp, 1, bytes, 0, tmp.Length - 1);

            return bytes;
        }

        public Task<UnsignedTransaction> Send(AddressInfo fromAddress, AddressInfo toAddress, Satoshi amount) {
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

            unsignedTx.Signatures = new List<string>();
            unsignedTx.PubKeys = new List<string>();

            Sign(unsignedTx, fromPrivate, true, true, true);

            return await PostAsync<UnsignedTransaction>("txs/send", unsignedTx);
        }

        private static void Sign(UnsignedTransaction unsignedTransaction, string privateKey, bool isHex, bool addPubKey,
            bool forceCompressed = false) {
            bool compressed = false;
            byte[] bytes = isHex ? privateKey.FromHexString() : GetBytesFromBase58Key(privateKey);

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

            foreach (var toSign in unsignedTransaction.ToSign) {
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

                    var signedString = ms.ToArray().ToHexString();

                    unsignedTransaction.Signatures.Add(signedString);
                }
            }
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
                UserAgent = "Blockcypher.NET"
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