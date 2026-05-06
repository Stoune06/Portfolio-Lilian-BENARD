using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Tooling
{

    /// <summary>
    /// <see cref = "JsonInterpretor" ></ see > is a custom < see langword="class"/> that allows the programmer
    /// to easyly Read or Write into json files
    /// </summary>
    [DebuggerStepThrough]
    public class JsonInterpretor
    {
        //public static JsonSerializerOptions DefaultOption { get; private set; } = new JsonSerializerOptions() //TODO ADD Back
        //{
        //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //    WriteIndented = true
        //};

        /// <summary>
        /// This method fetches data from a <c>json by reading</c> them and converting those into a <typeparamref name="T"/>
        /// , and then returning the said <typeparamref name="T"/>
        /// </summary>
        public static T Read<T>(string pPathToJson) => JsonUtility.FromJson<T>(File.ReadAllText(pPathToJson));

        //        /// <summary>
        //        /// Return a <c>List</c> <typeparamref name="TObject"/>  which are contained in <typeparamref name="URoot"/>
        //        /// via a <c>json path</c>
        //        /// <code>
        //        /// <see langword="public"/> <see langword="class"/> <typeparamref name="TObject"/>
        //        /// <br/>{
        //        ///     <see langword="public"/> <see langword="string"/> MyString {get; set;} = null;
        //        ///     <see langword="public"/> <see langword="bool"/> MyBool {get; set;} = false;
        //        /// }<br/> 
        //        /// <br/><see langword="public"/> <see langword="class"/> <typeparamref name="URoot"/>
        //        /// <br/>{ 
        //        /// <br/>    <see langword="public"/> <see langword="Listof"/> <typeparamref name="TObject"/> MyList {get; set;} = null;
        //        /// <br/>}
        //        /// </code>
        //        /// In that case if you enter the <see langword="string"/> "MyList" as second parameter this function will return the
        //        /// said <c>List</c>
        //        /// </summary>
        //        public static List<TObject> ReadListFromRoot<TObject, URoot>(string pPathToJson, string lRootPropertyName)
        //        {
        //            FileAccess lFile = FileAccess.Open(pPathToJson, FileAccess.ModeFlags.Read);
        //            URoot lURoot = JsonSerializer.Deserialize<URoot>(lFile.GetAsText());
        //            lFile.Close();

        //            m_PropertyInfo lProperty = typeof(URoot).GetProperty(lRootPropertyName);
        //            return (List<TObject>)lProperty.GetValue(lURoot);
        //        }

        //#if DEBUG
        //        /// <summary>
        //        /// Please refer to <see cref="ReadListFromRoot"/> for documentation, do not use this function for other purposes than
        //        /// debug. 
        //        /// </summary>
        //        public static List<TObject> ReadListFromUnknownRoot<TObject, URoot>(string pPathToJson)
        //        {
        //            FileAccess lFile = FileAccess.Open(pPathToJson, FileAccess.ModeFlags.Read);
        //            URoot lURoot = JsonSerializer.Deserialize<URoot>(lFile.GetAsText());
        //            lFile.Close();

        //            foreach (m_PropertyInfo item in typeof(URoot).GetProperties())
        //            {
        //                if (item.PropertyType == typeof(List<TObject>))
        //                {
        //                    return (List<TObject>)item.GetValue(lURoot);
        //                }
        //            }

        //            throw new NotImplementedException($"The root class : {typeof(URoot).Name} " +
        //                $"does not Contain any List<{typeof(TObject).Name}>");
        //        }
        //#endif

        //        /// <summary>
        //        /// <c>Re-Write the entire json</c> document by using the values stored in your <typeparamref name="T"/>
        //        /// </summary>
        //        public static void Write<T>(string pPathToJson, T pObjectToWrite)
        //            => Write<T>(pPathToJson, pObjectToWrite, DefaultOption);


        ///// <summary>
        ///// <c>Re-Write the entire json</c> document by using the values stored in your <typeparamref name="T"/>.
        ///// you can insert a <see cref="JsonSerializerOptions"/> for further precision
        ///// </summary>
        //public static void Write<T>(string pPathToJson, T pObjectToWrite, JsonSerializerOptions pOption)
        //{
        //    FileAccess lFile = FileAccess.Open(pPathToJson, FileAccess.ModeFlags.Write);
        //    lFile.StoreString(JsonSerializer.Serialize<T>(pObjectToWrite, pOption));
        //    lFile.Close();
        //}


        ///// <summary>
        ///// <c>Re-Write the entire json</c> document by using the values stored in your <typeparamref name="T"/>.
        ///// you can insert a <see cref="JsonSerializerOptions"/> for further precision
        ///// </summary>
        public static void Write(string pPathToJson, object pObjectToWrite)
            => File.WriteAllText(pPathToJson, JsonUtility.ToJson(pObjectToWrite));

    }
}