using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorApp
{
    internal class Calculator
    {
        public double Result { get; set; } = 0;

        public enum EnOperations : byte
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public enum EnOperationResultState : byte
        {
            Successful,
            ErrorDivideByZero,
            ErrorFailed
        }

        public void Add(double NumberToAdd)
        {
            Result += NumberToAdd;
        }

        public void Subtract(double NumberToSubtract)
        {
            Result -= NumberToSubtract;
        }

        public void Multiply(double NumberToMultiply)
        {
            Result *= NumberToMultiply;
        }

        public bool Divide(double NumberToDivide)
        {
            if (NumberToDivide == 0) return false;

            Result /= NumberToDivide;
            return true;
        }

        public EnOperationResultState PerformOperation(double Number, EnOperations Operation)
        {
            switch (Operation)
            {
                case EnOperations.Add:
                    Add(Number);
                    break;
                case EnOperations.Subtract:
                    Subtract(Number);
                    break;
                case EnOperations.Multiply:
                    Multiply(Number);
                    break;
                case EnOperations.Divide:
                    return Divide(Number)? EnOperationResultState.Successful : EnOperationResultState.ErrorDivideByZero;
                default:
                    return EnOperationResultState.ErrorFailed;
            }

            return EnOperationResultState.Successful;
        }

        public void Reset()
        {
            Result = 0;
        }

        public static double GetSqrt(double Number)
        {
            return Math.Sqrt(Number);
        }

        public static double GetSquare(double Number)
        {
            return Math.Pow(Number, 2);
        }

    }
}
