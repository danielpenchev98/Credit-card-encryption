using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MyRule
{
    public class MyValidationRuleUsername : ValidationRule
    {
        /// <summary>
        /// Funtion for validating the input while the user is typing it
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {   // parameter value holds the  data subject for validation

               string enteredName = value.ToString();
               Regex rule = new Regex(@"^[a-zA-Z][a-zA-Z0-9\-_\.]{7,25}$");
               if (!rule.IsMatch(enteredName))
               {

                    return new ValidationResult(false, "The username should:\n- begin with a letter\n- should contain only a-z, A-Z, 0-9, '-', '_', '.'\n- its length should be 8-25 characters");
               }
               else
               {
                    return new ValidationResult(true, null);
               }
        }
    }
}
