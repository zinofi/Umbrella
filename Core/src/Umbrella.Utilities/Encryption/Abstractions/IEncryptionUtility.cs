using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Encryption.Abstractions
{
	public interface IEncryptionUtility : IDisposable // TODO: This should not implement IDisposable, only the concrete class should!
	{
		// TODO: v3 - Remove this method as these will be provided by an Options class using DI
        void Initialize(string encryptionKey, string initializationVector);
        string DecryptString(string value);
		string EncryptString(string value);
	}
}