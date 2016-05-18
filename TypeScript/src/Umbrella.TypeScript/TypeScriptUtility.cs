using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;

namespace Umbrella.TypeScript
{
    public static class TypeScriptUtility
    {
        #region Public Static Methods
        public static string GenerateTypeName(string memberName, Type memberType, TypeScriptOutputModelType outputModelType)
        {
            string generatedName = memberName;

            if (!memberType.IsInterface && (outputModelType == TypeScriptOutputModelType.Interface
                || outputModelType == TypeScriptOutputModelType.KnockoutInterface
                || outputModelType == TypeScriptOutputModelType.AureliaInterface))
            {
                generatedName = "I" + generatedName;
            }

            if (outputModelType == TypeScriptOutputModelType.KnockoutClass || outputModelType == TypeScriptOutputModelType.KnockoutInterface)
            {
                int idxModelString = generatedName.LastIndexOf("Model");

                generatedName = idxModelString > -1
                    ? generatedName.Insert(idxModelString, "Knockout")
                    : generatedName + "KnockoutModel";
            }

            return generatedName;
        }

        public static TypeScriptMemberInfo GetTypeScriptMemberInfo(Type memberType, string memberName, TypeScriptOutputModelType outputType)
        {
            TypeScriptMemberInfo info = new TypeScriptMemberInfo
            {
                Name = memberName,
                CLRType = memberType
            };

            //The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
            //Not checking for the use of IntPtr and UIntPtr. Assuming they just won't be used!
            if (memberType.IsPrimitive)
            {
                if (memberType == typeof(bool))
                    info.TypeName = "boolean";
                else if (memberType == typeof(char))
                    info.TypeName = "string";
                else
                    info.TypeName = "number";
            }
            else
            {
                //Always deal with DateTime instances as strings and deal with them on the client
                if (memberType == typeof(string) || memberType == typeof(DateTime))
                {
                    info.TypeName = "string";
                }
                else if (memberType == typeof(decimal))
                {
                    info.TypeName = "number";
                }
                else if (memberType.IsNullableType())
                {
                    //Get the underlying primitive type or struct
                    Type underlyingType = Nullable.GetUnderlyingType(memberType);

                    info = GetTypeScriptMemberInfo(underlyingType, memberName, outputType);
                    info.IsNullable = true;
                }
                else if (memberType == typeof(object))
                {
                    info.TypeName = "any";
                }
                else if (memberType.IsArray)
                {
                    //Strip the [] from the name and try and get the type
                    string arrayTypeName = memberType.FullName.Replace("[]", "");

                    Type arrayType = Type.GetType(arrayTypeName);

                    info = GetTypeScriptMemberInfo(arrayType, memberName, outputType);

                    //Set the type name correctly
                    info.TypeName += "[]";
                }
                else if (typeof(IEnumerable).IsAssignableFrom(memberType))
                {
                    if (memberType.IsGenericType)
                    {
                        //Determine the type of the collection
                        Type genericEnumerableType = memberType.GetGenericArguments().First();

                        info = GetTypeScriptMemberInfo(genericEnumerableType, memberName, outputType);

                        //Set the type name correctly
                        info.TypeName += "[]";
                    }
                    else
                    {
                        //Not dealing with a generic collection - just output the type as "any[]";
                        info.TypeName = "any[]";
                    }

                    info.IsNullable = true;
                }
                else if (memberType.IsEnum)
                {
                    //Output the fully qualified name of the Enum in case it resides in another TypeScript namespace
                    info.TypeName = memberType.FullName;
                }
                else
                {
                    //We must be dealing with a class, e.g. a user defined child model

                    //We need to generate the name for this member which will be output on the TypeScript model
                    //Assume that the child class is being generated with the same output type as the parent, e.g. if the parent is a KnockoutClass, assume the child
                    //has a Knockout representation.
                    //TODO: Could provide an additional attribute to apply to a property to override this behaviour
                    //TODO: Could also validate to see if the child model has the representation we need be getting hold of the attribute, or even checking it just exists.

                    string generatedName = GenerateTypeName(memberType.Name, memberType, outputType);

                    //Ensure the name that is output is fully qualified by namespace as it may not necessarily exist in the same one as the parent
                    info.TypeName = memberType.Namespace + "." + generatedName;
                }
            }

            //If the type cannot be determined, default it to "any"
            if (string.IsNullOrEmpty(info.TypeName))
                info.TypeName = "any";

            //Set the initial output value
            if (info.IsNullable || !info.CLRType.IsValueType)
            {
                info.InitialOutputValue = info.TypeName.EndsWith("[]") ? "[]" : "null";
            }
            else
            {
                //object instance = Activator.CreateInstance(info.CLRType);

                info.InitialOutputValue = "null"; //instance.ToString();

                if (info.CLRType == typeof(DateTime))
                {
                    info.InitialOutputValue = "null";
                }
                else if (info.CLRType.IsEnum)
                {
                    string name = Enum.GetValues(info.CLRType).GetValue(0).ToString();

                    //Ensure the full namespace of the type is prefixed to the initial output value
                    info.InitialOutputValue = $"{info.CLRType.FullName}.{name}";
                }
                else if (info.CLRType == typeof(bool))
                {
                    info.InitialOutputValue = info.InitialOutputValue.ToLowerInvariant();
                }
            }

            return info;
        }

        public static List<string> GetInterfaceNames(Type modelType, TypeScriptOutputModelType outputType, bool includeSelf)
        {
            List<string> lstInterfaceName = new List<string>();

            //This interface is the primary interface which corresponds exactly to the modelType
            //Check the modelType isn't an interface first to avoid making the interface extend itself!
            if (includeSelf)
                lstInterfaceName.Add(GenerateTypeName(modelType.Name, modelType, outputType));

            //All other interfaces
            foreach (Type tInterface in modelType.GetInterfaces())
            {
                //Check if the interface has the same namespace as the model type to avoid outputting fully qualified names where possible
                string interfaceName = tInterface.Namespace == modelType.Namespace
                    ? tInterface.Name
                    : tInterface.FullName;

                lstInterfaceName.Add(GenerateTypeName(interfaceName, tInterface, outputType));
            }

            //Ensure we have a distinct list
            return lstInterfaceName.Distinct().ToList();
        }
        #endregion
    }
}