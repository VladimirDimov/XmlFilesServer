using ApiTests.Extensions;
using Microsoft.AspNetCore.Http;

namespace ApiTests.Helpers
{
    public class FormFileHelper
    {
        public FormFile FromXmlString(string xmlString, string filedName, string fileName)
        {
            return new FormFile(xmlString.ToStream(), 0, xmlString.ToBytes().Length, filedName, fileName);
        }
    }
}
