using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Enumerations;
using MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    public partial class ExpressionFunction
    {
        /// <summary>
        /// This function is used to return a random number within a specified interval.
        /// Function Syntax: RandomNum(start:integer, end:integer)
        /// </summary>
        /// <remarks>
        /// Removed new Random() 
        /// Replaced it by RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();
        /// Because the instance of Random() is created on every call,
        /// two calls within roughly 15 ms get the same clock-based seed and return the same number.
        /// </remarks>
        /// <returns>A random number within a specified interval.</returns>
        private int RandomNum()
        {
            Logger.Instance.WriteMethodEntry(EventIdentifier.ExpressionFunctionRandomNumber, EvaluationModeTemplate, this.mode);

            try
            {
                if (!this.HasParameterCount(2))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberInvalidFunctionParameterCountError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFunctionParameterCountError, this.function, 2, this.parameters.Count));

                Type parameterType = typeof(int);
                object minValue = this.parameters[0];
                if (minValue == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 1));

                if (!this.VerifyType(minValue, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberInvalidFirstFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidFirstFunctionParameterTypeError, this.function, parameterType.Name, minValue.GetType().Name));

                object maxValue = this.parameters[1];
                if (maxValue == null)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberNullFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_NullFunctionParameterError, this.function, 2));

                if (!this.VerifyType(maxValue, parameterType))
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberInvalidSecondFunctionParameterTypeError, new InvalidFunctionFormatException(Messages.ExpressionFunction_InvalidSecondFunctionParameterTypeError, this.function, parameterType.Name, maxValue.GetType().Name));

                if (this.mode == EvaluationMode.Parse)
                    return 0;

                int min, max;
                GetMinMax(minValue, maxValue, out min, out max);

                if (min > max)
                    throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberFunctionParameterError, new InvalidFunctionFormatException(Messages.ExpressionFunction_RandomNumberFunctionParameterError, this.function, min, max));

                int result = NextInt(min, max);

                string arguments = this.FormatArguments();
                Logger.Instance.WriteVerbose(EventIdentifier.ExpressionFunctionRandomNumber, "RandomNum({0}) returned '{1}'.", arguments, result);

                return result;
            }
            finally
            {
                Logger.Instance.WriteMethodExit(EventIdentifier.ExpressionFunctionRandomNumber, EvaluationModeTemplate, this.mode);
            }
        }

        private void GetMinMax(object minValue, object maxValue, out int min, out int max)
        {
            try
            {
                min = Convert.ToInt32(minValue, CultureInfo.InvariantCulture);
            }
            catch (OverflowException e)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberInvalidFirstFunctionParameterValueError,
                   new OverflowNotAllowedException(Messages.ExpressionFunction_RandomNumber_InvalidFunctionParameterFormatError, this.function, this.parameters[0], e.Message));
            }

            try
            {
                max = Convert.ToInt32(maxValue, CultureInfo.InvariantCulture);
            }
            catch (OverflowException e)
            {
                throw Logger.Instance.ReportError(EventIdentifier.ExpressionFunctionRandomNumberInvalidSecondFunctionParameterValueError,
                   new OverflowNotAllowedException(Messages.ExpressionFunction_RandomNumber_InvalidFunctionParameterFormatError, this.function, this.parameters[1], e.Message));
            }
        }

        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        /// <summary>Returns a uniformly distributed int in [minValue, maxValue), like Random.Next.</summary>
        private static int NextInt(int minValue, int maxValue)
        {            
            if (minValue == maxValue) return minValue;

            uint range = (uint)((long)maxValue - minValue);   // long avoids overflow
            uint limit = uint.MaxValue - (uint.MaxValue % range); // rejection sampling, avoids modulo bias
            byte[] buffer = new byte[4];
            uint value;
            do
            {
                Rng.GetBytes(buffer);
                value = BitConverter.ToUInt32(buffer, 0);
            } while (value >= limit);

            return (int)(minValue + (long)(value % range));
        }
    }
}
