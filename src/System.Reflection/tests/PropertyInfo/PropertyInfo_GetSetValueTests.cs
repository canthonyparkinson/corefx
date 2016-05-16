// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Reflection;
using System.Collections.Generic;

#pragma warning disable 0414

namespace System.Reflection.Tests
{
    public class PropertyInfoGetValueTests
    {
        public static void TestGetSet_PropertyA()
        {
            string propertyName = "PropertyA";
            MyCoVariantTest obj = new MyCoVariantTest();
            PropertyInfo pi = GetProperty(typeof(MyCoVariantTest), propertyName);

            Assert.Equal(pi.Name, propertyName);

            var value = pi.GetValue(obj);

            Assert.NotNull(value);

            string[] strs = new string[1];
            strs[0] = "hello";

            //set value
            pi.SetValue(obj, strs);
            value = pi.GetValue(obj);

            string[] strs2 = (string[])value;

            Assert.Equal(strs2[0], strs[0]);
        }


        [Fact]
        public static void TestGetSet_PropertyB()
        {
            string propertyName = "PropertyB";
            MyCoVariantTest obj = new MyCoVariantTest();
            PropertyInfo pi = GetProperty(typeof(MyCoVariantTest), propertyName);

            Assert.Equal(pi.Name, propertyName);

            var value = pi.GetValue(obj);

            Assert.Null(value);

            string[] strs = new string[1];
            strs[0] = "hello";

            //set value
            pi.SetValue(obj, strs);
            value = pi.GetValue(obj);

            string[] strs2 = (string[])value;

            Assert.Equal(strs2[0], strs[0]);
        }

        [Fact]
        public static void TestGetSet_Name()
        {
            string propertyName = "Name";
            InterfacePropertyImpl obj = new InterfacePropertyImpl();
            PropertyInfo pi = GetProperty(typeof(InterfacePropertyImpl), propertyName);

            Assert.Equal(pi.Name, propertyName);

            var value = pi.GetValue(obj, (Object[])null);

            Assert.Null(value);

            //set value
            string strs1 = "hello";

            pi.SetValue(obj, strs1);
            value = pi.GetValue(obj);
            string strs2 = (string)value;
            Assert.Equal(strs2, strs1);
        }

        [Fact]
        public static void TestGet_PropertyC()
        {
            string propertyName = "PropertyC";
            MyCoVariantTest obj = new MyCoVariantTest();
            PropertyInfo pi = GetProperty(typeof(MyCoVariantTest), propertyName);

            Assert.Equal(pi.Name, propertyName);

            var value = pi.GetValue(obj, new Object[] { 1, "2" });

            Assert.Null(value);
        }

        [Fact]
        public static void TestGet_Property2()
        {
            string propertyName = "Property2";
            LaterClass obj = new LaterClass();
            PropertyInfo pi = GetProperty(typeof(LaterClass), propertyName);

            Assert.Equal(pi.Name, propertyName);

            int value = (int)pi.GetValue(obj);

            Assert.Equal(value, 100);
        }

        //Try to set indexer Property
        [Fact]
        public static void TestSet_Item()
        {
            string propertyName = "Item";
            TypeWithProperties obj = new TypeWithProperties();
            PropertyInfo pi = GetProperty(typeof(TypeWithProperties), propertyName);
            string[] h = { "hello" };

            Assert.Equal(pi.Name, propertyName);

            pi.SetValue(obj, "someotherstring", new Object[] { 99, 2, h, "f" });

            Assert.Equal("992f1someotherstring", obj.setValue);

            pi.SetValue(obj, "pw", new Object[] { 99, 2, h, "SOME  string" });

            Assert.Equal("992SOME  string1pw", obj.setValue);
        }

        //
        // Negative Tests for PropertyInfo
        //

        [Theory]
        [InlineData("PropertyC", typeof(MyCoVariantTest), new Object[] { 1, "2", 3 })]
        [InlineData("PropertyC", typeof(MyCoVariantTest), null)]
        public static void TestGet_ThrowsTargetParameterCountException(string propertyName, Type type, Object[]testObj)
        {
            Object obj = Activator.CreateInstance(type);
            PropertyInfo pi = GetProperty(type, propertyName);

            Assert.Equal(pi.Name, propertyName);

            Assert.Throws<TargetParameterCountException>(() =>
            {
                var value = pi.GetValue(obj, testObj);
            });
        }

        [Theory]
        [InlineData("Property1", typeof(LaterClass), null)]
        [InlineData("PropertyC", typeof(MyCoVariantTest), new Object[] { "1", "2" })]
        public static void TestGet_ThrowsArgumentException(string propertyName, Type type, Object[] testObj)
        {
            Object obj = Activator.CreateInstance(type);
            PropertyInfo pi = GetProperty(type, propertyName);

            Assert.Equal(pi.Name, propertyName);

            Assert.Throws<ArgumentException>(() =>
            {
                var value = pi.GetValue(obj, testObj);
            });
        }


        //Verify PropertyInfo.GetValue throws Exception
        [Fact]
        public static void TestGet_ThrowsException()
        {
            string propertyName = "PropertyC";
            Object obj = Activator.CreateInstance(typeof(MyCoVariantTest));
            PropertyInfo pi = GetProperty(typeof(MyCoVariantTest), propertyName);

            Assert.Equal(pi.Name, propertyName);

            // In Win8p instead of TargetException , generic Exception is thrown
            // Refer http://msdn.microsoft.com/en-us/library/b05d59ty.aspx
            Assert.ThrowsAny<Exception>(() => pi.GetValue(null, new Object[] { "1", "2" }));
        }



        [Theory]
        [InlineData("NoSetterProp", 100, null)]
        [InlineData("Item", "pw", new Object[] { 99, 2, "hello", "SOME  string" })]
        [InlineData("Item", 100, new Object[] { 99, 2, new string[] { "hello" }, "SOME  string" })]
        public static void TestSet_ThrowsArgumentException(string propertyName, Object setValue, Object[] index)
        {
            TypeWithProperties obj = new TypeWithProperties();
            PropertyInfo pi = GetProperty(typeof(TypeWithProperties), propertyName);

            Assert.Equal(pi.Name, propertyName);

            Assert.Throws<ArgumentException>(() =>
            {
                pi.SetValue(obj, setValue, index);
            });
        }
        
        //Try to set instance Property with null object 
        [Theory]
        [InlineData("HasSetterProp", null, null)]
        [InlineData("Item", "pw", new Object[] { 99, 2, "SOME string" })]
        public static void TestSetNull_ThrowsException(string propertyName, Object setValue, Object[] index)
        {
            PropertyInfo pi = GetProperty(typeof(TypeWithProperties), propertyName);

            Assert.Equal(pi.Name, propertyName);

            // Generic Exception is thrown in WP8 instead of TargetException
            Assert.ThrowsAny<Exception>(() => pi.SetValue(null, null, null));
        }

        //Try to set instance Property with wrong type
        [Fact]
        public static void TestSet_ThrowsException()
        {
            string propertyName = "HasSetterProp";
            TypeWithProperties obj = new TypeWithProperties();
            PropertyInfo pi = GetProperty(typeof(TypeWithProperties), propertyName);

            Assert.Equal(pi.Name, propertyName);

            // Generic Exception is thrown in WP8 instead of TargetException
            Assert.ThrowsAny<Exception>(() => pi.SetValue(obj, "foo", null));
        }
        

        //Try to set indexer Property , Incorrect number of parameters
        [Fact]
        public static void TestSet_ThrowsTargetParameterCountException()
        {
            string propertyName = "Item";
            TypeWithProperties obj = new TypeWithProperties();
            PropertyInfo pi = GetProperty(typeof(TypeWithProperties), propertyName);


            Assert.Equal(pi.Name, propertyName);

            Assert.Throws<TargetParameterCountException>(() =>
            {
                pi.SetValue(obj, "pw", new Object[] { 99, 2, new string[] { "SOME string" } });  // Incorrect number of parameters
            });
        }
       


        //Gets PropertyInfo object from a Type
        public static PropertyInfo GetProperty(Type t, string property)
        {
            TypeInfo ti = t.GetTypeInfo();
            IEnumerator<PropertyInfo> allproperties = ti.DeclaredProperties.GetEnumerator();
            PropertyInfo pi = null;

            while (allproperties.MoveNext())
            {
                if (allproperties.Current.Name.Equals(property))
                {
                    //found property
                    pi = allproperties.Current;
                    break;
                }
            }
            return pi;
        }
    }


    //Reflection Metadata  

    public class MyCoVariantTest
    {
        public static Object[] objArr = new Object[1];
        public Object[] objArr2;

        public static Object[] PropertyA
        {
            get { return objArr; }
            set { objArr = value; }
        }

        public Object[] PropertyB
        {
            get { return objArr2; }
            set { objArr2 = value; }
        }

        [System.Runtime.CompilerServices.IndexerNameAttribute("PropertyC")]   // will make the property name be MyPropAA instead of default Item
        public Object[] this[int index, string s]
        {
            get { return objArr2; }
            set { objArr2 = value; }
        }
    }

    public class TypeWithProperties
    {
        public int NoSetterProp
        {
            get { return 0; }
        }

        public int HasSetterProp
        {
            set { }
        }

        public string this[int index, int index2, string[] h, string myStr]
        {
            get
            {
                string strHashLength = "null";
                if (h != null)
                {
                    strHashLength = h.Length.ToString();
                }
                return index.ToString() + index2.ToString() + myStr + strHashLength;
            }

            set
            {
                string strHashLength = "null";
                if (h != null)
                {
                    strHashLength = h.Length.ToString();
                }
                setValue = setValue = index.ToString() + index2.ToString() + myStr + strHashLength + value;
            }
        }
        public string setValue = null;
    }

    public interface InterfaceProperty
    {
        string Name
        {
            get;
            set;
        }
    }

    public class InterfacePropertyImpl : InterfaceProperty
    {
        private string _name = null;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }


    public class LaterClass
    {
        public int Property1
        {
            set { }
        }

        public int Property2
        {
            private get { return 100; }
            set { }
        }
    }
}
