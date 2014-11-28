#region
using System;
using BlockCypher.Objects;

#endregion

namespace BlockCypher.Test {
    internal class Program {
        private static void Main(string[] args) {
            Console.WriteLine("Token?");
            var token = Console.ReadLine();

            var b = new Blockcypher(token, Endpoint.BtcTest3);

            var fromAddress = new AddressInfo("mtWg6ccLiZWw2Et7E5UqmHsYgrAi5wqiov", // TESTNET
                "1af97b1f428ac89b7d35323ea7a68aba8cad178a04eddbbf591f65671bae48a2", // HEX
                "03bb318b00de944086fad67ab78a832eb1bf26916053ecd3b14a3f48f9fbe0821f"); // COMPRESSED
            var toAddress = new AddressInfo("n2XgDw5DixTyxFKKxFcHuK5KSaUvS2UAoj", // TESTNET
                "477fcfc70eafd99f235d3050c4b8f4a1a6ae9b8e1a7f5cc1a8abb49e901f0943", // HEX
                "034f6f8a9e6ce7d665f70fd1875b4d8f8138bcc8f6c75f8a030a6dd194cc2c197b"); // COMPRESSED

            //Console.WriteLine(b.Send(fromAddress, toAddress, 25000).Result.ToJson());

            Console.ReadKey();
        }
    }
}