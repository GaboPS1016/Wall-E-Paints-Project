using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class Interpreter : MonoBehaviour
{
    public  bool error = false;
    public Parser parser;
    public Methods methods;
    public MainScript main; 
    public List<string> LogicOperators = new List<string> { "<", ">", "<=", ">=", "==", "!=", "&&", "||", "true", "false" };
    public  Dictionary<string, int> numVars = new Dictionary<string, int>();
    public  Dictionary<string, bool> boolVars = new Dictionary<string, bool>();
    public void MainInterpreter(List<string> tokens)
    {
        tokenInterpreter(tokens, 0);
    }
    public void tokenInterpreter(List<string> tokens, int index)
    {
        if (index >= tokens.Count) return;
        if (error) return;

        //instructions, no functions
        if (IsMethod(tokens[index]) && !IsFunction(tokens[index]))
        {
            List<string> methodlist = parser.MethodParsing(tokens[index]);
            Enum.TryParse(methodlist[0], out Method method);
            int numParameters = methods.ParametersDictionary[method];
            List<int> parameters = new List<int>();

            if (numParameters != methodlist.Count - 1)
            {
                main.log.text = "ERROR!!!! INCORRECT NUMBER OF PARAMETERS";
                return;
            }
            string colorstr = "";

            //fill parameters lists
            if (method == Method.Color) colorstr = methodlist[1];
            else
            {
                for (int i = 1; i <= numParameters; i++)
                {
                    parameters.Add(DoingOperation(methodlist[i]));
                }
            }
            if (error) return;
            //execute the method
            methods.DoAction(method, parameters, colorstr);
            if (error) return;
            //next token
            tokenInterpreter(tokens, index + 1);
        }
        //functions
        else if (IsFunction(tokens[index]))
        {
            main.log.text = "ERROR!!!! FUNCTION NOT ASSIGNED TO A VARIABLE";
            return;
        }
        //variable declaration
        else if (IsVariableDeclaration(tokens[index]))
        {
            if (!IsValidVariableName(tokens[index - 1]))
            {
                main.log.text = "ERROR!!!! INCORRECT VARIABLE NAME";
                error = true;
                return;
            }
            //boolean var
            if (IsBool(tokens[index + 1]))
            {
                //check if it exists
                bool result = DoingBoolean(tokens[index + 1]);
                if (error) return;
                if (boolVars.ContainsKey(tokens[index - 1])) boolVars[tokens[index - 1]] = result;
                else boolVars.Add(tokens[index - 1], result);
                numVars.Remove(tokens[index - 1]);
            }
            //numeric var
            else
            {
                //check if it exists
                int result = DoingOperation(tokens[index + 1]);
                if (error) return;
                if (numVars.ContainsKey(tokens[index - 1])) numVars[tokens[index - 1]] = result;
                else numVars.Add(tokens[index - 1], result);
                boolVars.Remove(tokens[index - 1]);
            }
            if (error) return;
            //next token
            try
            {
                tokenInterpreter(tokens, index + 2);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }
        //loop
        else if (IsLoop(tokens[index]))
        {
            if (tokens[index + 1][0] != '[' || !IsBool(tokens[index + 2]))
            {
                main.log.text = "ERROR!!!! INCORRECT LOOP SINTAXIS";
                error = true;
                return;
            }
            //check the condition
            if (DoingBoolean(tokens[index + 2]))
            {
                string label = tokens[index + 1].Substring(1, tokens[index + 1].Length - 2);
                int target = Find(tokens, label);
                if (target == -1)
                {
                    main.log.text = "ERROR!!!! LABEL \"" + label + "\" NOT FOUND";
                    error = true;
                    return;
                }
                else
                {
                    tokenInterpreter(tokens, target);
                }
            }
            else
            {
                if (error) return;
                try
                {
                    tokenInterpreter(tokens, index + 3);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }
            }
        }
        //next token
        else tokenInterpreter(tokens, index + 1);
    }
    public  int DoingOperation(string token)
    {
        List<string> toks = parser.Parsing(token);
        List<int> numtoks = new List<int>();
        List<string> signtoks = new List<string>();
        bool firstnegative = false;

        if (IsBool(token))
        {
            main.log.text = "ERROR!!!! THE OPERATION IS BOOLEAN, CAN'T OPERATE";
            error = true;
            return 0;
        }
        if (toks.Count == 1)
        {
            if (toks[0][0] == '(') return DoingOperation(toks[0]); 
            else if (int.TryParse(toks[0], out int singlenum)) return singlenum;
            else toks = parser.Parsing(toks[0]);
        }    
        //first num negative?
        if (toks[0] == "-")
        {
            firstnegative = true;
            toks.RemoveAt(0);
        }
        for (int i = 0; i < toks.Count; i++)
        {
            //numbers
            if (i % 2 == 0)
            {
                //is a number
                if (int.TryParse(toks[i], out int num))
                {
                    numtoks.Add(num);
                }
                //is a function
                else if (IsFunction(toks[i]))
                {
                    numtoks.Add(FunctionCrafter(toks[i]));
                }
                //is a parenthesis with numbers
                else if (toks[i][0] == '(')
                {
                    numtoks.Add(DoingOperation(toks[i]));
                }
                //is an existent variable
                else if (numVars.ContainsKey(toks[i]))
                {
                    int value = numVars[toks[i]];
                    numtoks.Add(value);
                }
                //is a boolean variable
                else if (boolVars.ContainsKey(toks[i]))
                {
                    main.log.text = "ERROR!!!! THE VARIABLE \"" + toks[i] + "\" IS BOOLEAN";
                    error = true;
                    return 0;
                }
                //expression without spaces
                else if (parser.Parsing(toks[i]).Count > 1) numtoks.Add(DoingOperation(toks[i]));
                //doesnt exist
                else
                {
                    main.log.text = "ERROR!!!! \"" + toks[i] + "\" DOES NOT EXIST";
                    error = true;
                    return 0;
                }
            }
            //operators
            else
            {
                signtoks.Add(toks[i]);
            }
        }
        if (firstnegative)
        {
            numtoks[0] = -1 * numtoks[0];
            firstnegative = false;
        }
        return Operate(numtoks, signtoks);
    }
    public  int Operate(List<int> nums, List<string> signs)
    {
        for (int i = 0; i < signs.Count; i++)
        {
            if (signs[i] == "**")
            {
                nums[i] = (int)Math.Pow(nums[i], nums[i + 1]);
                nums.RemoveAt(i + 1);
                signs.RemoveAt(i);
                i = -1;
            }
        }
        for (int i = 0; i < signs.Count; i++)
        {
            switch (signs[i])
            {
                case "*":
                    nums[i] = nums[i] * nums[i + 1];
                    nums.RemoveAt(i + 1);
                    signs.RemoveAt(i);
                    i = -1;
                    break;
                case "/":
                    if (nums[i + 1] == 0)
                    {
                        main.log.text = "ERROR!!! DIVISION BY ZERO!!!";
                        error = true;
                        return 1;
                    }
                    nums[i] = nums[i] / nums[i + 1];
                    nums.RemoveAt(i + 1);
                    signs.RemoveAt(i);
                    i = -1;
                    break;
                case "%":
                    nums[i] = nums[i] % nums[i + 1];
                    nums.RemoveAt(i + 1);
                    signs.RemoveAt(i);
                    i = -1;
                    break;
            }
        }
        for (int i = 0; i < signs.Count; i++)
        {
            if (signs[i] == "+" || signs[i] == "-")
            {
                switch (signs[i])
                {
                    case "+":
                        nums[i] = nums[i] + nums[i + 1];
                        nums.RemoveAt(i + 1);
                        signs.RemoveAt(i);
                        i = -1;
                        break;
                    case "-":
                        nums[i] = nums[i] - nums[i + 1];
                        nums.RemoveAt(i + 1);
                        signs.RemoveAt(i);
                        i = -1;
                        break;
                }
            }
        }
        return nums[0];
    }
    public  bool DoingBoolean(string token)
    {
        //recursive method to operate ands and ors
        List<string> toks = parser.BoolParsing(token);
        List<string> boolsigns = new List<string>();
        List<bool> predicates = new List<bool>();
        //single tok
        if (toks.Count == 1)
        {
            if (toks[0][0] == '(')
            {
                string inntok = toks[0].Substring(1, toks[0].Length - 2);
                return DoingBoolean(inntok);
            }
            else if (toks[0] == "true") return true;
            else if (toks[0] == "false") return false;
            //bool var
            else if (boolVars.ContainsKey(toks[0])) return boolVars[toks[0]];
            //num var
            else if (numVars.ContainsKey(toks[0]))
            {
                main.log.text = "ERROR!!!! " + toks[0] + " IS A NUMERICAL VARIABLE, NOT BOOLEAN";
                error = true;
                return false;
            }
            //is a predicate
            else if (IsBool(toks[0])) return DoingPredicates(toks[0]);
            //nothing
            else
            {
                main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION, " + toks[0] + " DO NOT EXISTS";
                error = true;
                return false;
            }
        }
        for (int i = 0; i < toks.Count; i++)
        {
            //predicates
            if (i % 2 == 0)
            {
                //contains && or ||
                if (IsComplexBool(toks[i])) predicates.Add(DoingBoolean(toks[i]));
                //predicates
                else if (IsBool(toks[i])) predicates.Add(DoingPredicates(toks[i]));
                //is a bool variable
                else if (boolVars.ContainsKey(toks[i])) predicates.Add(boolVars[toks[i]]);
                //is a num variable
                else if (numVars.ContainsKey(toks[i]))
                {
                    main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION, CANT OPERATE NUM VARS WITH LOGIC OPERATORS";
                    error = true;
                    return false;
                }
                else
                {
                    main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION, " + toks[i] + " NOT EXISTS";
                    error = true;
                    return false;
                }
            }
            //operators || and &&
            else
            {
                boolsigns.Add(toks[i]);
            }
        }
        return OperateBooleans(predicates, boolsigns);
    }
    public  bool DoingPredicates(string token)
    {
        List<string> toks = parser.PredicateParsing(token);
        List<string> boolsigns = new List<string>();
        List<int> nums = new List<int>();
        //single tok
        if (toks.Count == 1)
        {
            if (toks[0][0] == '(')
            {
                string inntok = toks[0].Substring(1, toks[0].Length - 2);
                return DoingPredicates(inntok);
            }
            else if (toks[0] == "true") return true;
            else if (toks[0] == "false") return false;
            //bool var
            else if (boolVars.ContainsKey(toks[0])) return boolVars[toks[0]];
            //num var
            else if (numVars.ContainsKey(toks[0]))
            {
                main.log.text = "ERROR!!!! " + toks[0] + " IS A NUMERICAL VARIABLE, NOT BOOLEAN";
                error = true;
                return false;
            }
            //nothing
            else
            {
                main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION \""+ toks[0] +"\"";
                error = true;
                return false;
            }
        }
        for (int i = 0; i < toks.Count; i++)
        {
            //nums
            if (i % 2 == 0)
            {
                //expressions
                if (IsBool(toks[i]))
                {
                    main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION";
                    error = true;
                    return false;
                }
                else
                {
                    nums.Add(DoingOperation(toks[i]));
                }
            }
            //logic operators
            else
            {
                boolsigns.Add(toks[i]);
            }
        }
        //only one operator
        if (boolsigns.Count > 1)
        {
            main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION, TOO MANY BOOL OPERATORS IN PREDICATE ";
            error = true;
            return false;
        }
        return OperatePredicates(nums, boolsigns[0]);
    }
    public  bool OperateBooleans(List<bool> predicates, List<string> boolsigns)
    {
        for (int i = 0; i < boolsigns.Count; i++)
        {
            if (boolsigns[i] == "||")
            {
                predicates[i] = predicates[i] || predicates[i + 1];
                predicates.RemoveAt(i + 1);
                boolsigns.RemoveAt(i);
                i = -1;
            }
        }
        for (int i = 0; i < boolsigns.Count; i++)
        {
            if (boolsigns[i] == "&&")
            {
                predicates[i] = predicates[i] && predicates[i + 1];
                predicates.RemoveAt(i + 1);
                boolsigns.RemoveAt(i);
                i = -1;
            }
        }
        return predicates[0];
    }
    public  bool OperatePredicates(List<int> nums, string boolsign)
    {
        //operating
        switch (boolsign)
        {
            case "==":
                return nums[0] == nums[1];
            case "!=":
                return nums[0] != nums[1];
            case "<":
                return nums[0] < nums[1];
            case ">":
                return nums[0] > nums[1];
            case "<=":
                return nums[0] <= nums[1];
            case ">=":
                return nums[0] >= nums[1];
            default:
                main.log.text = "ERROR!!!! INCORRECT BOOLEAN EXPRESSION";
                error = true;
                return false;
        }
    }
    public  int FunctionCrafter(string token)
    {
        List<string> function = parser.MethodParsing(token);
        Enum.TryParse(function[0], out Method method);
        int numParameters = methods.ParametersDictionary[method];
        List<int> parameters = new List<int>();
        string colorstr = "";
        if (numParameters != function.Count - 1)
        {
            main.log.text = "ERROR!!!! INCORRECT NUMBER OF PARAMETERS";
            error = true;
            return 0;
        }

        if (method == Method.GetColorCount || method == Method.IsBrushColor || method == Method.IsCanvasColor)
        {
            for (int i = 2; i <= numParameters; i++)
            {
                parameters.Add(DoingOperation(function[i]));
            }
            colorstr = function[1];
        }
        else
        {
            for (int i = 1; i <= numParameters; i++)
            {
                parameters.Add(DoingOperation(function[i]));
            }
        }
        return methods.DoFunction(method, parameters, colorstr);
    }
    public  bool IsMethod(string token)
    {
        string newtoken = "";
        for (int i = 0; i < token.Length; i++)
        {
            if (token[i] == '(') break;
            newtoken += token[i];
        }
        return Enum.IsDefined(typeof(Method), newtoken);
    }
    public  bool IsFunction(string token)
    {
        string newtoken = "";
        for (int i = 0; i < token.Length; i++)
        {
            if (token[i] == '(') break;
            newtoken += token[i];
        }
        if (Enum.TryParse(newtoken, out Method numMethod))
        {
            if ((int)numMethod >= 7) return true;
        }
        return false;
    }
    public  bool IsVariableDeclaration(string token)
    {
        return token == "<-";
    }
    public bool IsLoop(string token)
    {
        return token == "GoTo";
    }
    public  bool IsValidVariableName(string token)
    {
        //return SyntaxFacts.IsValidIdentifier(token);

        // first character
        if (!char.IsLetter(token[0]) && token[0] != '_') return false;

        // the rest of the characters
        for (int i = 1; i < token.Length; i++)
        {
            if (!char.IsLetterOrDigit(token[i]) && token[i] != '_') return false;
        }
        // verify if the variable is a keyword
        if (Enum.IsDefined(typeof(Method), token) ||
            Enum.IsDefined(typeof(CellColor), token) ||
            token == "true" ||
            token == "false" ||
            LogicOperators.Contains(token)) return false;
        return true;
    }
    public  bool IsComplexBool(string token)
    {
        //has && or ||
        for (int i = 0; i < token.Length - 1; i++)
        {
            if ((token[i] == '&' && token[i + 1] == '&') || token[i] == '|' && token[i + 1] == '|') return true;
        }
        return false;
    }
    public  bool IsBool(string token)
    {
        if (LogicOperators.Contains(token)) return true;
        if (boolVars.ContainsKey(token)) return true;
        if (token == "") return false;
        List<string> toks = parser.Parsing(token);
        if (toks.Count == 1)
        {
            if (toks[0][0] == '(')
            {
                if (toks[0] == token) return false;
                if (IsBool(toks[0].Substring(1, toks[0].Length - 2))) return true;
            }
        }
        for (int i = 0; i < toks.Count; i++)
        {
            if (toks[i] != token && IsBool(toks[i])) return true;
        }
        return false;
    }
    public  int Find(List<string> tokens, string label)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] == label)
            {
                return i;
            }            
        }
        return -1;
    }
}
