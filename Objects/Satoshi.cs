#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    [JsonConverter(typeof(SatoshiConverter))]
    public class Satoshi {
        public decimal Btc {
            get { return Value / 100000000; }
            set { ValueLong = (long) (value * 100000000); }
        }

        public decimal mBtc {
            get { return ValueLong / 100000M; }
            set { ValueLong = (long) (value * 100000); }
        }

        public decimal uBtc {
            get { return ValueLong / 100M; }
            set { ValueLong = (long) (value * 100); }
        }

        public decimal Value {
            get { return ValueLong; }
            set { ValueLong = (long) value; }
        }

        public long ValueLong { get; set; }

        public Satoshi() {
            Value = 0;
        }

        public Satoshi(int val) {
            Value = val;
        }

        public Satoshi(long val) {
            Value = val;
        }

        public static implicit operator Satoshi(int value) {
            return new Satoshi(value);
        }

        public static implicit operator Satoshi(long value) {
            return new Satoshi(value);
        }
    }
}
