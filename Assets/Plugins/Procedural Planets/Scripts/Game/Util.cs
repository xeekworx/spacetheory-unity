using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Security;

namespace ProceduralPlanets
{
    /// <summary>
    /// Utility class used to reduce repetitive code.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    public static class Util
    {

        /// <summary>
        /// Converts a string to Base64 (can be used as string method extension, e.g. myString.ToBase64()
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static string ToBase64(this string _str)
        {
            byte[] _bytes = Encoding.UTF8.GetBytes(_str);
            string _encoded = Convert.ToBase64String(_bytes);
            return _encoded;
        }

        /// <summary>
        /// Converts a Base64 formatted string to plain text (can be used as a string method extension, e.g. myString.FromBase64)
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static string FromBase64(this string _str)
        {
            byte[] _bytes = Convert.FromBase64String(_str);
            string _decoded = Encoding.UTF8.GetString(_bytes);
            return _decoded;
        }

        /// <summary>
        /// Verifies if a string *appears* (not full validation) to be Base64 formatted (can be used as a string method extension, e.g. myString.IsBase64()
        /// </summary>
        /// <param name="_base64String"></param>
        /// <returns></returns>
        public static bool IsBase64(this string _base64String)
        {
            if (_base64String == null || 
                _base64String.Length == 0 || 
                _base64String.Length % 4 != 0 || 
                _base64String.Contains(" ") || 
                _base64String.Contains("\t") || 
                _base64String.Contains("\r") || 
                _base64String.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(_base64String);
                return true;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Escapes a string by replacing backslashes and special characters with copy/paste safe string (can be used as a string method extension, e.g. myString.EscapeString()
        /// </summary>
        /// <param name="_string"></param>
        /// <returns></returns>
        public static string EscapeString(this string _string)
        {
            return SecurityElement.Escape(_string);
        }

        /// <summary>
        /// Calculates the Max in Mask value (masks use power of two)
        /// </summary>
        /// <param name="_mask"></param>
        /// <returns>Integer of max in mask value</returns>
        public static int MaxInMask(int _mask)
        {
            return (int)(Mathf.Log(_mask, 2)) + 1;
        }

        /// <summary>
        /// Checks if a list has duplicates
        /// Reference: https://www.geekality.net/2010/01/19/how-to-check-for-duplicates/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subjects"></param>
        /// <returns>True/False</returns>

        public static bool HasDuplicates<T>(this IEnumerable<T> subjects)
        {
            return HasDuplicates(subjects, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Checks if a list has duplicates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subjects"></param>
        /// <param name="comparer"></param>
        /// <returns>True/False</returns>        
        public static bool HasDuplicates<T>(this IEnumerable<T> subjects, IEqualityComparer<T> comparer)
        {
            if (subjects == null)
                throw new ArgumentNullException("subjects");

            if (comparer == null)
                throw new ArgumentNullException("comparer");

            var set = new HashSet<T>(comparer);

            foreach (var s in subjects)
                if (!set.Add(s))
                    return true;

            return false;
        }
    }
}
