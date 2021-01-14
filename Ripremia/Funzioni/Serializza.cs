using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Xml.Serialization;
public static class Serializza {
    public static T XmlDeserialize<T>(this string toDeserialize) {
        XmlSerializer xmlSerialize = new XmlSerializer(typeof(T), "");
        using (StringReader textReader = new StringReader(toDeserialize)) {
            return (T)xmlSerialize.Deserialize(textReader);
        }
    }
    public static string XmlSerialize<T>(this T toSerialize) {
        XmlSerializer xmlSerialize = new XmlSerializer(typeof(T));
        using (StringWriter textWriter = new StringWriter()) {
            xmlSerialize.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }
}
