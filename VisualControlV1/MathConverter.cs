//-----------------------------------------------------------------------
// <copyright file="MathConverter.cs" company="Rachel Lim">
//     Copyright (c) 2011 . All rights reserved.
//     This class is freely available on the internet created by Rachel Lim. 
//     Contact Rachel Lim at https://rachel53461.wordpress.com/tag/math-converter/
//     for detailed copyright restrictions.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualControlV3
{
    /// <summary>
    /// Parses a mathematical string and executes the calculation
    /// </summary>
    public class MathConverter : IValueConverter
    {
        private static readonly char[] AllOperators = { '+', '-', '*', '/', '%', '(', ')' };

        private static readonly List<string> Grouping = new List<string> { "(", ")" };
        private static readonly List<string> Operators = new List<string> { "+", "-", "*", "/", "%" };

        /// <summary>
        /// Evaluates a mathematical string and keeps track of the results in a List of numbers
        /// </summary>
        /// <param name="mathEquation">reference to the equation</param>
        /// <param name="numbers">reference to the numbers in the numbers list</param>
        /// <param name="index">index of where to start from</param>
        public void EvaluateMathString(ref string mathEquation, ref List<double> numbers, int index)
        {
            // Loop through each mathemtaical token in the equation
            var token = GetNextToken(mathEquation);

            while (token != string.Empty)
            {
                // Remove token from mathEquation
                mathEquation = mathEquation.Remove(0, token.Length);

                // If token is a Grouping character, it affects program flow
                if (Grouping.Contains(token))
                {
                    switch (token)
                    {
                        case "(":
                            EvaluateMathString(ref mathEquation, ref numbers, index);
                            break;

                        case ")":
                            return;
                    }
                }

                // If token is an operator, do requested operation
                if (Operators.Contains(token))
                {
                    // If next token after operator is a parenthesis, call method recursively
                    var nextToken = GetNextToken(mathEquation);
                    if (nextToken == "(")
                    {
                        EvaluateMathString(ref mathEquation, ref numbers, index + 1);
                    }

                    // Verify that enough numbers exist in the List<double> to complete the operation
                    // and that the next token is either the number expected, or it was a ( meaning
                    // that this was called recursively and that the number changed
                    if (numbers.Count > index + 1 &&
                        (double.Parse(nextToken) == numbers[index + 1] || nextToken == "("))
                    {
                        switch (token)
                        {
                            case "+":
                                numbers[index] = numbers[index] + numbers[index + 1];
                                break;
                            case "-":
                                numbers[index] = numbers[index] - numbers[index + 1];
                                break;
                            case "*":
                                numbers[index] = numbers[index] * numbers[index + 1];
                                break;
                            case "/":
                                numbers[index] = numbers[index] / numbers[index + 1];
                                break;
                            case "%":
                                numbers[index] = numbers[index] % numbers[index + 1];
                                break;
                        }

                        numbers.RemoveAt(index + 1);
                    }
                    else
                    {
                        // Handle Error - Next token is not the expected number
                        throw new FormatException("Next token is not the expected number");
                    }
                }

                token = GetNextToken(mathEquation);
            }
        }

        // Gets the next mathematical token in the equation
        private string GetNextToken(string mathEquation)
        {
            // If we're at the end of the equation, return string.empty
            if (mathEquation == string.Empty)
            {
                return string.Empty;
            }

            // Get next operator or numeric value in equation and return it
            var tmp = "";
            foreach (var c in mathEquation)
            {
                if (AllOperators.Contains(c))
                {
                    return tmp == "" ? c.ToString() : tmp;
                }

                tmp += c;
            }

            return tmp;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Parse value into equation and remove spaces
            var mathEquation = parameter as string;
            mathEquation = mathEquation.Replace(" ", "");
            mathEquation = mathEquation.Replace("@VALUE", value.ToString());

            // Validate values and get list of numbers in equation
            var numbers = new List<double>();
            double tmp;

            foreach (var s in mathEquation.Split(AllOperators))
            {
                if (s != string.Empty)
                {
                    if (double.TryParse(s, out tmp))
                    {
                        numbers.Add(tmp);
                    }
                    else
                    {
                        // Handle Error - Some non-numeric, operator, or Grouping character found in string
                        throw new InvalidCastException();
                    }
                }
            }

            // Begin parsing method
            EvaluateMathString(ref mathEquation, ref numbers, 0);

            // After parsing the numbers list should only have one value - the total
            return numbers[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}