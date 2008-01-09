using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		
		#endregion Fields
		
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
			string formatted = string.Format(message, args);
			TestLogEvent logEvent = new TestLogEvent(level, formatted);
			
			_events.Add(logEvent);
		}		
		
		#endregion Public Methods
		
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
		
		#endregion Public Properties
		
		#region Other
		
		/// <summary>
		/// Test log event
		/// </summary>
		public struct TestLogEvent		
		{
			/// <summary>
			/// Log message
			/// </summary>
			public readonly string Message;			
			
			/// <summary>
			/// Log level
			/// </summary>
			public readonly LogLevel Level;			
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
		}		
		
		#endregion Other

	}
}