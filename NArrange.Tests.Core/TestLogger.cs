using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

using NArrange.Core;

namespace NArrange.Tests.Core
{
	/// <summary>
	/// Test logger
	/// </summary>
	public class TestLogger : ILogger
	{
		#region Fields

		private List<TestLogEvent> _events = new List<TestLogEvent>();
		private bool _writeToConsole = false;

		#endregion Fields

		#region Public Properties

		/// <summary>
		/// Gets the log event history
		/// </summary>
		public ReadOnlyCollection<TestLogEvent> Events
		{
			get
			{
			    return _events.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not messages should be written 
		/// to the console.
		/// </summary>
		public bool WriteToConsole
		{
			get
			{
				return _writeToConsole;
			}
			set
			{
				_writeToConsole = value;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Clears the test log
		/// </summary>
		public void Clear()
		{
			_events.Clear();
		}

		/// <summary>
		/// Determines if the specified message exists in the log
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool HasMessage(LogLevel level, string message)
		{
			bool hasMessage = false;

			foreach (TestLogEvent logEvent in _events)
			{
			    if (logEvent.Level == level && logEvent.Message == message)
			    {
			        hasMessage = true;
			        break;
			    }
			}

			return hasMessage;
		}

		/// <summary>
		/// Determines if a partial matching message exists in the log
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool HasPartialMessage(LogLevel level, string message)
		{
			bool hasMessage = false;

			foreach (TestLogEvent logEvent in _events)
			{
			    if (logEvent.Level == level && logEvent.Message.Contains(message))
			    {
			        hasMessage = true;
			        break;
			    }
			}

			return hasMessage;
		}

		/// <summary>
		/// Logs a message
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void LogMessage(LogLevel level, string message, params object[] args)
		{
			string formatted = string.Format(CultureInfo.InvariantCulture,
			    message, args);

			if (WriteToConsole)
			{
				Console.WriteLine(formatted);
			}

			TestLogEvent logEvent = new TestLogEvent(level, formatted);

			_events.Add(logEvent);
		}

		/// <summary>
		/// Gets the text of all events.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder textBuilder = new StringBuilder();
			foreach (TestLogEvent logEvent in Events)
			{
			    textBuilder.AppendLine(logEvent.ToString());
			}

			return textBuilder.ToString();
		}

		#endregion Public Methods

		#region Other

		/// <summary>
		/// Test log event
		/// </summary>
		public struct TestLogEvent
		{
			#region Fields

			/// <summary>
			/// Log level
			/// </summary>
			public readonly LogLevel Level;

			/// <summary>
			/// Log message
			/// </summary>
			public readonly string Message;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Creates a new test log event
			/// </summary>
			/// <param name="level"></param>
			/// <param name="message"></param>
			public TestLogEvent(LogLevel level, string message)
			{
				Level = level;
				Message = message;
			}

			#endregion Constructors

			#region Public Methods

			/// <summary>
			/// Gets the string representation.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("{0}: {1}", Level, Message);
			}

			#endregion Public Methods
		}

		#endregion Other
	}
}