using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Common.Constants;

namespace Common.Commons
{
    public static class CommonFuncMain
    {
        static public object GetValueObject(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static List<IDictionary<string, object>> JoinList(List<IDictionary<string, object>> source, List<IDictionary<string,
                            object>> dest, string keySource, TYPE_DATA_CAMPARE typeCampare, string keyDest = null)
        {
            keyDest = keyDest == null ? keySource : keyDest;
            List<IDictionary<string, object>> results = new List<IDictionary<string, object>>();

            foreach (var itemSource in source)
            {
                foreach (var itemDest in dest)
                {
                    if (CampareObject(itemDest[keyDest], itemSource[keySource], typeCampare))
                    {
                        results.Add(Merge(itemSource, itemDest));
                    }
                }
            }
            return results;
        }

        public static IDictionary<string, object> Merge(IDictionary<string, object> itemSource, IDictionary<string, object> itemDest)
        {
            IDictionary<string, object> result = new ExpandoObject();

            foreach (var pair in itemSource.Concat(itemDest))
            {
                result[pair.Key] = pair.Value;
            }

            return result;
        }

        public static bool CampareObject(object source, object dest, TYPE_DATA_CAMPARE typeData)
        {
            bool resultCampare = false;

            if (source == null || dest == null) return false;

            string strSource = source.ToString();
            string strDest = dest.ToString();

            switch (typeData)
            {
                case TYPE_DATA_CAMPARE.STRING:
                    resultCampare = strSource == strDest ? true : false;
                    break;
                case TYPE_DATA_CAMPARE.INT:
                    resultCampare = int.Parse(strSource) == int.Parse(strDest) ? true : false;
                    break;
                case TYPE_DATA_CAMPARE.FLOAT:
                    resultCampare = float.Parse(strSource) == float.Parse(strDest) ? true : false;
                    break;
                case TYPE_DATA_CAMPARE.BOOL:
                    resultCampare = bool.Parse(strSource) == bool.Parse(strDest) ? true : false;
                    break;
                case TYPE_DATA_CAMPARE.DATE_TIME:
                    resultCampare = DateTime.Parse(strSource).ToLocalTime() == DateTime.Parse(strDest).ToLocalTime() ? true : false;
                    break;
                case TYPE_DATA_CAMPARE.DATE:
                    resultCampare = DateTime.Parse(strSource).ToLocalTime().Date == DateTime.Parse(strDest).ToLocalTime().Date ? true : false;
                    break;

            }
            return resultCampare;
        }

        public static dynamic DictionaryToObject(IDictionary<String, Object> dictionary)
        {
            var expandoObj = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<String, Object>>)expandoObj;

            foreach (var keyValuePair in dictionary)
            {
                expandoObjCollection.Add(keyValuePair);
            }
            dynamic eoDynamic = expandoObj;
            return eoDynamic;
        }

        public static bool IsGuidType(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }

        public static bool IsUnicode(string input)
        {
            return Encoding.ASCII.GetByteCount(input) != Encoding.UTF8.GetByteCount(input);
        }

        public static string utf8Convert3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static T ToObject<T>(this Object fromObject)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(fromObject, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }));
        }

        public static List<T> ToObjectList<T>(this Object fromObject)
        {
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(fromObject, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }));
        }
        ///Summary
        // Validates the object and its properties.
        // The variable isValid will be true if everything is valid
        // The results variable contains the results of the validation
        ///Summary
        public static bool ValidateModel<T>(T objectToValidate, out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>(); // Will contain the results of the validation
            ValidationContext vc = new ValidationContext(objectToValidate);
            bool isValid = Validator.TryValidateObject(objectToValidate, vc, validationResults, true);
            return isValid;
        }
        public static bool ValidateModelProperty<T>(T objectToValidate, PropertyInfo value, out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>(); // Will contain the results of the validation
            ValidationContext vc = new ValidationContext(objectToValidate)
            {
                MemberName = value.Name
            };
            bool isValid = Validator.TryValidateProperty(value.GetValue(objectToValidate), vc, validationResults);
            return isValid;
        }
        public static string RemoveSpecialCharacters(string input)
        {
            // Define the pattern to match special characters
            string pattern = "[^a-zA-Z0-9]";

            // Remove special characters using regular expressions
            string result = Regex.Replace(input, pattern, "");

            return result;
        }
    }
}
