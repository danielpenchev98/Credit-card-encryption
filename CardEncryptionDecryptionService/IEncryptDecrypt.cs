using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CardEncryptionDecryptionService
{
    [ServiceContract]
    public interface IEncryptDecrypt
    {
        [OperationContract]
        string Encrypt(string sessionId, string cardNumber);

        [OperationContract]
        string Decrypt(string sessionId, string encrNumber);

        [OperationContract]
        bool CreateTextFileWithEncryptedNumbersAndTheirCardNumbers(string sessionId,string filename);

        [OperationContract]
        bool CreateTextFileWithCardNumbersAndTheirEncryptedNumbers(string sessionId,string filename);
    }
}
