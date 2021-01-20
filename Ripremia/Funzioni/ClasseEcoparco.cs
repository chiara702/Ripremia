using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

public class ClassApiParco { //vers. 1
    const String BaseUrl = "http://www.ecocontrolgsm.cloud/WebService/WebServiceEcoServiceApp.asmx/";
    public Boolean LastError;
    public String LastErrorDescrizione;

    //public List<KeyValuePair<String, Object>> Param = new List<KeyValuePair<String, Object>>();
    public ClassApiParco() {}

    public DataTable EseguiQuery(String Query) {
        try {
            String URLSqlQuery = BaseUrl + "EcoparcoEseguiQuery";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            var ContentString = "query=" + System.Web.HttpUtility.UrlPathEncode(Query) + "&staging=false";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var ContentStream = req.GetRequestStream();
            ContentStream.Write(UTF8Encoding.UTF8.GetBytes(ContentString));
            ContentStream.Close();
            var gr = req.GetResponse();
            var st = gr.GetResponseStream();
            var stRead = new System.IO.StreamReader(st);
            var rit = stRead.ReadToEnd();
            gr.Close();
            var Table = Serializza.XmlDeserialize<DataTable>(rit);
            return Table;
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }

    internal object EseguiQuery(object p) {
        throw new NotImplementedException();
    }

    public DataRow EseguiQueryRow(String Tabella, int Id) {
        try {
            var Query = "Select * From " + Tabella + " Where Id=" + Id.ToString();
            String URLSqlQuery = BaseUrl + "EcoparcoEseguiQuery";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            var ContentString = "query=" + System.Web.HttpUtility.UrlPathEncode(Query) + "&staging=false";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var ContentStream = req.GetRequestStream();
            ContentStream.Write(UTF8Encoding.UTF8.GetBytes(ContentString));
            ContentStream.Close();
            var gr = req.GetResponse();
            var st = gr.GetResponseStream();
            var stRead = new System.IO.StreamReader(st);
            var rit = stRead.ReadToEnd();
            gr.Close();
            var Table = Serializza.XmlDeserialize<DataTable>(rit);
            if (Table.Rows.Count != 1) return null;
            return Table.Rows[0];
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }
    public DataRow EseguiQueryRow(String Tabella, string Where) { //Where: Id=5 and nome='fabio'
        try {
            var Query = "Select * From " + Tabella + " Where " + Where;
            String URLSqlQuery = BaseUrl + "EcoparcoEseguiQuery";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            var ContentString = "query=" + System.Web.HttpUtility.UrlPathEncode(Query) + "&staging=false";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var ContentStream = req.GetRequestStream();
            ContentStream.Write(UTF8Encoding.UTF8.GetBytes(ContentString));
            ContentStream.Close();
            var gr = req.GetResponse();
            var st = gr.GetResponseStream();
            var stRead = new System.IO.StreamReader(st);
            var rit = stRead.ReadToEnd();
            gr.Close();
            var Table = Serializza.XmlDeserialize<DataTable>(rit);
            if (Table.Rows.Count == 0) return null;
            return Table.Rows[0];
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }

    public Object EseguiCommand(String Query) {
        try {
            String URLSqlQuery = BaseUrl + "EcoparcoEseguiCommand";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            var ContentString = "command=" + System.Web.HttpUtility.UrlPathEncode(Query) + "&staging=false";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var ContentStream = req.GetRequestStream();
            ContentStream.Write(UTF8Encoding.UTF8.GetBytes(ContentString));
            ContentStream.Close();
            var gr = req.GetResponse();
            var st = gr.GetResponseStream();
            var stRead = new System.IO.StreamReader(st, UTF8Encoding.UTF8);
            var rit = stRead.ReadToEnd();
            gr.Close();
            var RitObj = Serializza.XmlDeserialize<Object>(rit);
            return RitObj;
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }


    public class Parametri {
        public List<KeyValuePair<String, Object>> Param = new List<KeyValuePair<String, Object>>();
        public Parametri() { }
        public void AddParameterString(string Parametro, string Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }
        public void AddParameterObject(string Parametro, Object Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }

        public void AddParameterInteger(String Parametro, int Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }
        public void AddParameterDecimal(String Parametro, Decimal Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }
        
    }

    public Parametri GetParam() {
        return new Parametri();
    }


    

    private string SqlParameterGenerator(Parametri Param) {
        String Output = "";
        foreach (KeyValuePair<string, object> x in Param.Param) {
            if (x.Value is String) {
                Output += "SET @" + x.Key + "='" + x.Value.ToString().Replace("'","\'") + "';\n";
            }
            if (x.Value is int) {
                Output += "SET @" + x.Key + "=" + x.Value.ToString() + ";\n";
            }
            if (x.Value is byte[]){
                Output += "SET @" + x.Key + "=" + "X'" + ToHex((byte[])x.Value) + "';\n";
            }
            if (x.Value is DateTime) {
                Output += "SET @" + x.Key + "=" + "'" + ((DateTime)x.Value).ToString("yyyy/MM/dd HH:mm:ss") + "';\n";
            }
        }
        return Output;
    }

    public object EseguiInsert(string Tabella, Parametri Parameters) {
        string Sql = "INSERT INTO " + Tabella + " (";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            Sql += x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += ") VALUES(";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            // Sql &= "?,"
            Sql += "@" + x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += ");";
        Sql += "SELECT LAST_INSERT_ID();"; //"SELECT @@IDENTITY;";
        var commandtotale = SqlParameterGenerator(Parameters) + Sql;
        var rit = EseguiCommand(commandtotale);
        return rit;
    }
    public object EseguiUpdate(string Tabella, long Id, Parametri Parameters) {
        string Sql = "UPDATE " + Tabella + " SET ";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            Sql += x.Key + "=@" + x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += " WHERE Id=" + Id;
        var commandtotale = SqlParameterGenerator(Parameters) + Sql + ";SELECT ROW_COUNT();";
        var rit = EseguiCommand(commandtotale);
        return rit;
    }
    public object EseguiUpdateWhere(string Tabella, string WhereUpdate, Parametri Parameters) {
        string Sql = "UPDATE " + Tabella + " SET ";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            Sql += x.Key + "=@" + x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += " WHERE " + WhereUpdate;
        var commandtotale = SqlParameterGenerator(Parameters) + Sql + ";SELECT ROW_COUNT();";
        var rit = EseguiCommand(commandtotale);
        return rit;
    }

    public object EseguiDelete(string Tabella, long Id) {
        string Sql = "DELETE FROM " + Tabella + " Where Id=" + Id.ToString() + ";SELECT ROW_COUNT();";
        return EseguiCommand(Sql);
    }
    public object EseguiDeleteWhere(string Tabella, string Where) {
        string Sql = "DELETE FROM " + Tabella + " Where " + Where + ";SELECT ROW_COUNT();";
        return EseguiCommand(Sql);
    }

    private string ToHex(byte[] bytes){
        char[] c = new char[bytes.Length * 2];
        byte b;
        for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
        {
            b = ((byte)(bytes[bx] >> 4));
            c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            b = ((byte)(bytes[bx] & 0x0F));
            c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
        }
        return new string(c);
    }

}

public class ClassApiEcoControl {
    const String BaseUrl = "http://185.58.119.77/WebService/webserviceEcoParco.asmx/";
    //const String BaseUrl = "https://localhost:44344/WebService/webserviceEcoParco.asmx/";
    public Boolean LastError;
    public String LastErrorDescrizione;
    public class ResponseCreaQrCodeMonetaVirtuale {
        public String QrCode;
        public long IdGenerazione;
        public String ErroreString;
    }
    public ResponseCreaQrCodeMonetaVirtuale CreaQRCodeMonetaVirtuale(String Paese,String IdentificativoBidone, int ImportoInCentesimi, String CodiceFiscale) {
        LastError = false;
        LastErrorDescrizione = "";
        try {
            String URLSqlQuery = BaseUrl + string.Format("CreaQRCodeMonetaVirtuale?paese={0}&IdentificativoBidone={1}&ImportoInCentesimi={2}&CodiceFiscale={3}", System.Web.HttpUtility.UrlEncode(Paese), System.Web.HttpUtility.UrlEncode(IdentificativoBidone), System.Web.HttpUtility.UrlEncode(ImportoInCentesimi.ToString()), System.Web.HttpUtility.UrlEncode(CodiceFiscale));
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            req.Timeout = 8000;
            req.Method = "GET";
            using (var gr = req.GetResponse()) {
                var st = gr.GetResponseStream();
                var stRead = new System.IO.StreamReader(st);
                var rit = stRead.ReadToEnd();
                var Response = Serializza.XmlDeserialize<ResponseCreaQrCodeMonetaVirtuale>(rit);
                return Response;
            }
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }

    

   

    public DataTable EseguiQuery(String Query) {
        LastError = false;
        LastErrorDescrizione = "";
        try {
            String URLSqlQuery = BaseUrl + "EcocontrolEseguiQuery?query=" + System.Web.HttpUtility.UrlPathEncode(Query) + "&staging=false";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            req.Timeout = 8000;
            req.Method = "GET";
            using (var gr = req.GetResponse()) {
                var st = gr.GetResponseStream();
                var stRead = new System.IO.StreamReader(st);
                var rit = stRead.ReadToEnd();
                var Table = Serializza.XmlDeserialize<DataTable>(rit);
                if (Table.Rows.Count == 0) return null;
                return Table;
            }
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        } 
    }
    public DataRow EseguiQueryRow(String Tabella, int Id) {
        LastError = false;
        LastErrorDescrizione = "";
        try {
            String URLSqlQuery = BaseUrl + "EcocontrolEseguiQuery?query=" + System.Web.HttpUtility.UrlPathEncode("Select * From " + Tabella + " Where Id=" + Id.ToString()) + "&staging=false";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            req.Timeout = 8000;
            req.Method = "GET";
            using (var gr = req.GetResponse()) {
                var st = gr.GetResponseStream();
                var stRead = new System.IO.StreamReader(st);
                var rit = stRead.ReadToEnd();
                var Table = Serializza.XmlDeserialize<DataTable>(rit);
                if (Table.Rows.Count == 0) return null;
                return Table.Rows[0];
            }
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }

    public DataRow EseguiQueryRow(String Tabella, string Where) { //Where: Id=5 and nome='fabio'
        LastError = false;
        LastErrorDescrizione = "";
        try {
            String URLSqlQuery = BaseUrl + "EcocontrolEseguiQuery?query=" + System.Web.HttpUtility.UrlPathEncode("Select * From " + Tabella + " Where " + Where) + "&staging=false";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            req.Timeout = 8000;
            req.Method = "GET";
            using (var gr = req.GetResponse()) {
                var st = gr.GetResponseStream();
                var stRead = new System.IO.StreamReader(st);
                var rit = stRead.ReadToEnd();
                var Table = Serializza.XmlDeserialize<DataTable>(rit);
                if (Table.Rows.Count == 0) return null;
                return Table.Rows[0];
            }
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }
    public Object EseguiCommand(String Query) {
        LastError = false;
        LastErrorDescrizione = "";
        try {
            String URLSqlQuery = BaseUrl + "EcocontrolEseguiCommand?Command=" + System.Web.HttpUtility.UrlPathEncode(Query) + "&staging=false";
            var req = (HttpWebRequest)WebRequest.Create(URLSqlQuery);
            req.Timeout = 8000;
            req.Method = "GET";
            using (var gr = req.GetResponse()) {
                var st = gr.GetResponseStream();
                var stRead = new System.IO.StreamReader(st);
                var rit = stRead.ReadToEnd();
                var Obj = Serializza.XmlDeserialize<Object>(rit);
                return Obj;
            }
        } catch (WebException e) {
            if (e.Response == null) {
                LastError = true;
                LastErrorDescrizione = e.Message;
            }
            if (e.Response != null) {
                var stRead = new System.IO.StreamReader(e.Response.GetResponseStream());
                var rit = stRead.ReadToEnd();
                LastError = true;
                LastErrorDescrizione = rit;
            }
            return null;
        }
    }

    public class Parametri {
        public List<KeyValuePair<String, Object>> Param = new List<KeyValuePair<String, Object>>();
        public Parametri() { }
        public void AddParameterString(string Parametro, string Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }

        public void AddParameterInteger(String Parametro, int Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }
        public void AddParameterDecimal(String Parametro, Decimal Valore) {
            Param.Add(new KeyValuePair<string, object>(Parametro, Valore));
        }
    }

    public Parametri GetParam() {
        return new Parametri();
    }




    private string SqlParameterGenerator(Parametri Param) {
        String Output = "";
        foreach (KeyValuePair<string, object> x in Param.Param) {
            if (x.Value is String) {
                Output += "SET @" + x.Key + "='" + x.Value.ToString().Replace("'", "\'") + "';\n\n";
            }
            if (x.Value is int) {
                Output += "SET @" + x.Key + "=" + x.Value.ToString() + ";\n";
            }
        }
        return Output;
    }

    public object EseguiInsert(string Tabella, Parametri Parameters) {
        string Sql = "INSERT INTO " + Tabella + " (";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            Sql += x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += ") VALUES(";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            // Sql &= "?,"
            Sql += "@" + x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += ");";
        Sql += "SELECT @@IDENTITY";
        var commandtotale = SqlParameterGenerator(Parameters) + Sql;
        var rit = EseguiCommand(commandtotale);
        return rit;
    }
    public object EseguiUpdate(string Tabella, long Id, Parametri Parameters) {
        string Sql = "UPDATE " + Tabella + " SET ";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            Sql += x.Key + "=@" + x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += " WHERE Id=" + Id;
        var commandtotale = SqlParameterGenerator(Parameters) + Sql + ";SELECT ROW_COUNT();";
        var rit = EseguiCommand(commandtotale);
        return rit;
    }
    public object EseguiUpdateWhere(string Tabella, string WhereUpdate, Parametri Parameters) {
        string Sql = "UPDATE " + Tabella + " SET ";
        foreach (KeyValuePair<String, Object> x in Parameters.Param)
            Sql += x.Key + "=@" + x.Key + ",";
        Sql = Sql.TrimEnd(',');
        Sql += " WHERE " + WhereUpdate;
        var commandtotale = SqlParameterGenerator(Parameters) + Sql + ";SELECT ROW_COUNT();";
        var rit = EseguiCommand(commandtotale);
        return rit;
    }

    public object EseguiDelete(string Tabella, long Id) {
        string Sql = "DELETE FROM " + Tabella + " Where Id=" + Id.ToString() + ";SELECT ROW_COUNT();";
        return EseguiCommand(Sql);
    }
    public object EseguiDeleteWhere(string Tabella, string Where) {
        string Sql = "DELETE FROM " + Tabella + " Where " + Where + ";SELECT ROW_COUNT();";
        return EseguiCommand(Sql);
    }


}
    

public class Funzioni {

    public static string Antinull(object input) {
        if (input == null) return ""; else return input.ToString();
    }
    public static string AntiAp(String stringa) {
        return stringa.Replace("'", "''");
    }
    public static bool VerificaCodiceFiscale(string Codice) {
        // Dato un Codice Fiscale verifica il codice di controllo
        // Input: il Codice Fiscale da verificare, 16 caratteri
        // Output: true/false
        // 2010.12.05
        if (Codice == null) return false;
        Codice = Codice.ToUpper();
        if (Codice.Length != 16)
            return false; // errore
        else {
            if (Codice.Substring(15, 1) ==
                CalcolaCodiceControllo(Codice.Substring(0, 15)))
                return true;
            else
                return false;
        }
    }
    private static string CalcolaCodiceControllo(string Codice) {
        // Calcola il codice di controllo del Codice Fiscale
        // Input: i primi 15 caratteri del Codice Fiscale
        // Output: il codice di controllo
        // 2010.12.05
        int Contatore = 0;
        Codice = Codice.ToUpper();
        if (Codice.Length != 15)
            return "0"; // zero: errore
        else {
            for (int i = 0; i < Codice.Length; i++) {
                Contatore += ValoreDelCarattere(Codice.Substring(i, 1), i);
            }
            Contatore %= 26; // si considera il resto
            return Convert.ToChar(Contatore + 65).ToString();
        }
    }
    private static int ValoreDelCarattere(string Carattere, int Posizione) {
        int Valore = 0;
        switch (Carattere) {
            case "A":
            case "0":
                if ((Posizione % 2) == 0)
                    Valore = 1;
                else
                    Valore = 0;
                break;
            case "B":
            case "1":
                if ((Posizione % 2) == 0)
                    Valore = 0;
                else
                    Valore = 1;
                break;
            case "C":
            case "2":
                if ((Posizione % 2) == 0)
                    Valore = 5;
                else
                    Valore = 2;
                break;
            case "D":
            case "3":
                if ((Posizione % 2) == 0)
                    Valore = 7;
                else
                    Valore = 3;
                break;
            case "E":
            case "4":
                if ((Posizione % 2) == 0)
                    Valore = 9;
                else
                    Valore = 4;
                break;
            case "F":
            case "5":
                if ((Posizione % 2) == 0)
                    Valore = 13;
                else
                    Valore = 5;
                break;
            case "G":
            case "6":
                if ((Posizione % 2) == 0)
                    Valore = 15;
                else
                    Valore = 6;
                break;
            case "H":
            case "7":
                if ((Posizione % 2) == 0)
                    Valore = 17;
                else
                    Valore = 7;
                break;
            case "I":
            case "8":
                if ((Posizione % 2) == 0)
                    Valore = 19;
                else
                    Valore = 8;
                break;
            case "J":
            case "9":
                if ((Posizione % 2) == 0)
                    Valore = 21;
                else
                    Valore = 9;
                break;
            case "K":
                if ((Posizione % 2) == 0)
                    Valore = 2;
                else
                    Valore = 10;
                break;
            case "L":
                if ((Posizione % 2) == 0)
                    Valore = 4;
                else
                    Valore = 11;
                break;
            case "M":
                if ((Posizione % 2) == 0)
                    Valore = 18;
                else
                    Valore = 12;
                break;
            case "N":
                if ((Posizione % 2) == 0)
                    Valore = 20;
                else
                    Valore = 13;
                break;
            case "O":
                if ((Posizione % 2) == 0)
                    Valore = 11;
                else
                    Valore = 14;
                break;
            case "P":
                if ((Posizione % 2) == 0)
                    Valore = 3;
                else
                    Valore = 15;
                break;
            case "Q":
                if ((Posizione % 2) == 0)
                    Valore = 6;
                else
                    Valore = 16;
                break;
            case "R":
                if ((Posizione % 2) == 0)
                    Valore = 8;
                else
                    Valore = 17;
                break;
            case "S":
                if ((Posizione % 2) == 0)
                    Valore = 12;
                else
                    Valore = 18;
                break;
            case "T":
                if ((Posizione % 2) == 0)
                    Valore = 14;
                else
                    Valore = 19;
                break;
            case "U":
                if ((Posizione % 2) == 0)
                    Valore = 16;
                else
                    Valore = 20;
                break;
            case "V":
                if ((Posizione % 2) == 0)
                    Valore = 10;
                else
                    Valore = 21;
                break;
            case "W":
                if ((Posizione % 2) == 0)
                    Valore = 22;
                else
                    Valore = 22;
                break;
            case "X":
                if ((Posizione % 2) == 0)
                    Valore = 25;
                else
                    Valore = 23;
                break;
            case "Y":
                if ((Posizione % 2) == 0)
                    Valore = 24;
                else
                    Valore = 24;
                break;
            case "Z":
                if ((Posizione % 2) == 0)
                    Valore = 23;
                else
                    Valore = 25;
                break;
            default:
                Valore = 0;
                break;
        }
        return Valore;
    }
    public static bool IsValidEmail(string email) {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        } catch {
            return false;
        }
    }


    public static int VerificaPassword(String Password) {
        //ritorni: 0:OK
        //-1:caratteri ammessi non validi (caratteri validi a-Z,A-Z,0-9,!,@,#,$,%,*,.,-,_) o lunghezza non corretta (Password da 8 a 20 caratteri)
        //-2:la password richiede almeno un carattere maiuscolo
        //-3:la password richiede almeno un carattere minuscolo
        //-4:la password richiede almeno un numero
        //-5:la password richiede almeno un carattere speciale tra: !,@,#,$,%,*,.,-,_
        if (Regex.IsMatch(Password, "^[A-Za-z0-9/!/@/#/$/&/*/%/./-/_/+]{8,20}$") == false) return -1;
        if (Regex.IsMatch(Password, "[A-Z]{1,}") == false) return -2; //controllo della presenza di almeno una lettera maiuscola
        if (Regex.IsMatch(Password, "[a-z]{1,}") == false) return -3; //controllo della presenza di almeno una lettera minuscola
        if (Regex.IsMatch(Password, "[0-9]{1,}") == false) return -4; //controllo della presenza di almeno un numero
        if (Regex.IsMatch(Password, "[/!/@/#/$/&/*/%/./-/_/+]") == false) return -5;
        return 0;
    }
    public static void SendEmail(string Destinatario, string Mittente, string Oggetto, string Messaggio) {
        // imposta destinatario
        if (Destinatario == "")
            return;
        MailAddress sendTo = new MailAddress(Destinatario);
        // imposta mittente
        MailAddress from = new MailAddress(Mittente);
        // istanzia l'oggetto MailMessage
        MailMessage message = new MailMessage(from, sendTo);
        // campi del messaggio
        message.IsBodyHtml = true;
        message.Subject = Oggetto;
        message.Body = Messaggio;
        // credenziali di accesso
        System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential("info@ecocontrolgsm.it", "fabio123456");
        // imposta connessione con il server GMAIL
        SmtpClient SMTPServer = new SmtpClient("smtp.ecocontrolgsm.it");
        SMTPServer.UseDefaultCredentials = false;
        SMTPServer.Port = 587;
        SMTPServer.Credentials = basicAuthenticationInfo;
        // SMTPServer.EnableSsl = True
        // invio della mail
        try {
            SMTPServer.Send(message);
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    public static String ToQueryData(DateTime data) {
        return data.ToString("yyyy/MM/dd HH:mm:ss");
    }
}

