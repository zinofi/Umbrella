namespace Umbrella.Extensions.Logging.Azure
{
	/// <summary>
	/// Specifies information about the callsite of a log event.
	/// </summary>
	public class LocationInformation
    {
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		public string ClassName { get; }

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public string FileName { get; }

		/// <summary>
		/// Gets the line number.
		/// </summary>
		public string LineNumber { get; }

		/// <summary>
		/// Gets the name of the method.
		/// </summary>
		public string MethodName { get; }

		/// <summary>
		/// Gets the full information.
		/// </summary>
		public string FullInfo { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LocationInformation"/> class.
		/// </summary>
		/// <param name="className">Name of the class.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="lineNumber">The line number.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="fullInfo">The full information.</param>
		public LocationInformation(string className, string fileName, string lineNumber, string methodName, string fullInfo)
        {
            ClassName = className;
            FileName = fileName;
            LineNumber = lineNumber;
            MethodName = methodName;
            FullInfo = fullInfo;
        }
    }
}