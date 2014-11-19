namespace BlockCypher.Objects {
    public class Satoshi {
        public decimal Btc {
            get { return Value / 100000000; }
        }

        public decimal Value { get; set; }

        public decimal mBtc {
            get { return Value / 100000; }
        }

        public decimal uBtc {
            get { return Value / 100; }
        }
    }
}