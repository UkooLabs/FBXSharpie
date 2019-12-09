using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace UkooLabs.FbxSharpie
{
	public class FbxValue
    {
        public object AsObject { get; }

		public FbxValue(object value)
		{
			AsObject = value;
		}

		public char GetAsChar()
		{
			if (AsObject is char charValue)
			{
				return charValue;
			}
			throw new InvalidCastException();
		}

	


		private int[] ToIntArray(object value)
		{
			if (value is double[] doubleArray)
			{
				return (from item in doubleArray select ToInt(item)).ToArray();
			}
			if (value is float[] floatArray)
			{
				return (from item in floatArray select ToInt(item)).ToArray();
			}
			if (value is byte[] byteArray)
			{
				return (from item in byteArray select ToInt(item)).ToArray();
			}
			if (value is long[] longArray)
			{
				return (from item in longArray select ToInt(item)).ToArray();
			}
			if (value is int[] intArray)
			{
				return (from item in intArray select ToInt(item)).ToArray();
			}
			throw new InvalidCastException();
		}

		private double ToDouble(object value)
        {
            if (value is double doubleValue)
            {
                return doubleValue;
            }
            if (value is float floatValue)
            {
                return floatValue;
            }
            if (value is byte byteValue)
            {
                return byteValue;
            }
            if (value is long longValue)
            {
                return longValue;
            }
            if (value is int intValue)
            {
                return intValue;
            }
            throw new InvalidCastException();
        }

        private double[] ToDoubleArray(object value)
        {
            if (value is double[] doubleArray)
            {
                return doubleArray;
            }
            if (value is float[] floatArray)
            {
                return (from item in floatArray select ToDouble(item)).ToArray();
            }
            if (value is byte[] byteArray)
            {
                return (from item in byteArray select ToDouble(item)).ToArray();
            }
            if (value is long[] longArray)
            {
                return (from item in longArray select ToDouble(item)).ToArray();
            }
            if (value is int[] intArray)
            {
                return (from item in intArray select ToDouble(item)).ToArray();
            }
            throw new InvalidCastException();
        }



		private int ToInt(object value)
		{
			if (value is double doubleValue)
			{
				return (int)doubleValue;
			}
			if (value is float floatValue)
			{
				return (int)floatValue;
			}
			if (value is byte byteValue)
			{
				return (int)byteValue;
			}
			if (value is long longValue)
			{
				return (int)longValue;
			}
			if (value is int intValue)
			{
				return intValue;
			}
			throw new InvalidCastException();
		}

		private float ToFloat(object value)
        {
			if (value is double doubleValue)
			{
				return (float)doubleValue;
			}
			if (value is float floatValue)
            {
                return floatValue;
            }
            if (value is byte byteValue)
            {
                return byteValue;
            }
            if (value is long longValue)
            {
                return longValue;
            }
            if (value is int intValue)
            {
                return intValue;
            }
            throw new InvalidCastException();
        }

		private string ToString(object value)
		{
			if (value is double doubleValue)
			{
				return doubleValue.ToString();
			}
			if (value is float floatValue)
			{
				return floatValue.ToString();
			}
			if (value is byte byteValue)
			{
				return byteValue.ToString();
			}
			if (value is long longValue)
			{
				return longValue.ToString();
			}
			if (value is int intValue)
			{
				return intValue.ToString();
			}
			if (value is string stringValue)
			{
				return stringValue;
			}
			throw new InvalidCastException();
		}

		private long ToLong(object value)
		{
			if (value is double doubleValue)
			{
				return (long)doubleValue;
			}
			if (value is float floatValue)
			{
				return (long)floatValue;
			}
			if (value is byte byteValue)
			{
				return (long)byteValue;
			}
			if (value is long longValue)
			{
				return longValue;
			}
			if (value is int intValue)
			{
				return (long)intValue;
			}
			throw new InvalidCastException();
		}

		private float[] ToFloatArray(object value)
		{
			if (value is double[] doubleArray)
			{
				return (from item in doubleArray select ToFloat(item)).ToArray();
			}
			if (value is float[] floatArray)
			{
				return floatArray;
			}
			if (value is byte[] byteArray)
			{
				return (from item in byteArray select ToFloat(item)).ToArray();
			}
			if (value is long[] longArray)
			{
				return (from item in longArray select ToFloat(item)).ToArray();
			}
			if (value is int[] intArray)
			{
				return (from item in intArray select ToFloat(item)).ToArray();
			}
			throw new InvalidCastException();
		}


		public bool IsString => AsObject is string;

		public string AsString => ToString(AsObject);

		public int GetAsInt()
		{
			return ToInt(AsObject);
		}

		public long GetAsLong()
		{
			return ToLong(AsObject);
		}

		public double GetAsDouble()
		{
			return ToDouble(AsObject);
		}

		public double[] GetAsDoubleArray()
		{
			return ToDoubleArray(AsObject);
		}

		public float GetAsFloat()
		{
			return ToFloat(AsObject);
		}

		public int[] GetAsIntArray()
		{
			return ToIntArray(AsObject);
		}

		public float[] GetAsFloatArray()
		{
			return ToFloatArray(AsObject);
		}

		
	}
}
