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
            Divide,
            Mod
        }

        public enum EnOperationResultState : byte
        {
            Successful,
            ErrorDivideByZero,
            ErrorOperandNotInteger,
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

        public EnOperationResultState Divide(double NumberToDivide)
        {
            if (NumberToDivide == 0) return EnOperationResultState.ErrorDivideByZero;

            Result /= NumberToDivide;
            return EnOperationResultState.Successful;
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
                    return Divide(Number);
                case EnOperations.Mod:
                    return Mod(Number); 
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

        private static bool IsInteger(double NumberToCheck)
        {
            // in case you wonder how this works
            // then simply think of it as it calcultes the nearest 
            // integer of the number and checks how far it is 
            // in case it is too far this means the number is not integer
            // other wise it will be so close and that means the number
            // is usually a slightly off integer 

            return Math.Abs(NumberToCheck - Math.Round(NumberToCheck)) < 1e-9;
        }

        public EnOperationResultState Mod(double Divisor)
        {
            if (Divisor == 0)
                return EnOperationResultState.ErrorDivideByZero;


            if (IsInteger(Result) && IsInteger(Divisor))
            {
                int LeftOperand = ((int)Math.Round(Result));
                int RightOperand = ((int)Math.Round(Divisor));

                Result = LeftOperand % RightOperand;
                
                return EnOperationResultState.Successful;
            }
            else
                return EnOperationResultState.ErrorOperandNotInteger;
        }

    }
}
