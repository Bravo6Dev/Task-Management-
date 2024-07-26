using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BuisnessLayer.Classes
{
    public class Validation
    {
        static private Regex Re;
        static public bool EmptyInputs(params string[] Text)
                => Text.Any(T => string.IsNullOrEmpty(T));
        static public bool PhoneNumber(string input)
        {
            Re = new Regex("^(091|092|093|094)(\\s)?\\d{7}$");
            return Re.IsMatch(input);
        }
        static public bool Names(string input)
        {
            Re = new Regex("^\\D*$");
            return Re.IsMatch(input);
        }
    }
}
