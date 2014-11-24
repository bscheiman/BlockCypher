#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    [JsonConverter(typeof (SatoshiConverter))]
    public class Satoshi {
        private long _value;

        public decimal Btc {
            get { return Value / 100000000; }
            set { _value = (long) (value * 100000000); }
        }

        public decimal Value {
            get { return _value; }
            set { _value = (long) value; }
        }

        public decimal mBtc {
            get { return _value / 100000; }
            set { _value = (long) (value * 100000); }
        }

        public decimal uBtc {
            get { return _value / 100; }
            set { _value = (long) (value * 100); }
        }

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