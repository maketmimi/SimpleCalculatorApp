using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace SimpleCalculatorApp
{
	public partial class FrStandredCalacScreen : Form
	{
		private readonly Calculator calculator = new Calculator();
		private Calculator.EnOperations CurrentOperation = Calculator.EnOperations.Add;
		private EnCalculatorState CurrentCalculatorState = EnCalculatorState.Ready;

		private readonly Dictionary<Calculator.EnOperations, string> OperationToSymbol = 
			new Dictionary<Calculator.EnOperations, string>()
		{
			{Calculator.EnOperations.Add, "+" },
			{Calculator.EnOperations.Subtract, "-" },
			{Calculator.EnOperations.Multiply, "×" },
			{Calculator.EnOperations.Divide, "÷" },
			{Calculator.EnOperations.Mod, "Mod" }
		};

		private enum EnCalculatorState
		{
			Ready,
			Error,
			FinalResult
		}

		private enum EnMainDisplayState : byte
		{
			Empty,
			HasResult_CannotPerformOperation,
			HasResult_CanPerformOperation,
			Default,
			HasInput,
			Error
		}

		public FrStandredCalacScreen()
		{
			InitializeComponent();
			InitializeOperationsButtons();
			SetMainDisplyToDefault();
		}

		private void ResetCalculator()
		{
			ClearSubDisplay();
			SetMainDisplyToDefault();
			calculator.Reset();
			CurrentOperation = Calculator.EnOperations.Add;
			CurrentCalculatorState = EnCalculatorState.Ready;
		}

		private void PrepareCalculator()
		{
			switch (CurrentCalculatorState)
			{
				case EnCalculatorState.Error:
				case EnCalculatorState.FinalResult:
					ResetCalculator();
					break;
			}
		}

		private EnMainDisplayState GetCurrentMainDisplayState()
		{
			return (TxtMainDisplay.Tag is EnMainDisplayState StateToReturn) ? StateToReturn : EnMainDisplayState.Error;
		}

		private void SetMainDisplayState(EnMainDisplayState State)
		{
			TxtMainDisplay.Tag = State;
		}

		private void InitializeOperationsButtons()
		{
			BtAdd.Tag = Calculator.EnOperations.Add;
			BtSubtract.Tag = Calculator.EnOperations.Subtract;
			BtMultiply.Tag = Calculator.EnOperations.Multiply;
			BtDivide.Tag = Calculator.EnOperations.Divide;
			BtMoudule.Tag = Calculator.EnOperations.Mod;
		}

		private void SetMainDisplyToDefault()
		{
			TxtMainDisplay.Text = "0";
			SetMainDisplayState(EnMainDisplayState.Default);
		}

		private void ShowCharOnMainDisplay_Click(object sender, EventArgs e)
		{
			PrepareDisplaysForNextInput();

			if (sender is Button BtChar)    
			{
				// the tag stores the Char that the button represents
				AddInputToMainDisplay(BtChar.Tag.ToString());
			}
		}

        private void ClearMainDisplay_Click(object sender, EventArgs e)
		{
			SetMainDisplyToDefault();
		}

		private void AddInputToMainDisplay(string Input)
		{
			TxtMainDisplay.Text += Input;
			SetMainDisplayState(EnMainDisplayState.HasInput);
		}

		private void ShowPeriodOnMainDisplay_Click(object sender, EventArgs e)
		{
			PrepareDisplaysForNextInput();

			if (!TxtMainDisplay.Text.Contains("."))
			{
				if (EnMainDisplayState.Empty.Equals(GetCurrentMainDisplayState()))
					SetMainDisplyToDefault();
				AddInputToMainDisplay(".");
			}
		}

		private void ClearSubDisplay()
		{
			TxtSubDisplay.Clear();
		}

		private bool IsInputZero()
		{
			return TxtMainDisplay.Text == "0";
		}

		private void PrepareDisplaysForNextInput()
		{
			PrepareCalculator();

			EnMainDisplayState CurrentState = GetCurrentMainDisplayState();

			if (CurrentState != EnMainDisplayState.HasInput || IsInputZero())
			{
				TxtMainDisplay.Clear();
				SetMainDisplayState(EnMainDisplayState.Empty);
			}
		}

		private bool IsOperationButton(object ObjectToCheck)
		{
			return (ObjectToCheck is Button BtOperation) 
				&& (BtOperation.Tag is Calculator.EnOperations OperationToPerform);

		}

		private void AddContentToSubDisplay(string Content)
		{
			TxtSubDisplay.Text += Content;
		}

		private void AddValidOperationResultToMainDisplay()
		{
			TxtMainDisplay.Text = calculator.Result.ToString();
			SetMainDisplayState(EnMainDisplayState.HasResult_CannotPerformOperation);
		}

		private void ShowOperationResult(Calculator.EnOperationResultState ResultState)
		{
			AddContentToSubDisplay(TxtMainDisplay.Text);
			
			switch (ResultState)
			{
				case Calculator.EnOperationResultState.Successful:
					AddValidOperationResultToMainDisplay();
					break;
				case Calculator.EnOperationResultState.ErrorDivideByZero:
					TxtMainDisplay.Text = "Cannot divide by 0";
					SetMainDisplayState(EnMainDisplayState.Error);
					break;
				case Calculator.EnOperationResultState.ErrorOperandNotInteger:
					TxtMainDisplay.Text = "Operand Not Integer";
					SetMainDisplayState(EnMainDisplayState.Error);
					break;
				case Calculator.EnOperationResultState.ErrorFailed:
					TxtMainDisplay.Text = "Unexpected Error";
					SetMainDisplayState(EnMainDisplayState.Error);
					break;
			}
		}

		private void ContinueFromFinalResult()
		{
            if (double.TryParse(TxtMainDisplay.Text, out double FinalResult))
            {
				ResetCalculator();
				TxtMainDisplay.Clear();
				AddInputToMainDisplay(FinalResult.ToString());
				PerformOperation(CurrentOperation);
            }
            else
            {
				PutInvalidInputState();
            }
        }

		private void PrepareCalculatorForNextOperation()
		{
			switch (CurrentCalculatorState)
			{
				case EnCalculatorState.Error:
					ResetCalculator(); 
					break;
			}
		}

		private bool CanPerformOperation()
		{
			switch (GetCurrentMainDisplayState())
			{
				case EnMainDisplayState.HasInput:
				case EnMainDisplayState.HasResult_CanPerformOperation:
				case EnMainDisplayState.Default:
					return true;
				default:
					return false;
			}
        }

		private void PerformOperation(Calculator.EnOperations OperationToPerform)
		{
			if (!CanPerformOperation()) return;

			bool IsOperationSuccessful = false;

			if (double.TryParse(TxtMainDisplay.Text, out double InputOnMainDisplay))
			{
				Calculator.EnOperationResultState ResultState =
							calculator.PerformOperation(InputOnMainDisplay, OperationToPerform);

				ShowOperationResult(ResultState);

				IsOperationSuccessful = 
					(ResultState == Calculator.EnOperationResultState.Successful);
			}
			else
			{
				PutInvalidInputState();
			}

			if (!IsOperationSuccessful)
				CurrentCalculatorState = EnCalculatorState.Error;
		}

		private void AddOperationSymbolToSubDisplay(Calculator.EnOperations Operation)
		{
			// it has an initial Value not equal to default (0)
            if (TxtSubDisplay.Text.Length == 0)
			{
				SetMainDisplayState(EnMainDisplayState.HasInput);
				PerformOperation(CurrentOperation);
			}

            if (StringHelpers.StringEndsWithAny(TxtSubDisplay.Text, OperationToSymbol.Values.ToArray(), out string PrevSymbol))
            {
				TxtSubDisplay.Text =
					TxtSubDisplay.Text.Substring(0, TxtSubDisplay.Text.Length - PrevSymbol.Length)
					+ OperationToSymbol[Operation];
            }
            else
            {
				AddContentToSubDisplay(OperationToSymbol[Operation]);
            }
        }

		private void OperationButtonClicked(object sender, EventArgs e)
		{
			if (IsOperationButton(sender))
			{
				PrepareCalculatorForNextOperation();

                if (CurrentCalculatorState == EnCalculatorState.FinalResult)
					ContinueFromFinalResult();
				else
					PerformOperation(CurrentOperation);

				Calculator.EnOperations NextOperationToPerform =
					(Calculator.EnOperations) ((Button)sender).Tag;

				AddOperationSymbolToSubDisplay(NextOperationToPerform);

                CurrentOperation = NextOperationToPerform;
			}
		}

		private void PerformEqualOperation()
		{
			switch (GetCurrentMainDisplayState())
			{
				case EnMainDisplayState.HasResult_CannotPerformOperation:
				case EnMainDisplayState.Default:
					SetMainDisplayState(EnMainDisplayState.HasInput);
					break;
			}

            PerformOperation(CurrentOperation);
            AddContentToSubDisplay("=");
            if (CurrentCalculatorState == EnCalculatorState.Ready)
                CurrentCalculatorState = EnCalculatorState.FinalResult;
        }

		private void BtEqual_Click(object sender, EventArgs e)
		{
			switch (CurrentCalculatorState)
			{
				case EnCalculatorState.Error:
				case EnCalculatorState.FinalResult:
					ResetCalculator();
					break;
				case EnCalculatorState.Ready:
					PerformEqualOperation();
                    break;
			}
		}

        private void BtClearAll_Click(object sender, EventArgs e)
        {
			ResetCalculator();
        }

        private void BtBackSpace_Click(object sender, EventArgs e)
        {
			PrepareCalculator();

			if (TxtMainDisplay.Text.Length > 0)
			{
				TxtMainDisplay.Text = 
					TxtMainDisplay.Text.Substring(0, TxtMainDisplay.Text.Length - 1);
				SetMainDisplayState(EnMainDisplayState.HasInput);
			}

			if (TxtMainDisplay.Text.Length == 0)
				SetMainDisplyToDefault();
        }

		private void PutClaculatorInErrorState()
		{
            SetMainDisplayState(EnMainDisplayState.Error);
            CurrentCalculatorState = EnCalculatorState.Error;
        }

		private void PutInvalidInputState()
		{
            TxtMainDisplay.Text = "Invalid Input";
			PutClaculatorInErrorState();
        }

		private void PutMathErrorState()
		{
			TxtMainDisplay.Text = "Math Error!";
			PutClaculatorInErrorState();
		}

        private void BtSignToggle_Click(object sender, EventArgs e)
        {
            if (double.TryParse(TxtMainDisplay.Text, out double Input))
            {
				Input = -Input;
				TxtMainDisplay.Text = Input.ToString();
            }
			else
			{
				PutInvalidInputState();
			}
        }

		private void PerformSqrtOperation(double Input)
		{
            if (Input < 0)
            {
                PutMathErrorState();
                return;
            }

			double Result = Calculator.GetSqrt(Input);

			TxtMainDisplay.Clear();
			TxtMainDisplay.Text = Result.ToString();
			SetMainDisplayState(EnMainDisplayState.HasResult_CanPerformOperation);
        }

        private void BTSqrt_Click(object sender, EventArgs e)
        {
            if (double.TryParse(TxtMainDisplay.Text, out double Input))
            {
				PerformSqrtOperation(Input);
            }
            else
            {
				PutInvalidInputState();
            }
        }

		private void PerformSquareOperation(double Input)
		{
            double Result = Calculator.GetSquare(Input);

            TxtMainDisplay.Clear();
            TxtMainDisplay.Text = Result.ToString();
            SetMainDisplayState(EnMainDisplayState.HasResult_CanPerformOperation);
        }

        private void BtSquare_Click(object sender, EventArgs e)
        {
            if (double.TryParse(TxtMainDisplay.Text, out double Input))
            {
                PerformSquareOperation(Input);
            }
            else
            {
                PutInvalidInputState();
            }
        }
    
	}
}
