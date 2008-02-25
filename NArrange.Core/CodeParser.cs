using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using NArrange.Core.CodeElements;

namespace NArrange.Core
{
	/// <summary>
	/// Base code parser implementation.
	/// </summary>
	public abstract class CodeParser : ICodeParser
	{
		#region Constants

		/// <summary>
		/// Empty character
		/// </summary>
		protected const char EmptyChar = '\0';

		#endregion Constants

		#region Static Fields

		/// <summary>
		/// Whitepace characters
		/// </summary>
		protected static readonly char[] WhitespaceChars = { ' ', '\t', '\r', '\n' };

		#endregion Static Fields

		#region Fields

		private char[] _charBuffer = new char[1];		
		private char _currCh = '\0';		
		private int _lineNumber = 1;		
		private int _position = 1;		
		private char _prevCh = '\0';		
		private TextReader _reader;		
		
		#endregion Fields

		#region Protected Properties

		/// <summary>
		/// Gets the most recently read character
		/// </summary>
		protected char CurrentChar
		{
			get
			{
				return _currCh;
			}
		}

		/// <summary>
		/// Returns the next character in the stream, if any
		/// </summary>
		/// <returns>Next character, if none then EmptyChar</returns>
		protected char NextChar
		{
			get
			{
				int data = Reader.Peek();
				if (data > 0)
				{
					char ch = (char)data;
					return ch;
				}
				else
				{
					return EmptyChar;
				}
			}
		}

		/// <summary>
		/// Gets the previously read character (i.e. before CurrentChar)
		/// </summary>
		protected char PreviousChar
		{
			get
			{
				return _prevCh;
			}
		}

		/// <summary>
		/// Gets the text reader
		/// </summary>
		protected TextReader Reader
		{
			get
			{
				return _reader;
			}
		}

		#endregion Protected Properties

		#region Private Methods

		private void Reset()
		{
			_currCh = '\0';
			_prevCh = '\0';
			_lineNumber = 1;
			_position = 1;
		}

		#endregion Private Methods

		#region Protected Methods

		/// <summary>
		/// Applies attributes and comments to a code element
		/// </summary>
		/// <param name="comments"></param>
		/// <param name="attributes"></param>
		/// <param name="codeElement"></param>
		protected static void ApplyCommentsAndAttributes(CodeElement codeElement,
			ReadOnlyCollection<ICommentElement> comments,
			ReadOnlyCollection<AttributeElement> attributes)
		{
			CommentedElement commentedElement = codeElement as CommentedElement;
			if (commentedElement != null)
			{
				//
				// Add any header comments
				//
				foreach (ICommentElement comment in comments)
				{
					commentedElement.AddHeaderComment(comment);
				}
			}
			
			AttributedElement attributedElement = codeElement as AttributedElement;
			if (attributedElement != null)
			{
				foreach (AttributeElement attribute in attributes)
				{
					attributedElement.AddAttribute(attribute);
			
					//
					// Treat attribute comments as header comments
					//
					if (attribute.HeaderComments.Count > 0)
					{
						foreach (ICommentElement comment in attribute.HeaderComments)
						{
							attributedElement.AddHeaderComment(comment);
						}
			
						attribute.ClearHeaderCommentLines();
					}
				}
			}
		}


		/// <summary>
		/// Parses elements from the current point in the stream
		/// </summary>
		/// <returns></returns>
		protected abstract List<ICodeElement> DoParseElements();
		/// <summary>
		/// Eats the specified character
		/// </summary>
		/// <param name="ch">Character to eat</param>
		protected void EatChar(char ch)
		{
			EatWhitespace();
			TryReadChar();
			if (CurrentChar != ch)
			{
				this.OnParseError("Expected " + ch);
			}
		}

		/// <summary>
		/// Reads until the next non-whitespace character is reached.
		/// </summary>
		protected void EatWhitespace()
		{
			EatWhitespace(Whitespace.All);
		}

		/// <summary>
		/// Reads until the next non-whitespace character is reached.
		/// </summary>
		protected void EatWhitespace(Whitespace whitespaceType)
		{
			int data = Reader.Peek();
			while (data > 0)
			{
				char ch = (char)data;
			
				if ((((whitespaceType & Whitespace.Space) == Whitespace.Space) && ch == ' ') ||
					(((whitespaceType & Whitespace.Tab) == Whitespace.Tab) && ch == '\t') ||
					(((whitespaceType & Whitespace.CarriageReturn) == Whitespace.CarriageReturn) && ch == '\r') ||
					(((whitespaceType & Whitespace.LineFeed) == Whitespace.LineFeed) && ch == '\n'))
				{
					TryReadChar();
				}
				else
				{
					return;
				}
			
				data = Reader.Peek();
				if (data <= 0)
				{
					UnexpectedEndOfFile();
				}
			}
		}

		/// <summary>
		/// Determines whether or not the specified character is whitespace.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		protected static bool IsWhitespace(char ch)
		{
			return ch == ' ' || ch == '\t' ||
				ch == '\n' || ch == '\r';
		}

		/// <summary>
		/// Throws a parse error 
		/// </summary>
		/// <param name="message"></param>
		protected void OnParseError(string message)
		{
			throw new ParseException(message, _lineNumber, _position);
		}

		/// <summary>
		/// Reads the current line
		/// </summary>
		/// <returns></returns>
		protected string ReadLine()
		{
			string commentText = Reader.ReadLine();
			_lineNumber++;
			
			return commentText;
		}

		/// <summary>
		/// Tries to read the specified character from the stream.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		protected bool TryReadChar(char ch)
		{
			int data = _reader.Peek();
			char nextCh = (char)data;
			if (nextCh == ch)
			{
				TryReadChar();
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Tries to read any character from the stream
		/// </summary>
		/// <returns></returns>
		protected bool TryReadChar()
		{
			if (_reader.Read(_charBuffer, 0, 1) > 0)
			{
				_prevCh = _currCh;
				_currCh = _charBuffer[0];
			
				if (_prevCh == '\r' && _currCh == '\n')
				{
					_lineNumber++;
					_position = 1;
				}
				else
				{
					_position++;
				}
			
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Throws an unexpected end of file error.
		/// </summary>
		protected void UnexpectedEndOfFile()
		{
			throw new ParseException("Unexpected end of file", _lineNumber, _position);
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Parses a collection of code elements from a stream reader.
		/// </summary>
		/// <param name="reader">Code stream reader</param>
		/// <returns></returns>
		public ReadOnlyCollection<ICodeElement> Parse(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			
			List<ICodeElement> codeElements = new List<ICodeElement>();
			
			Reset();
			_reader = reader;
			
			codeElements = DoParseElements();
			
			return codeElements.AsReadOnly();
		}

		#endregion Public Methods
	}
}