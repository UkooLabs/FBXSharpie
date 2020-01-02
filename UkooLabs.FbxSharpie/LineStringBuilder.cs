using System;
using System.Collections.Generic;
using System.Text;

namespace UkooLabs.FbxSharpie
{
	public class LineStringBuilder
	{
		private readonly StringBuilder _stringBuilder;

		private int _currentLineLength;

		public LineStringBuilder()
		{
			_stringBuilder = new StringBuilder();
			_currentLineLength = 0;
		}

		public LineStringBuilder Append(string value)
		{
			for (var i = 0; i < value.Length; i++) 
			{
				var currentChar = value[i];
				_stringBuilder.Append(currentChar);
				_currentLineLength = currentChar == '\n' ? 0 : _currentLineLength + 1;
				if (_currentLineLength >= Settings.MaxLineLength && currentChar == ',')
				{
					_stringBuilder.Append('\n');
					_currentLineLength = 0;
				}
			}
			return this;
		}

		public void Indent(int level)
		{
			for (int i = 0; i < level; i++)
			{
				_stringBuilder.Append("\t");
			}
		}
	
		public override string ToString()
		{
			return _stringBuilder.ToString();
		}
	}
}
