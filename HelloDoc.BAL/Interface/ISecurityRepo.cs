using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface ISecurityRepo
    {
        string Encrypt(string clearText);

        string Decrypt(string cipherText);
    }
}
