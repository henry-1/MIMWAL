using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Reflection;


namespace WorkflowActivityLibrary.Tests.Activities
{
    internal static class ActivityTestHelper
    {

        public static void SetValueField<T>(
            object instance,
            string fieldName,
            T value)
            where T : struct
        {
            FieldInfo field =
                instance.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.IsNotNull(
                field,
                "Field '{0}' was not found.",
                fieldName);

            field.SetValue(
                instance,
                value);
        }


        public static T GetValueField<T>(
            object instance,
            string fieldName)
            where T : struct
        {
            FieldInfo field = instance.GetType().GetField(
                fieldName,
                BindingFlags.Instance |
                BindingFlags.NonPublic);

            Assert.IsNotNull(
                field,
                "Field '{0}' was not found.",
                fieldName);

            return (T)field.GetValue(instance);
        }


        public static object InvokeStaticPrivateMethod(
            Type type,
            string methodName,
            params object[] parameters)
        {
            MethodInfo method = type.GetMethod(
                methodName,
                BindingFlags.NonPublic |
                BindingFlags.Static);

            Assert.IsNotNull(
                method,
                "Private static method '{0}' was not found.",
                methodName);

            try
            {
                return method.Invoke(
                    null,
                    parameters);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;

                throw;
            }
        }

        public static object InvokePrivateMethod(
            object instance,
            string methodName,
            params object[] parameters)
        {
            MethodInfo method = instance.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(
                method,
                "Method '" + methodName + "' was not found.");

            return method.Invoke(instance, parameters);
        }

        public static T GetField<T>(
            object instance,
            string fieldName)
            where T : class
        {
            FieldInfo field = instance.GetType().GetField(
                fieldName,
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public);

            Assert.IsNotNull(
                field,
                "Field '" + fieldName + "' was not found.");

            T value = field.GetValue(instance) as T;

            Assert.IsNotNull(
                value,
                "Field '" + fieldName + "' was null or not of the expected type.");

            return value;
        }

        public static void SetField<T>(
            object instance,
            string fieldName,
            T value)
        {
            FieldInfo field = instance.GetType().GetField(
                fieldName,
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public);

            Assert.IsNotNull(
                field,
                "Field '" + fieldName + "' was not found.");

            field.SetValue(
                instance,
                value);
        }

        public static Hashtable CreateEmptyDefinitionsTable()
        {
            Hashtable table = new Hashtable();
            table["Count"] = 0;

            return table;
        }

        public static object InvokePrivateStaticMethod(
            Type type,
            string methodName,
            params object[] parameters)
        {
            MethodInfo method = type.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.IsNotNull(
                method,
                "Method '" + methodName + "' was not found.");

            return method.Invoke(
                null,
                parameters);
        }
    }
}
