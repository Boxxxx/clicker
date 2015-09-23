/*
 * Copyright (c) 2012 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace MiniJSON
{
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = Json.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }

    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    public static class Json
    {
#if UNITY_EDITOR
        private const string UNITY_OBJECT_GUID_PREFIX = "guid##";
#endif
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="fillobject">if you're Deserialize directly into a class, this will fill a class instance object</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>

        public class Option {
            // listObjectKey Used when deserialize a list whose object should contain an id as dictionary.key.
            // listObjectValueKey Used when deserialize a list whose object should contain an value as dictionary.value.
            /// <summary> Example
            /// List
            /// "gift":[
            ///     {
            ///         "key":3,
            ///         "value":4
            ///     },
            /// ]
            /// Dictionary
            /// "gift":{
            ///     "3":4,
            /// }
            /// </summary>
            ///BaseType:{
            ///             "key":1,
            ///             "value":"shit"
            ///         }
            ///         {
            ///             "key":1,
            ///             "value":{
            ///                 "id:1,
            ///                 "lvl:1,
            ///                 "buildingId":1
            ///             }
            ///         }
            /// CustomType:{
            ///             "id":1,
            ///             "lvl":1,
            ///             "buildingId":1
            ///         }
            public bool isListObjectBaseType = false;
            public string listObjectKey = "key";
            public string listObjectValueKey = "value";

            // When true,dictionary or object would be filled with old instance other than
            // create a new one.
            public bool fillInOldObject = false;

            public Option() {
                
            }
            public Option(Option data) {
                listObjectKey = data.listObjectKey;
                listObjectValueKey = data.listObjectValueKey;
                fillInOldObject = data.fillInOldObject;
                isListObjectBaseType = data.isListObjectBaseType;
            }
        }
        public static Option option = new Option();
        static Stack oldOption = new Stack();
        public static void PushOption() {
            oldOption.Push(new Option(option));
        }
        public static void PopOption() {
            option = (Option)oldOption.Pop();
        }

        public static object Deserialize(string json, object fillobject = null)
        {
            // save the string for debug information
            if (json == null)
            {
                return null;
            }

            
            object ret = Parser.Parse(json);

            if (fillobject != null)
            {
                Filler.Fill(ret, ref fillobject);
            }

            return ret;
        }
        public static object Deserialize(string json, ref object fillobject)
        {
            if (json == null)
            {
                return null;
            }

            object ret = Parser.Parse(json);

            if (fillobject != null)
            {
                Filler.Fill(ret, ref fillobject);
            }

            return ret;
        }

        public static object DeserializeFromFile(string path, object fillobject = null)
        {
            string data;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader br = new StreamReader(fs))
                    data = br.ReadToEnd();
            }
            return Deserialize(data, fillobject);
        }

        public static void Deserialize(object data, object fillObject) {
            if (fillObject != null)
                Filler.Fill(data, ref fillObject);
        }

        public static void Deserialize(object data, ref object fillObject) {
            if (fillObject != null)
                Filler.Fill(data, ref fillObject);
        }

        /// <summary>
        /// Deserialze a list whose object should contain an id to a dictionary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fillObject"></param>
        public static void DeserializeListToDict(object data,ref object fillObject) {
            if (fillObject != null)
                Filler.FillDictWithList(data, ref fillObject);
        }
        public static object MakeJsonObject(object data) {
            return Deserialize(Serialize(data));
        }

        sealed class Filler
        {

            private static Filler instance = new Filler();

            public static void Fill(object data, ref object fillobject) {
                instance.FillValue(data, ref fillobject);
            }

            public static void FillDictWithList(object data, ref object fillobject) {
                instance.FillObjectWithList(data, ref fillobject);
            }

            void FillObjectWithList(object data, ref object fillobject) {
                IDictionary asDict = fillobject as IDictionary;
                IList dataList = data as IList;
                if (dataList != null) {
                    Type listType = asDict.GetType();
                    if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                        Type keyType = listType.GetGenericArguments()[0];
                        Type valueType = listType.GetGenericArguments()[1];
                        object customFiller = null;
                        if (Array.IndexOf(valueType.GetInterfaces(), typeof(ICustomJsonDeserialize)) > -1) {
                            customFiller = Activator.CreateInstance(valueType);
                        }
                        if (!option.fillInOldObject)
                            asDict.Clear();
                        foreach (object dataObj in dataList) {
                            try {
                                if (dataObj != null) {
                                    object newObject = null;
                                    IDictionary dataObjDict = dataObj as IDictionary;
                                    if (dataObjDict.Contains(option.listObjectKey)) {
                                        object dataKey = ChangeType(dataObjDict[option.listObjectKey], keyType);
                                        //Is object list?
                                        if (valueType.IsGenericType &&
                                            valueType.GetGenericTypeDefinition() == typeof (List<>)) {
                                            newObject = Activator.CreateInstance(valueType);
                                            FillValue(dataObjDict[option.listObjectValueKey], ref newObject);
                                        }
                                        //Is object dictionary?
                                        else if (valueType.IsGenericType &&
                                                 valueType.GetGenericTypeDefinition() == typeof (Dictionary<,>)) {
                                            newObject = Activator.CreateInstance(valueType);
                                            DeserializeListToDict(dataObjDict[option.listObjectValueKey], ref newObject);
                                        }
                                        //Otherwise
                                        else {
                                            var valueData = option.isListObjectBaseType
                                                ? dataObjDict[option.listObjectValueKey] : dataObjDict;
                                            if (option.fillInOldObject && asDict.Contains(dataKey)) {
                                                newObject = asDict[dataKey];
                                                FillValue(valueData, ref newObject);
                                            }
                                            else {
                                                if (customFiller != null)
                                                    newObject = ((ICustomJsonDeserialize)customFiller).CustomDeserialize(valueData,
                                                        ref valueType);
                                                else {
                                                    if (valueType == typeof (string) || dataObj is string)
                                                        newObject = (string)"";
                                                    else
                                                        newObject = Activator.CreateInstance(valueType);
                                                    FillValue(valueData, ref newObject);
                                                }
                                            }
                                        }
                                        if (valueType.Equals(typeof (object)))
                                            asDict[dataKey] = newObject;
                                        else
                                            asDict[dataKey] = ChangeType(newObject, valueType);
                                    }
                                }
                            }
                            catch (Exception ex) {
                                System.Console.Write(ex.Message);
                            }
                        }
                    }
                }
            }

            void FillValue(object data, ref object fillobject)
            {
                if (fillobject == null)
                    return;

                IList asList;
                IDictionary asDict;

                Type fillType = fillobject.GetType();
                bool deserializeListToDict = false;
                if (Array.IndexOf(fillType.GetInterfaces(), typeof(IDeserializeListToDict)) > -1)
                    deserializeListToDict = true;

                if ((asList = fillobject as IList) != null)
                {
                    FillArray(data, ref fillobject);
                }
                else if ((asDict = fillobject as IDictionary) != null)
                {
                    FillObject(data, ref fillobject);
                }
                else if (fillobject is bool)
                {
                    try
                    {
                        fillobject = Convert.ToBoolean(data);
                    }
                    catch (Exception)
                    {
                        fillobject = false;
                    }
                }
                else if (fillobject is string)
                {
                    fillobject = data.ToString();
                }
                else if (fillobject is float
                    || fillobject is double
                    || fillobject is decimal
                    || fillobject is int
                    || fillobject is long
                    || fillobject is sbyte
                    || fillobject is short
                    || fillobject is uint
                    || fillobject is ulong
                    || fillobject is byte
                    || fillobject is ushort
                    || fillobject is char)
                {
                    try
                    {
                        fillobject = ChangeType(data, fillType);
                    }
                    catch (Exception)
                    {
                        fillobject = ChangeType(0, fillType);
                    }
                }
                else if (fillType.IsEnum)
                {
                    try
                    {
                        fillobject = ChangeType(Enum.Parse(fillType, data.ToString()), fillType);
                    }
                    catch (Exception)
                    {
                        Array valuesArray = Enum.GetValues(fillType);
                        fillobject = valuesArray.GetValue(0);
                    }
                }
                else if (fillobject is DateTime)
                {
                    fillobject = DateTime.ParseExact(data.ToString(), "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                    fillobject = ((DateTime)fillobject).ToUniversalTime();
                }
#if UNITY_EDITOR
                // if filleObject is UnityEngine.Object and data is a guid string
                else if (fillobject is UnityEngine.Object 
                    && data is string 
                    && data.ToString().IndexOf(UNITY_OBJECT_GUID_PREFIX) == 0) 
                {
                    var guid = data.ToString();
                    fillobject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(
                        guid.Substring(UNITY_OBJECT_GUID_PREFIX.Length)));
                }
#endif
                else
                {
                    IDictionary dataDict = data as IDictionary;
                    if (dataDict != null)
                    {
                        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
                        FieldInfo[] fields = fillType.GetFields(flags);
                        foreach (FieldInfo field in fields)
                        {
                            if (dataDict.Contains(field.Name))
                            {
                                Type fieldType = field.FieldType;
                                if (dataDict[field.Name] != null) {
                                    object fieldNewObject = null;
                                    if (option.fillInOldObject) {
                                        fieldNewObject = field.GetValue(fillobject);
                                    }
                                    if(!option.fillInOldObject || fieldNewObject==null) {
                                        if (fieldType == typeof (string))
                                            fieldNewObject = (string)"";
                                        else {
                                            fieldNewObject = Activator.CreateInstance(fieldType);
                                        }
                                    }

                                    //if fieldNewObject has interface "CustomDeserializable"
                                    //use interface's methods to fill object
                                    //else 
                                    if (Array.IndexOf(fieldType.GetInterfaces(), typeof (ICustomJsonDeserialize)) > -1)
                                        fieldNewObject = ((ICustomJsonDeserialize)fieldNewObject).CustomDeserialize(
                                            dataDict[field.Name], ref fieldType, field);
                                    else if (deserializeListToDict) {
                                        ((IDeserializeListToDict)fillobject).DeserializeListToDict(dataDict[field.Name],
                                            ref fieldNewObject, field);
                                    }
                                    else
                                        FillValue(dataDict[field.Name], ref fieldNewObject);
                                    field.SetValue(fillobject, fieldNewObject);
                                }
                            }
                        }
                    }
                    else
                    {
                        // fillObject is raw object & data is base type
                        if (data is float
                    || data is double
                    || data is decimal
                    || data is int
                    || data is long
                    || data is sbyte
                    || data is short
                    || data is uint
                    || data is ulong
                    || data is byte
                    || data is ushort
                    || data is char)
                            fillobject = ChangeType(data, data.GetType());
                        else
                            fillobject = GuessChangeType(data);
                    }
                }
            }

            void FillArray(object data, ref object fillobject)
            {
                IList asList = fillobject as IList;
                IList dataList = data as IList;
                if (dataList != null)
                {
                    Type listType = asList.GetType();
                    //if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
					if (true)
                    {
                        Type itemType = listType.GetGenericArguments()[0];
                        object customFiller = null;
                        if (Array.IndexOf(itemType.GetInterfaces(), typeof(ICustomJsonDeserialize)) > -1) {
                            customFiller = Activator.CreateInstance(itemType);
                        }
                        asList.Clear();
                        foreach (object dataObj in dataList)
                        {
                            if (dataObj != null)
                            {
                                object newObject = null;
                                if (customFiller != null)
                                    newObject = ((ICustomJsonDeserialize)customFiller).CustomDeserialize(dataObj, ref itemType);
                                else {
                                    if (itemType == typeof(string) || dataObj is string)
                                        newObject = (string)"";
                                    else
                                        newObject = Activator.CreateInstance(itemType);
                                    FillValue(dataObj, ref newObject);
                                }
                                if (itemType.Equals(typeof(object)))
                                    asList.Add(newObject);
                                else
                                    asList.Add(ChangeType(newObject, itemType));
                            }
                            else
                            {
                                asList.Add(null);
                            }
                        }
                    }
                }
                else
                    fillobject = null;
            }

            void FillObject(object data, ref object fillobject)
            {
                IDictionary asDict = fillobject as IDictionary;
                IDictionary dataDict = data as IDictionary;
                if (dataDict != null)
                {
                    Type listType = asDict.GetType();
                    if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        Type keyType = listType.GetGenericArguments()[0];
                        Type valueType = listType.GetGenericArguments()[1];
                        object customFiller = null;
                        if (Array.IndexOf(valueType.GetInterfaces(), typeof(ICustomJsonDeserialize)) > -1) {
                            customFiller = Activator.CreateInstance(valueType);
                        }
                        if (!option.fillInOldObject)
                            asDict.Clear();
                        foreach (object dataKey in dataDict.Keys)
                        {
                            try
                            {
                                if (dataDict[dataKey] != null)
                                {
                                    object newObject = null;
                                    if (option.fillInOldObject && asDict.Contains(ChangeType(dataKey, keyType))) {
                                        newObject = asDict[ChangeType(dataKey, keyType)];
                                        FillValue(dataDict[dataKey], ref newObject);
                                    }
                                    else {
                                        if (customFiller != null)
                                            newObject = ((ICustomJsonDeserialize)customFiller).CustomDeserialize(dataDict[dataKey], ref valueType);
                                        else {
                                            if (valueType == typeof(string) || dataDict[dataKey] is string)
                                                newObject = (string)"";
                                            else
                                                newObject = Activator.CreateInstance(valueType);
                                            FillValue(dataDict[dataKey], ref newObject);
                                        }
                                    }
                                    if (valueType.Equals(typeof(object)))
                                        asDict[ChangeType(dataKey, keyType)] = newObject;
                                    else
                                        asDict[ChangeType(dataKey, keyType)] = ChangeType(newObject, valueType);
                                }
                                else {
                                    asDict[ChangeType(dataKey, keyType)] = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Console.Write(ex.Message);
                            }
                        }
                    }
                }
                else
                    fillobject = null;
            }

            object ChangeType<T>(T data, Type type)
            {
                if (type.IsEnum)
                    // if type is enum, use Enum::Parse
                    return Enum.Parse(type, data.ToString());
                else
                    // otherwise, use common convert
                    return Convert.ChangeType(data, type);
            }

            object GuessChangeType(object data)
            {
                object ret = null;
                ret = Convert.ChangeType(data, typeof(int));
                if (ret != null)
                    return ret;
                ret = Convert.ChangeType(data, typeof(float));
                if (ret != null)
                    return ret;
                ret = Convert.ChangeType(data, typeof(decimal));
                if (ret != null)
                    return ret;
                ret = Convert.ChangeType(data, typeof(double));
                if (ret != null)
                    return ret;
                ret = Convert.ChangeType(data, typeof(bool));
                if (ret != null)
                    return ret;
                ret = Convert.ChangeType(data, typeof(string));
                
                return ret;
            }
        }

        sealed class Parser : IDisposable
        {
            const string WHITE_SPACE = " \t\n\r";
            const string WORD_BREAK = " \t\n\r{}[],:\"";

            enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            };

            StringReader json;

            Parser(string jsonString)
            {
                json = new StringReader(jsonString);
            }

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString))
                {
                    return instance.ParseValue();
                }
            }

            public void Dispose()
            {
                json.Dispose();
                json = null;
            }

            Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> table = new Dictionary<string, object>();

                // ditch opening brace
                json.Read();

                // {
                while (true)
                {
                    switch (NextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            // name
                            string name = ParseString();
                            if (name == null)
                            {
                                return null;
                            }

                            // :
                            if (NextToken != TOKEN.COLON)
                            {
                                return null;
                            }
                            // ditch the colon
                            json.Read();

                            // value
                            table[name] = ParseValue();
                            break;
                    }
                }
            }

            List<object> ParseArray()
            {
                List<object> array = new List<object>();

                // ditch opening bracket
                json.Read();

                // [
                var parsing = true;
                while (parsing)
                {
                    TOKEN nextToken = NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.SQUARED_CLOSE:
                            parsing = false;
                            break;
                        default:
                            object value = ParseByToken(nextToken);

                            array.Add(value);
                            break;
                    }
                }

                return array;
            }

            object ParseValue()
            {
                TOKEN nextToken = NextToken;
                return ParseByToken(nextToken);
            }

            object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.STRING:
                        return ParseString();
                    case TOKEN.NUMBER:
                        return ParseNumber();
                    case TOKEN.CURLY_OPEN:
                        return ParseObject();
                    case TOKEN.SQUARED_OPEN:
                        return ParseArray();
                    case TOKEN.TRUE:
                        return true;
                    case TOKEN.FALSE:
                        return false;
                    case TOKEN.NULL:
                        return null;
                    default:
                        return null;
                }
            }

            string ParseString()
            {
                StringBuilder s = new StringBuilder();
                char c;

                // ditch opening quote
                json.Read();

                bool parsing = true;
                while (parsing)
                {

                    if (json.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new StringBuilder();

                                    for (int i = 0; i < 4; i++)
                                    {
                                        hex.Append(NextChar);
                                    }

                                    s.Append((char)Convert.ToInt32(hex.ToString(), 16));
                                    break;
                            }
                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            object ParseNumber()
            {
                string number = NextWord;

                if (number.IndexOf('.') == -1)
                {
                    long parsedInt;
                    Int64.TryParse(number, out parsedInt);
                    return parsedInt;
                }

                double parsedDouble;
                Double.TryParse(number, out parsedDouble);
                return parsedDouble;
            }

            void EatWhitespace()
            {
                while (WHITE_SPACE.IndexOf(PeekChar) != -1)
                {
                    json.Read();

                    if (json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            void EatComment()
            {
                bool hitStar = false;
                while (json.Peek() != -1)
                {
                    switch (NextChar)
                    {
                        case '*':
                            hitStar = true;
                            break;
                        case '/':
                            if (hitStar)
                                return;
                            break;
                        default:
                            hitStar = false;
                            break;
                    }
                }
            }

            void EatLine()
            {
                json.ReadLine();
            }

            char PeekChar
            {
                get
                {
                    return Convert.ToChar(json.Peek());
                }
            }

            char NextChar
            {
                get
                {
                    return Convert.ToChar(json.Read());
                }
            }

            string NextWord
            {
                get
                {
                    StringBuilder word = new StringBuilder();

                    while (WORD_BREAK.IndexOf(PeekChar) == -1)
                    {
                        word.Append(NextChar);

                        if (json.Peek() == -1)
                        {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            TOKEN NextToken
            {
                get
                {
                    EatWhitespace();

                    if (json.Peek() == -1)
                    {
                        return TOKEN.NONE;
                    }

                    string word = "";
                    char c = PeekChar;
                    switch (c)
                    {
                        case '{':
                            return TOKEN.CURLY_OPEN;
                        case '}':
                            json.Read();
                            return TOKEN.CURLY_CLOSE;
                        case '[':
                            return TOKEN.SQUARED_OPEN;
                        case ']':
                            json.Read();
                            return TOKEN.SQUARED_CLOSE;
                        case ',':
                            json.Read();
                            return TOKEN.COMMA;
                        case '"':
                            return TOKEN.STRING;
                        case ':':
                            return TOKEN.COLON;
                        case '/':
                            // Addition : check comment and ignore it
                            word += NextChar;
                            if (PeekChar == '*')
                                EatComment();
                            else if (PeekChar == '/')
                                EatLine();
                            return NextToken;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }

                    word += NextWord;

                    switch (word)
                    {
                        case "false":
                            return TOKEN.FALSE;
                        case "true":
                            return TOKEN.TRUE;
                        case "null":
                            return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) or a class into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        /// <summary>
        ///     Since function "Serialize"  would create a new instance,object 
        /// serialized before would not be saved,especially in a customFillValue.
        /// So use function "SerializeAppend" to append string with the current  
        /// instance.
        /// </summary>
        /// <param name="obj"></param>
        public static void SerializeAppend(object obj) {
            Serializer.SerializeAppend(obj);
        }

        /// <summary>
        /// Serialize a dictionary to a list,whose object should contains an id.
        /// This function should be combined with CustomSerializeValue when using.
        /// </summary>
        /// <param name="obj"></param>
        public static void SerializeDictToList(object obj) {
            Serializer.SerializeDictToList(obj);
        }

        public static List<object> MakeListFromDict(object obj) {
            return Serializer.MakeListFromDict(obj);
        }

        public static string SerializeToFile(object obj, string filename)
        {
            string data = Serialize(obj);
            using (FileStream fs = new FileStream(filename, FileMode.Create))
                using (StreamWriter bw = new StreamWriter(fs))
                    bw.Write(data);
            return data;
        }

        sealed class Serializer
        {

            private static Serializer instance=new Serializer();

            StringBuilder builder;

            List<object> serializedObject;

            Serializer()
            {
                serializedObject = new List<object>();
                builder = new StringBuilder();
            }

            public static string Serialize(object obj)
            {
                instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            public static void SerializeAppend(object obj) {
                if (obj != null)
                    instance.SerializeValue(obj);
            }

            public static void SerializeDictToList(object obj) {
                if (obj != null)
                    instance.SerializeObjectToList(obj);
            }

            public static List<object> MakeListFromDict(object obj) {
                if (obj != null)
                    return instance.MakeListFromObject(obj);
                return null;
            }

            void SerializeObjectToList(object value) {
                IDictionary asDict;
                if ((asDict = value as IDictionary) != null) {
                    var list = new List<object>();
                    if (option.isListObjectBaseType) {
                        list = MakeListFromObject(value);
                    }
                    else {
                        foreach (object key in asDict.Keys)
                            list.Add(asDict[key]);
                    }
                    SerializeValue(list);
                }
            }

            List<object> MakeListFromObject(object value) {
                var list = new List<object>();
                IDictionary asDict;
                if ((asDict = value as IDictionary) != null) {
                    var dictType = asDict.GetType();
                    if (dictType.IsGenericType && dictType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                        var valueType = dictType.GetGenericArguments()[1];
                        foreach (object key in asDict.Keys) {
                            var obj = new Dictionary<string, object>();
                            obj[option.listObjectKey] = key;
                            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof (Dictionary<,>))
                                obj[option.listObjectValueKey] = MakeListFromObject(asDict[key]);
                            else
                                obj[option.listObjectValueKey] = asDict[key];
                            list.Add(obj);
                        }
                    }
                }
                return list;
            }

            void SerializeValue(object value)
            {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null)
                {
                    builder.Append("null");
                }
                else if ((asStr = value as string) != null)
                {
                    SerializeString(asStr);
                }
                else if (value is bool)
                {
                    builder.Append(value.ToString().ToLower());
                }
                else if ((asList = value as IList) != null)
                {
                    SerializeArray(asList);
                }
                else if ((asDict = value as IDictionary) != null)
                {
                    SerializeObject(asDict);
                }
                else if (value is char)
                {
                    SerializeString(value.ToString());
                }
                else if (value is DateTime)
                {
                    SerializeString(((DateTime)value).ToUniversalTime().ToString("s"));
                }
#if UNITY_EDITOR
                else if (value is UnityEngine.Object) {
                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as UnityEngine.Object));
                    if (string.IsNullOrEmpty(guid)) {
                        SerializeOther(value);
                    }
                    else {
                        SerializeString(UNITY_OBJECT_GUID_PREFIX + guid);
                    }
                }
#endif
                else
                {
                    SerializeOther(value);
                }
            }

            void SerializeObject(IDictionary obj)
            {
                bool first = true;

                builder.Append('{');

                foreach (object e in obj.Keys)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeString(e.ToString());
                    builder.Append(':');

                    SerializeValue(obj[e]);

                    first = false;
                }

                builder.Append('}');
            }

            void SerializeArray(IList anArray)
            {
                builder.Append('[');

                bool first = true;

                foreach (object obj in anArray)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeValue(obj);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string str)
            {
                builder.Append('\"');

                char[] charArray = str.ToCharArray();
                foreach (var c in charArray)
                {
                    switch (c)
                    {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126))
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                            }
                            break;
                    }
                }

                builder.Append('\"');
            }

            void SerializeClass(object value)
            {
                if (serializedObject.Contains(value))
                {
                    //warning : serialize object loops
                    builder.Append("null");
                    return;
                }

                Type classType = value.GetType();
                if (classType.IsEnum)
                {
                    SerializeString(value.ToString());
                }
                else
                {
                    serializedObject.Add(value);

                    bool first = true;
                    builder.Append('{');

                    BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

                    FieldInfo[] fields = classType.GetFields(flags);

                    bool serializeDictToList = false;
                    if (Array.IndexOf(classType.GetInterfaces(), typeof(ISerializeDictToList)) > -1)
                        serializeDictToList = true;

                    foreach (FieldInfo field in fields)
                    {
                        if (!first)
                            builder.Append(',');
                        SerializeString(field.Name);
                        builder.Append(':');

                        if (Array.IndexOf(field.FieldType.GetInterfaces(),typeof(ICustomJsonSerialize))>-1)
                            ((ICustomJsonSerialize)value).CustomSerialize(classType.GetField(field.Name, flags).GetValue(value), field);
                        else if (serializeDictToList)
                            ((ISerializeDictToList)value).SerializeDictToList(classType.GetField(field.Name, flags).GetValue(value), field);
                        else
                            SerializeValue(classType.GetField(field.Name, flags).GetValue(value));
                        first = false;
                    }
                    builder.Append('}');
                    serializedObject.Remove(value);
                }
            }

            void SerializeOther(object value)
            {
                if (value is float
                    || value is int
                    || value is uint
                    || value is long
                    || value is double
                    || value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is ulong
                    || value is decimal)
                {
                    builder.Append(value.ToString());
                }
                else
                {
                    //SerializeString(value.ToString());
                    SerializeClass(value);
                }
            }
        }
    }

    /// <summary>
    /// Serialize an object using custom serialize.
    /// Priority higher than ISerializeDictToList.
    /// </summary>
    public interface ICustomJsonSerialize {
        void CustomSerialize(object value, params object[] paras);
    }

    /// <summary>
    /// Deserialize an json object to instance using custom deserialize.
    /// Priority higher than ICustomJsonDeserialize.
    /// </summary>
    public interface ICustomJsonDeserialize {
        object CustomDeserialize(object data, ref Type dataType, params object[] paras);
    }

    /// <summary>
    /// Serialize a dictionary instance to a list json str.
    /// Make sure dicionary's value has proper key for deserilizing list back to dictionary.
    /// </summary>
    public interface ISerializeDictToList {
        void SerializeDictToList(object value, params object[] paras);
    }

    /// <summary>
    /// Deserialize a list json str to a dictionary instance.
    /// Make sure list's object has proper key.
    /// </summary>
    public interface IDeserializeListToDict {
        void DeserializeListToDict(object data, ref object fillObject, params object[] paras);
    }
}